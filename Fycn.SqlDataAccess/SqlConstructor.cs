using Fycn.Model.Sys;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fycn.Utility;

namespace Fycn.SqlDataAccess
{
    public class SqlConstructor
    {

        #region sql Construct with parameters
        private const string pPre = "p_";

        /// <summary>
        /// 根据传入值和语句参数列表获得构造参数。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="sqlParmsList"></param>
        /// <returns></returns>
        public static IDictionary<string, object> MakeParms<T>(T t)
        {
            if (t == null) return null;
            Dictionary<string, object> retDic = null;
            if (t is IDictionary)
            {
                var dictionary = t as IDictionary<string, object>;
                if (dictionary == null) throw new CommDalException("Input parms type is not Dictionary<string,object>, pls check!, type is :" + t.GetType());
                retDic = dictionary.ToDictionary(r => (pPre + r.Key.Replace("p_", "")).ToUpper(), r => r.Value);
            }
            else if (t is IList)
            {
                var conditions = t as IList<Condition>;
                retDic = new Dictionary<string, object>();
                if (conditions == null) throw new CommDalException("Input parms type is not IList<Condition>, pls check!, type is :" + t.GetType());

                foreach (var condition in conditions)
                {
                    retDic.Add(pPre + condition.ParamName.ToUpper(), condition.ParamValue);
                }
            }
            else
            {
                retDic = MakeDbparmsWithObj(t);
            }
            return retDic;
        }
        /// <summary>
        /// 排序和过滤参数
        /// </summary>
        /// <param name="dbParms"></param>
        /// <param name="sqlParmsList"></param>
        /// <returns></returns>
        public static IDictionary<string, object> FilterParmsWithList(IDictionary<string, object> dbParms, IList<string> sqlParmsList)
        {
            if (sqlParmsList == null || sqlParmsList.Count <= 0) return dbParms;
            var newRet = new Dictionary<string, object>();

            for (var i = 0; i < sqlParmsList.Count; i++)
            {
                var key = sqlParmsList[i].ToUpper();
                if (!dbParms.ContainsKey(key))
                {
                    throw new Exception("Import parameter lost! this key is [" + key + "]");
                }
                var value = dbParms[key];
                object tmpValue = value;
                if (value is DateTime)
                {
                    if (value != null)
                    {
                        tmpValue = ((DateTime)value).ToString(DateTimeHandler.LongDateTimeStyleToOracleString);
                    }
                }
                //if (!newRet.ContainsKey(key))
                newRet.Add(key + i, tmpValue);
            }
            return newRet;
        }

        private static Dictionary<string, object> MakeDbparmsWithObj<T>(T t)
        {
            var pis = CommDbHelper.GetModelDataPropertyInfos(typeof(T));
            var dbparms = new Dictionary<string, object>();
            foreach (var pi in pis)
            {
                var v = pi.PropertyInfo.GetValue(t, new object[0]);
                dbparms.Add((pPre + pi.PropertyName).ToUpper(), v);
            }
            return dbparms;
        }

        #endregion

        #region Sqlstring Construct
        public static string GetSequeceIdSql<T>()
        {
            var pis = CommDbHelper.GetModelDataPropertyInfos(typeof(T));
            var pi = pis.Find(t => t.IsAuto);
            if (pi == null) return String.Empty;
            var sql = String.Format("SELECT LAST_INSERT_ID()");
            return sql;
        }
        /// <summary>
        /// 替换#语句和带有日期参数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sqlTemplete"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static string FilterSQLWithInsteadValue<T>(string sqlTemplete, T t)
        {
            if (t == null) return sqlTemplete;
            var parmDic = new Dictionary<string, object>();
            if (!typeof(IDictionary<string, object>).IsAssignableFrom(typeof(T)))
            {
                parmDic = GetObjDicFromT(t);
            }
            else
            {
                parmDic = t as Dictionary<string, object>;
            }
            sqlTemplete = FilterSQLWithReplaceDic(sqlTemplete, parmDic);
            return sqlTemplete;
        }


        private static Dictionary<string, object> GetObjDicFromT<T>(T t)
        {
            var dic = new Dictionary<string, object>();
            var pis = CommDbHelper.GetModelDataPropertyInfos(typeof(T));
            foreach (var modelPropertyInfo in pis)
            {
                var v = modelPropertyInfo.PropertyInfo.GetValue(t, new object[0]);
                dic.Add(pPre + modelPropertyInfo.PropertyName, v);
            }
            return dic;
        }


        /// <summary>
        /// 过滤sql语句查看替换字段
        /// </summary>
        /// <param name="key"></param>
        /// <param name="parms"></param>
        /// <returns></returns>
        private static string FilterSQLWithReplaceDic(string sqlTemplete, IDictionary<string, object> parms)
        {
            var sqlStr = sqlTemplete.ToUpper();
            if (sqlStr.IndexOf("#") >= 0)
            {
                foreach (var p in parms)
                {
                    //FilterIllegalText(p.Value.ToString());
                    sqlStr = sqlStr.Replace(("#" + p.Key).ToUpper(), p.Value.ToString());
                }
            }
            return sqlStr;
        }

        /// <summary>
        /// Make up a create sql string from obj
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public static string GetCreateSQL<T>(List<T> t)
        {
            var key = typeof(T).Namespace + "." + typeof(T).Name;

            var sqlTemp = CacheHandler<string>.GetObject(key, GetCreateSqlStrFromObj<T>);
            var sqlTempLst = sqlTemp.Split('|');
            var mt = new StringBuilder();
            for (var i = 0; i < t.Count; i++)
            {
                mt.Append(GetCreateSqlValue(String.Format("({0})", sqlTempLst[1]), t[i]));
                mt.Append(",");
            }

            return sqlTempLst[0] + mt.ToString().Trim(',');
        }

        protected static string GetCreateSqlStrFromObj<T>()
        {
            var type = typeof(T);
            var tableAttrs = type.GetCustomAttributes(typeof(TableAttribute), true);
            var tableName = type.Name;
            if (tableAttrs.Length > 0)
            {
                var tableAttr = tableAttrs[0] as TableAttribute;
                tableName = tableAttr == null ? type.Name : tableAttr.Name;
            }

            var columkey = new StringBuilder();
            var columkeyStr = String.Empty;
            var columval = new StringBuilder();
            var columvalStr = String.Empty;
            var propertis = CommDbHelper.GetModelDataPropertyInfos(typeof(T));
            var propertyList = propertis.FindAll(t => !String.IsNullOrEmpty(t.ColumnName));
            foreach (var propertyInfo in propertyList)
            {
                columkey.AppendFormat("{0},", propertyInfo.ColumnName);
                columval.AppendFormat("'{0}',", propertyInfo.PropertyName);
            }
            columkeyStr = columkey.ToString().TrimEnd(',');
            columvalStr = columval.ToString().TrimEnd(',');
            var startsql = String.Format("insert into {0}({1}) values", tableName, columkeyStr);
            return startsql + "|" + columvalStr;
        }

        /// <summary>
        /// Construct sql value with the single object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="templete"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        private static string GetCreateSqlValue<T>(string templete, T t)
        {
            var pis = CommDbHelper.GetModelDataPropertyInfos(typeof(T));
            templete = templete.ToLower();
            foreach (var p in pis)
            {
                var o = p.PropertyInfo.GetValue(t, new object[0]);
                var pureO = GetPureSQLValue(o, p.PropertyType);
                var k = "'" + p.PropertyName.ToLower() + "'";
                templete = templete.Replace(k, pureO);
            }
            return templete;
        }
        private static string GetPureSQLValue(object o, Type t)
        {
            if (o != null)
            {
                if (t.ToString() == "System.DateTime")
                {
                    o = DateTime.MinValue == (DateTime)o ? "null" : String.Format("FROM_UNIXTIME({0})", (Convert.ToDateTime(o).Ticks - DateTime.Parse("1970-01-01 08:00:00").Ticks) / 10000000);
                }
                else
                {
                    o = String.Format("'{0}'", o.ToString().Replace("'", "''"));
                }
                return o.ToString();
            }
            return "null";
        }
        public static string GetSelectSqlByParmDic<T>(IDictionary<string, object> parms)
        {
            var whereSql = new StringBuilder();
            if (parms != null)
            {
                foreach (var o in parms)
                {
                    whereSql.AppendFormat("{0}='{1}' and ", o.Key, o.Value);
                }
            }
            whereSql.Append("1=1");
            return String.Format("select * from {0} where {1}", GetTableName<T>(), whereSql);
        }

        public static string GetSelectPaginationSqlByParmDic<T>(IDictionary<string, object> parms)
        {
            var whereSql = new StringBuilder();
            var limitSql = new StringBuilder(" limit ");
            if (parms != null)
            {
                foreach (var o in parms)
                {
                    if (o.Key.Contains("P_INDEX"))
                    {
                        limitSql.AppendFormat("{0}", "?p_Index");
                    }
                    else if (o.Key.Contains("P_LENGTH"))
                    {
                        limitSql.AppendFormat(",{0}", "?p_Length");
                    }
                    else
                    {
                        whereSql.AppendFormat("{0} like {1} and ", GetAttributes<T>(o.Key), "?" + o.Key);
                    }

                }
            }
            whereSql.Append("1=1");
            //return $"select * from {GetTableName<T>()} where {whereSql} {limitSql}";
            return String.Format("select * from {0} where {1} {2}", GetTableName<T>(), whereSql, limitSql);
        }

        #endregion


        private static string GetTableName<T>()
        {
            var tableInfo = CommDbHelper.GetModelDataTableInfo(typeof(T));
            return tableInfo.TableName;
        }

        private static string GetAttributes<T>(string key)
        {
            var result = "";

            var tableInfo = CommDbHelper.GetModelDataTableInfo(typeof(T));
            foreach (var propertyInfo in tableInfo.AllPropertyInfos)
            {

                if (propertyInfo.PropertyName.ToUpper().Equals(key.Substring(key.IndexOf("_", StringComparison.Ordinal) + 1)))
                {
                    result = propertyInfo.ColumnName;
                    break;
                }
            }
            return result;
        }

        public static string GetSelectSqlByObj<T>(T t)
        {
            var tis = CommDbHelper.GetModelDataTableInfo(typeof(T));
            var sqlTxt = new StringBuilder("select * from ");
            sqlTxt.Append(tis.TableName);
            sqlTxt.Append(" where ");
            foreach (var modelPropertyInfo in tis.AllPropertyInfos)
            {
                var value = modelPropertyInfo.PropertyInfo.GetValue(t, null);
                if (modelPropertyInfo.DefaultValue != value)
                {
                    sqlTxt.AppendFormat("{0}={1} and ", modelPropertyInfo.ColumnName, GetPureSQLValue(value, modelPropertyInfo.PropertyType));
                }
            }
            sqlTxt.Append("1=1");
            return sqlTxt.ToString();
        }

        public static string GetDeleteSqlByParmDic<T>(object kv)
        {
            var whereSql = new StringBuilder();
            var tis = CommDbHelper.GetModelDataTableInfo(typeof(T));
            foreach (var keys in tis.KeysName)
            {
                if (keys.Contains('|')) continue;
                var pis = tis.AllPropertyInfos.Find(m => String.Equals(m.ColumnName, keys, StringComparison.CurrentCultureIgnoreCase) || String.Equals(m.PropertyName, keys, StringComparison.CurrentCultureIgnoreCase));
                if (pis == null) throw new Exception("ERROR DELETE KEY: " + keys + " IN TABLE: " + tis.TableName);
                whereSql.AppendFormat("{0}='{1}' and ", pis.ColumnName, kv);
            }
            whereSql.Append("1=1");
            return String.Format("delete from {0} where {1}", tis.TableName, whereSql);
        }

        public static string GetDeleteSqlByParmDic<T>(T t)
        {
            var whereSql = new StringBuilder();
            var tis = CommDbHelper.GetModelDataTableInfo(typeof(T));
            foreach (var keys in tis.KeysName)
            {
                var ks = keys.Split('|');
                foreach (var s in ks)
                {
                    var pis = tis.AllPropertyInfos.Find(m => String.Equals(m.ColumnName, s, StringComparison.CurrentCultureIgnoreCase) || String.Equals(m.PropertyName, s, StringComparison.CurrentCultureIgnoreCase));
                    if (pis == null) throw new Exception("ERROR DELETE KEY: " + s + " IN TABLE: " + tis.TableName);
                    var val = ReflectionHandler.PropertyFastGetValue(pis.PropertyInfo, t);
                    if (pis.PropertyType == typeof(DateTime))
                    {
                        whereSql.AppendFormat("{0}=to_date('{1}','{2}')", pis.ColumnName, ((DateTime)val).ToString(DateTimeHandler.LongDateTimeStyleToOracleString), DateTimeHandler.LongDateTimeStyleForOracle);
                    }
                    else
                    {
                        whereSql.AppendFormat("{0}='{1}' and ", pis.ColumnName, val);
                    }
                }
                whereSql.AppendFormat("1=1 or ");
            }
            whereSql.Append("1=0");
            return String.Format("delete from {0} where {1}", tis.TableName, whereSql);
        }
    }
}
