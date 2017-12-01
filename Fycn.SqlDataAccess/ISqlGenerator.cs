using Fycn.Model.Sys;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fycn.SqlDataAccess
{
    public interface ISqlGenerator
    {
        #region Tran

        void BeginTransaction();
        void CommitTransaction();
        void RollBack();

        #endregion

        #region Create

        /// <summary>
        /// 创建单个对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        int Create<T>(T t);

        /// <summary>
        /// 创建单个对象,并返回自增长Id
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        int CreateForAutoId<T>(T t);

        /// <summary>
        /// 创建多个对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        int Create<T>(List<T> t);


        /// <summary>
        /// 创建多个对象,并返回自增长Id列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        List<int> CreateForAutoId<T>(List<T> t);

        #endregion


        #region Read
        /// <summary>
        /// 根据主键值获取对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        T Get<T>(object value) where T : class;

        /// <summary>
        /// 根据传入对象自动生成参数获取完成对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        T Get<T>(T t) where T : class;


        /// <summary>
        /// 获取全部对象列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        List<T> Load<T>();

        /// <summary>
        /// 根据传入对象自动生成参数获取对象列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        List<T> Load<T>(T t);

        List<T> LoadByConditions<T>(CommonSqlKey sqlKey, IList<Condition> parmObj);
        List<T> LoadByConditionsWithOrder<T>(CommonSqlKey sqlKey, IList<Condition> parmObj, string orderField, string orderType);

        int CountByConditions(CommonSqlKey sqlKey, IList<Condition> parmObj);

        int CountByDictionary<T>(CommonSqlKey sqlKey, IDictionary<string, object> parmObj);

        /// <summary>
        /// 根据字典对象作为参数获取列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sqlKey"></param>
        /// <param name="parmObj"></param>
        /// <returns></returns>
        List<T> Load<T>(CommonSqlKey sqlKey, IDictionary<string, object> parmObj);

        /// <summary>
        /// 根据SQL获取单值
        /// </summary>
        /// <param name="sqlKey"></param>
        /// <returns></returns>
        object Single(CommonSqlKey sqlKey);

        /// <summary>
        /// 根据带值的类型对象作为参数获取列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sqlKey"></param>
        /// <param name="parmObj"></param>
        /// <returns></returns>
        object Single<T>(CommonSqlKey sqlKey, T parmObj);

        /// <summary>
        /// 根据字典对象作为参数获取列表
        /// </summary>
        /// <param name="sqlKey"></param>
        /// <param name="parmObj"></param>
        /// <returns></returns>
        object Single(CommonSqlKey sqlKey, IDictionary<string, object> parmObj);



        DataTable LoadDataTable(string sql);

        DataTable LoadDataTable(CommonSqlKey sqlKey, IDictionary<string, object> parmDic);

        DataTable LoadDataTableByConditions(CommonSqlKey sqlKey, IList<Condition> parmObj);

        #endregion

        #region Update
        /// <summary>
        /// 根据sql语句和类型对象参数获取列表对象更新数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sqlKey"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        int Update<T>(CommonSqlKey sqlKey, T t);
        #endregion


        #region Delete

        /// <summary>
        /// 全表清除
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        //int Truncate<T>();

        /// <summary>
        /// 根据单一主键进行删除操作
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="kv"></param>
        /// <returns></returns>
        int Delete<T>(object kv);

        /// <summary>
        /// 根据类型对象参数进行删除数据，用=和and连接,其中model需要设定key，用“，”连接各主键，用“|”区分组合主键内各字段
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        int Delete<T>(T t);

        /// <summary>
        /// 根据sql语句和类型对象删除数据
        /// </summary>
        /// <param name="sqlKey"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        int Delete<T>(CommonSqlKey sqlKey, T t);

        #endregion

        #region Execute
        /// <summary>
        /// 根据sql语句和类型对象参数执行语句
        /// </summary>
        /// <param name="sqlKey"></param>
        /// <param name="parms"></param>
        /// <returns></returns>
        object Execute(CommonSqlKey sqlKey, IDictionary<string, object> parms);
        /// <summary>
        /// 根据类型对象参数进行执行语句，用=和and连接
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sqlKey"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        object Execute<T>(CommonSqlKey sqlKey, T t);

        /// <summary>
        /// 执行sql语句
        /// </summary>
        /// <param name="sqlTxt"></param>
        /// <returns></returns>
        void Execute(List<string> sqlTxt);
        /// <summary>
        /// 执行sql语句
        /// </summary>
        /// <param name="sqlKey"></param>
        /// <returns></returns>
        object Execute(CommonSqlKey sqlKey);

        #endregion
    }
}
