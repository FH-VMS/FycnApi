using Fycn.Model;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Fycn.Utility;
using Fycn.Model.Sys;

namespace Fycn.SqlDataAccess
{
    public class SqlGenerator:ISqlGenerator
    {
        public static CommDbHelper DbHelper;

        public static ILogger Logger
        {
            get { return LogFactory.GetInstance(); }
        }
        public SqlGenerator()
        {
            if (DbHelper == null)
                DbHelper = new CommDbHelper();
        }



        public void BeginTransaction()
        {
            CommDbTransaction.BeginTran();
        }

        public void CommitTransaction()
        {
            CommDbTransaction.Commit();
        }

        public void RollBack()
        {
            CommDbTransaction.RollBack();
        }

        public int Create<T>(T t)
        {
            return Create(new List<T>
            {
                t
            });
        }

        public int CreateForAutoId<T>(T t)
        {
            return Create(new List<T>
            {
                t
            }, true)[0];
        }

        public int Create<T>(List<T> ts)
        {
            return Create(ts, false)[0];
        }

        public List<int> CreateForAutoId<T>(List<T> ts)
        {
            return Create(ts, true);
        }
        private List<int> Create<T>(List<T> ts, bool isRetAutoId)
        {
            if (ts == null || ts.Count == 0)
                return new List<int> { 0 };

            var objValid = ObjectValidation(ts);
            if (objValid.Count > 0)
            {
                var r = new StringBuilder();
                foreach (var valid in objValid)
                {
                    r.AppendFormat("[{0}]", valid);
                }
                throw new CommDalException(20, r.ToString());
            }

            if (!isRetAutoId)
            {
                var result = CreateBatch(ts);
                return new List<int> { result };
            }

            var idLst = new List<int>();
            foreach (var t in ts)
            {
                var result = CreateSingle(t);
                idLst.Add(result > 0 ? GetSequenceId<T>() : 0);
            }
            return idLst;
        }

        /// <summary>
        /// 进行数据验证判断
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        private List<string> ObjectValidation<T>(IEnumerable<T> t)
        {
            var pis = CommDbHelper.GetModelDataPropertyInfos(typeof(T));
            var result = new List<string>();
            //Check null
            var notAllowedNullColumn = pis.Where(modelPropertyInfo => modelPropertyInfo.IsNotNull).ToList();
            if (notAllowedNullColumn.Count <= 0)
                return result;
            foreach (var obj in t)
            {
                foreach (var column in notAllowedNullColumn)
                {
                    var v = column.PropertyInfo.GetValue(obj, null);
                    if (v == null || String.IsNullOrEmpty(v.ToString()))
                    {
                        result.Add(column.PropertyName);
                    }
                }
            }
            return result;
        }
        private int GetSequenceId<T>()
        {
            var autoIdSql = SqlConstructor.GetSequeceIdSql<T>();
            if (String.IsNullOrEmpty(autoIdSql)) return 0;
            var seqId = int.Parse(DbHelper.ExecuteScalar(autoIdSql).ToString());
            return seqId;
        }

        private int CreateBatch<T>(List<T> ts)
        {
            var sql = SqlConstructor.GetCreateSQL(ts);
            return DbHelper.ExecuteNonQuery(sql);
        }

        private int CreateSingle<T>(T t)
        {
            return CreateBatch(new List<T> { t });
        }

        private List<T> GetFromDictionary<T>(CommonSqlKey sqlKey, IDictionary<string, object> parmDic)
        {
            DbDataReader dr = null;
            var tLst = new List<T>();
            var logStep = 0;
            try
            {
                //如果为空sql可以，则根据类型和传入参数自动构造select语句，同时where条件为传入参数
                var sqlTxt = sqlKey == CommonSqlKey.Null ? SqlConstructor.GetSelectSqlByParmDic<T>(parmDic) : CommSqlText.Instance[sqlKey];
                logStep++;
                var sqlParameters = sqlKey == CommonSqlKey.Null ? null : parmDic; // SqlConstructor.MakeParms(parmDic, CommSqlText.SqlParms[sqlKey]);
                sqlTxt = SqlConstructor.FilterSQLWithInsteadValue(sqlTxt, sqlParameters);
                logStep++;
                sqlParameters = sqlKey == CommonSqlKey.Null ? null : SqlConstructor.FilterParmsWithList(sqlParameters, CommSqlText.SqlParms[sqlKey]);
                logStep++;
                dr = DbHelper.ExecuteReader(sqlTxt, sqlParameters);
                logStep = 100;
                if (dr == null)
                    return tLst;
                var hasCount = dr.Read();
                while (hasCount)
                {
                    logStep += 10;
                    var t = MakeMapToObject<T>(dr);
                    if (t != null)
                        tLst.Add(t);
                    logStep += 1;
                    hasCount = dr.Read();
                }
            }
            catch (MySqlException ee)
            {
                Logger.LogInfo(String.Format("GetFromDictionary ERROR STEP:{0}, EXCEPTION:{1}", logStep, ee.Message), 0, LogType.ERROR);
                throw ee;
            }
            finally
            {
                if (dr != null)
                {
                    dr.Close();
                    dr.Dispose();

                }
            }
            return tLst;
        }

        private int GetCountFromDictionary<T>(CommonSqlKey sqlKey, IDictionary<string, object> parmDic)
        {
            DbDataReader dr = null;
            int tLst = 0;
            var logStep = 0;
            try
            {
                //如果为空sql可以，则根据类型和传入参数自动构造select语句，同时where条件为传入参数
                var sqlTxt = sqlKey == CommonSqlKey.Null ? SqlConstructor.GetSelectSqlByParmDic<T>(parmDic) : CommSqlText.Instance[sqlKey];
                logStep++;
                var sqlParameters = sqlKey == CommonSqlKey.Null ? null : parmDic; // SqlConstructor.MakeParms(parmDic, CommSqlText.SqlParms[sqlKey]);
                sqlTxt = SqlConstructor.FilterSQLWithInsteadValue(sqlTxt, sqlParameters);
                logStep++;
                sqlParameters = sqlKey == CommonSqlKey.Null ? null : SqlConstructor.FilterParmsWithList(sqlParameters, CommSqlText.SqlParms[sqlKey]);
                logStep++;
                tLst = DbHelper.ExecuteScalar(sqlTxt, sqlParameters) == null
              ? 0 : int.Parse(DbHelper.ExecuteScalar(sqlTxt, sqlParameters).ToString());
            }
            catch (MySqlException ee)
            {
                Logger.LogInfo(String.Format("GetFromDictionary ERROR STEP:{0}, EXCEPTION:{1}", logStep, ee.Message), 0, LogType.ERROR);
                throw ee;
            }
            finally
            {
                if (dr != null)
                {
                    dr.Close();
                    dr.Dispose();

                }
            }
            return tLst;
        }

        public T Get<T>(object value) where T : class
        {
            var properties = CommDbHelper.GetModelDataPropertyInfos(typeof(T));
            var parmDic = new Dictionary<string, object>();
            foreach (var propertyInfo in properties)
            {
                if (!propertyInfo.IsPrimaryColumn)
                    continue;
                parmDic.Add(propertyInfo.ColumnName, value);
                break;
            }
            var resultLst = GetFromDictionary<T>(CommonSqlKey.Null, parmDic);
            return resultLst != null && resultLst.Count > 0 ? resultLst[0] : null;
        }

        public T Get<T>(T t) where T : class
        {
            var resultLst = Load(t);
            return resultLst != null && resultLst.Count > 0 ? resultLst[0] : null;
        }

        public T Get<T>(CommonSqlKey sqlKey, T t) where T : class
        {
            var resultLst = Load(sqlKey, t);
            return resultLst != null && resultLst.Count > 0 ? resultLst[0] : null;
        }

        public T Get<T>(CommonSqlKey sqlKey, IDictionary<string, object> parmDic) where T : class
        {
            var resultLst = Load<T>(sqlKey, parmDic);
            return resultLst != null && resultLst.Count > 0 ? resultLst[0] : null;
        }


        public List<T> Load<T>()
        {
            return GetFromDictionary<T>(CommonSqlKey.Null, null);
        }

        public List<T> Load<T>(T parm)
        {
            var sqlTxt = SqlConstructor.GetSelectSqlByObj(parm);
            var tLst = new List<T>();
            var dr = DbHelper.ExecuteReader(sqlTxt);
            while (dr.Read())
            {
                var t = MakeMapToObject<T>(dr);
                if (t != null)
                    tLst.Add(t);
            }
            return tLst;
        }

        public List<T> Load<T>(CommonSqlKey sqlKey)
        {
            return GetFromDictionary<T>(sqlKey, null);
        }

        public List<T> Load<T>(CommonSqlKey sqlKey, T parmObj)
        {
            var parmDic = SqlConstructor.MakeParms(parmObj);

            var resultLst = GetFromDictionary<T>(sqlKey, parmDic);
            return resultLst;
        }

        public List<T> Load<T>(CommonSqlKey sqlKey, IDictionary<string, object> parmObj)
        {
            var parmDic = SqlConstructor.MakeParms(parmObj);
            //parmDic = SqlConstructor.FilterParmsWithList(parmDic, CommSqlText.SqlParms[sqlKey]);
            return GetFromDictionary<T>(sqlKey, parmDic);
        }

        public List<T> LoadByConditions<T>(CommonSqlKey sqlKey, IList<Condition> parmObj)
        {
            var parmDic = SqlConstructor.MakeParms(parmObj);

            return GetFromDictionaryByConditions<T>(sqlKey, parmDic, parmObj);
        }

        public List<T> LoadByConditionsWithOrder<T>(CommonSqlKey sqlKey, IList<Condition> parmObj, string orderField = "", string orderType = "")
        {
            var parmDic = SqlConstructor.MakeParms(parmObj);

            return GetFromDictionaryByConditionsWithOrder<T>(sqlKey, parmDic, parmObj, orderField, orderType);
        }

        public int CountByConditions(CommonSqlKey sqlKey, IList<Condition> parmObj)
        {
            var result = 0;
            var parmDic = SqlConstructor.MakeParms(parmObj);

            var sqlTxt = CommSqlText.Instance[sqlKey];

            var sqlParameters = parmDic;

            //解析where 后查询条件
            List<DbParameter> parameter;
            var whereSql = ConditionHandler.GetWhereSql(parmObj.ToList(), out parameter, sqlKey);

            sqlTxt = sqlTxt + whereSql;

            result = DbHelper.ExecuteScalar(sqlTxt, sqlParameters) == null
                ? 0
                : int.Parse(DbHelper.ExecuteScalar(sqlTxt, sqlParameters).ToString());

            return result;
        }

        public int CountByDictionary<T>(CommonSqlKey sqlKey, IDictionary<string, object> parmObj)
        {
            var parmDic = SqlConstructor.MakeParms(parmObj);

            return GetCountFromDictionary<T>(sqlKey, parmDic);
        }

        public object Single(CommonSqlKey sqlKey)
        {
            return GetSingleFromDicDictionary(sqlKey, null);
        }

        public object Single<T>(CommonSqlKey sqlKey, T parmObj)
        {
            var parmDic = SqlConstructor.MakeParms(parmObj);
            return GetSingleFromDicDictionary(sqlKey, parmDic);
        }

        public object Single(CommonSqlKey sqlKey, IDictionary<string, object> parmDic)
        {
            return GetSingleFromDicDictionary(sqlKey, parmDic);
        }

        private object GetSingleFromDicDictionary(CommonSqlKey sqlKey, IDictionary<string, object> parmDic)
        {
            var logStep = 0;
            try
            {
                if (sqlKey == CommonSqlKey.Null) return null;
                var sqlTxt = CommSqlText.Instance[sqlKey];
                logStep++;
                sqlTxt = SqlConstructor.FilterSQLWithInsteadValue(sqlTxt, parmDic);
                logStep++;
                parmDic = SqlConstructor.FilterParmsWithList(parmDic, CommSqlText.SqlParms[sqlKey]);
                logStep++;

                return DbHelper.ExecuteScalar(sqlTxt, parmDic);
            }
            catch (MySqlException ee)
            {
                Logger.LogInfo(String.Format("GetSingleFromDicDictionary ERROR STEP:{0}, EXCEPTION:{1}", logStep, ee.Message), 0, LogType.ERROR);
                throw ee;
            }
        }

        public DataTable LoadDataTable(string sql)
        {
            return DbHelper.ExecuteDataTable(sql);
        }

        public DataTable LoadDataTable(CommonSqlKey sqlKey, IDictionary<string, object> parmDic)
        {

            var logStep = 0;
            try
            {
                if (sqlKey == CommonSqlKey.Null) return null;
                var sqlTxt = CommSqlText.Instance[sqlKey];
                logStep++;
                sqlTxt = SqlConstructor.FilterSQLWithInsteadValue(sqlTxt, parmDic);
                logStep++;
                parmDic = SqlConstructor.FilterParmsWithList(parmDic, CommSqlText.SqlParms[sqlKey]);
                logStep++;

                return DbHelper.ExecuteDataTable(sqlTxt, parmDic);
            }
            catch (MySqlException ee)
            {
                Logger.LogInfo(String.Format("GetSingleFromDicDictionary ERROR STEP:{0}, EXCEPTION:{1}", logStep, ee.Message), 0, LogType.ERROR);
                throw ee;
            }
        }

        public DataTable LoadDataTableByConditions(CommonSqlKey sqlKey, IList<Condition> parmObj)
        {
            var logStep = 0;
            try
            {
                if (sqlKey == CommonSqlKey.Null) return null;
                var sqlTxt = CommSqlText.Instance[sqlKey];
                logStep++;
                var parmDic = SqlConstructor.MakeParms(parmObj);
                  List<DbParameter> parameter;
                  var whereSql = ConditionHandler.GetWhereSql(parmObj.ToList(), out parameter, sqlKey);
                  sqlTxt = SqlConstructor.FilterSQLWithInsteadValue(sqlTxt + whereSql, parmDic);
                logStep++;
                parmDic = SqlConstructor.FilterParmsWithList(parmDic, CommSqlText.SqlParms[sqlKey]);
                logStep++;

                return DbHelper.ExecuteDataTable(sqlTxt, parmDic);
            }
            catch (MySqlException ee)
            {
                Logger.LogInfo(String.Format("GetSingleFromDicDictionary ERROR STEP:{0}, EXCEPTION:{1}", logStep, ee.Message), 0, LogType.ERROR);
                throw ee;
            }
           
            
        }

       

        public int Update<T>(CommonSqlKey sqlKey, T t)
        {
            var sqlContent = CommSqlText.Instance[sqlKey];
            var sqlParameters = SqlConstructor.MakeParms(t);
            sqlContent = SqlConstructor.FilterSQLWithInsteadValue(sqlContent, sqlParameters);
            sqlParameters = SqlConstructor.FilterParmsWithList(sqlParameters, CommSqlText.SqlParms[sqlKey]);
            var result = DbHelper.ExecuteNonQuery(sqlContent, sqlParameters);
            return result;
        }

        public int Delete<T>(object kv)
        {
            if (!ReflectionHandler.IsValueType(kv))
                return Delete((T)kv);
            var delSql = SqlConstructor.GetDeleteSqlByParmDic<T>(kv);
            var result = DbHelper.ExecuteNonQuery(delSql);
            return result;
        }

        public int Delete<T>(T t)
        {
            var delSql = SqlConstructor.GetDeleteSqlByParmDic(t);
            var result = DbHelper.ExecuteNonQuery(delSql);
            return result;
        }

        public int Delete<T>(CommonSqlKey sqlKey, T t)
        {
            var sqlContent = CommSqlText.Instance[sqlKey];
            var sqlParameters = SqlConstructor.MakeParms(t);
            sqlContent = SqlConstructor.FilterSQLWithInsteadValue(sqlContent, sqlParameters);
            sqlParameters = SqlConstructor.FilterParmsWithList(sqlParameters, CommSqlText.SqlParms[sqlKey]);
            var result = DbHelper.ExecuteNonQuery(sqlContent, sqlParameters);
            return result;
        }

        public object Execute(CommonSqlKey sqlKey, IDictionary<string, object> parms)
        {
            var sqlContent = CommSqlText.Instance[sqlKey];
            var sqlParameters = SqlConstructor.MakeParms(parms);
            sqlParameters = SqlConstructor.FilterParmsWithList(sqlParameters, CommSqlText.SqlParms[sqlKey]);
            var result = DbHelper.ExecuteScalar(sqlContent, sqlParameters);
            return result;
        }

        public object Execute<T>(CommonSqlKey sqlKey, T t)
        {
            var sqlContent = CommSqlText.Instance[sqlKey];
            var sqlParameters = SqlConstructor.MakeParms(t);
            sqlParameters = SqlConstructor.FilterParmsWithList(sqlParameters, CommSqlText.SqlParms[sqlKey]);
            var result = DbHelper.ExecuteScalar(sqlContent, sqlParameters);
            return result;
        }

        public void Execute(List<string> sqlTxt)
        {
            foreach (var sql in sqlTxt)
            {
                DbHelper.ExecuteNonQuery(sql);
            }
        }

        public object Execute(CommonSqlKey sqlKey)
        {
            var sqlContent = CommSqlText.Instance[sqlKey];
            var result = DbHelper.ExecuteScalar(sqlContent);
            return result;
        }



        #region Utility

        private List<T> GetFromDictionaryByConditions<T>(CommonSqlKey sqlKey, IDictionary<string, object> parmDic, IList<Condition> conditions)
        {
            DbDataReader dr = null;
            var tLst = new List<T>();
            var logStep = 0;
            try
            {
                var sqlTxt = CommSqlText.Instance[sqlKey];

                logStep++;
                var sqlParameters = parmDic;

                //解析where 后查询条件
                List<DbParameter> parameter;
                var whereSql = ConditionHandler.GetWhereSql(conditions.ToList(), out parameter, sqlKey);

                sqlTxt = sqlTxt + whereSql;

                logStep++;
                logStep++;
                dr = DbHelper.ExecuteReader(sqlTxt, sqlParameters);
                logStep = 100;
                if (dr == null)
                    return tLst;
                var hasCount = dr.Read();
                while (hasCount)
                {
                    logStep += 10;
                    var t = MakeMapToObject<T>(dr);
                    if (t != null)
                        tLst.Add(t);
                    logStep += 1;
                    hasCount = dr.Read();
                }
            }
            catch (MySqlException ee)
            {
                Logger.LogInfo(String.Format("GetFromDictionary ERROR STEP:{0}, EXCEPTION:{1}", logStep, ee.Message), 0, LogType.ERROR);
                throw ee;
            }
            finally
            {
                if (dr != null)
                {
                    dr.Close();
                    dr.Dispose();

                }
            }
            return tLst;
        }

        private List<T> GetFromDictionaryByConditionsWithOrder<T>(CommonSqlKey sqlKey, IDictionary<string, object> parmDic, IList<Condition> conditions, string orderField = "", string orderType = "")
        {
            DbDataReader dr = null;
            var tLst = new List<T>();
            var logStep = 0;
            try
            {
                var sqlTxt = CommSqlText.Instance[sqlKey];

                logStep++;
                var sqlParameters = parmDic;

                //解析where 后查询条件
                List<DbParameter> parameter;
                var whereSql = ConditionHandler.GetWhereSql(conditions.ToList(), out parameter,sqlKey, orderField, orderType);

                sqlTxt = sqlTxt + whereSql;

                logStep++;
                logStep++;
                dr = DbHelper.ExecuteReader(sqlTxt, sqlParameters);
                logStep = 100;
                if (dr == null)
                    return tLst;
                var hasCount = dr.Read();
                while (hasCount)
                {
                    logStep += 10;
                    var t = MakeMapToObject<T>(dr);
                    if (t != null)
                        tLst.Add(t);
                    logStep += 1;
                    hasCount = dr.Read();
                }
            }
            catch (MySqlException ee)
            {
                Logger.LogInfo(String.Format("GetFromDictionary ERROR STEP:{0}, EXCEPTION:{1}", logStep, ee.Message), 0, LogType.ERROR);
                throw ee;
            }
            finally
            {
                if (dr != null)
                {
                    dr.Close();
                    dr.Dispose();

                }
            }
            return tLst;
        }

        private static Dictionary<string, object> MakeMapToDictionary(IDataRecord dr)
        {
            var drDic = new Dictionary<string, object>();
            for (var i = 0; i < dr.FieldCount; i++)
            {
                var colName = dr.GetName(i).ToUpper().Replace("_", ""); //把读取的字段所有下划线去掉
                if (!drDic.ContainsKey(colName))
                    drDic.Add(colName, dr.GetValue(i));
            }
            return drDic;
        }

        private static T MakeMapToObject<T>(IDataRecord dr)
        {
            T result;
            if (typeof(T).IsValueType || typeof(T) == typeof(string))
            {
                var v = dr.GetValue(0);
                result = v == null ? default(T) : (T)v;
            }
            else
            {

                try
                {
                    var drDic = MakeMapToDictionary(dr);
                    result = (T)GetResult(typeof(T), drDic);
                }
                catch (Exception ee)
                {
                    throw new Exception(ee.Message);
                    //result = default(T);
                }
            }

            return result;
        }

        private static object GetResult(Type retType, Dictionary<string, object> drDic)
        {
            var result = Activator.CreateInstance(retType);
            var curKey = String.Empty;
            //            try
            //            {
            var pis = CommDbHelper.GetModelDataPropertyInfos(retType);
            foreach (var propertyInfo in pis)
            {
                if (propertyInfo != null)
                {
                    if (propertyInfo.PropertyType.IsSubclassOf(typeof(BaseModel))) //如果是模型类，则递归获取对象实例
                    {
                        var parms = new List<Object>
                            {
                                propertyInfo.PropertyType,
                                drDic
                            };
                        var obj = typeof(SqlGenerator).GetMethod("GetResult", BindingFlags.NonPublic | BindingFlags.Static).Invoke(new object(), parms.ToArray());
                        ReflectionHandler.PropertyFastSetValue(propertyInfo.PropertyInfo, result, obj);
                    }

                    curKey = (String.IsNullOrEmpty(propertyInfo.ColumnName)
                        ? propertyInfo.PropertyName
                        : propertyInfo.ColumnName).ToUpper().Replace("_", "");
                    if (!drDic.ContainsKey(curKey))
                    {
                        continue;
                    }
                    var value = drDic[curKey];

                    if (value != DBNull.Value)
                    {
                        if (propertyInfo.PropertyType == typeof(DateTime))
                        {
                            DateTime temp;
                            if (!DateTime.TryParse(value.ToString().Trim(), out temp))
                            {
                                value = temp.ToString(CultureInfo.InvariantCulture);
                            }
                        }

                        if (propertyInfo.PropertyType == typeof(int?))
                        {
                            ReflectionHandler.PropertyFastSetValue(propertyInfo.PropertyInfo, result, (int?)value);
                        }
                        else if (propertyInfo.PropertyType == typeof(float?))
                        {
                            ReflectionHandler.PropertyFastSetValue(propertyInfo.PropertyInfo, result, (float?)value);
                        }
                        else if (propertyInfo.PropertyType == typeof(byte[]))
                        {
                            ReflectionHandler.PropertyFastSetValue(propertyInfo.PropertyInfo, result, (byte[])value);
                        }
                        else if (propertyInfo.PropertyType.IsValueType || propertyInfo.PropertyType == typeof(string))
                        {
                            ReflectionHandler.PropertyFastSetValue(propertyInfo.PropertyInfo, result, Convert.ChangeType(value, propertyInfo.PropertyType));
                        }
                        else
                        {
                            //TODO 考虑转换json
                            // var mi = typeof(JsonHandler).GetMethod("GetObjectFromJson");
                            // var valObj = Activator.CreateInstance(propertyInfo.PropertyType);

                            //var gM = mi.MakeGenericMethod(propertyInfo.PropertyType);

                            //ReflectionHandler.Execute(gM, valObj, value.ToString());
                            ReflectionHandler.PropertyFastSetValue(propertyInfo.PropertyInfo, result, value.ToString());
                        }
                    }
                }
            }
            //            }
            //            catch (Exception ee)
            //            {
            //                throw new Exception("ERROR KEY:" + curKey + " MSG: " + ee.Message);
            //            }
            return result;
        }

        #endregion
    }
}
