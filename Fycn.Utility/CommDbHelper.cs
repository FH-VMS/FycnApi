using Fycn.Model.Sys;
using MySql.Data.MySqlClient;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Fycn.Utility
{
    public class CommDbHelper
    {
        /// <summary>
        /// Class that manages all lower level ADO.NET data base access.
        /// 
        /// GoF Design Patterns: Singleton, Factory, Proxy.
        /// </summary>
        /// <remarks>
        /// This class is a 'swiss army knife' of data access. It handles all the 
        /// database access details and shields its complexity from its clients.
        /// 
        /// The Factory Design pattern is used to create database specific instances
        /// of Connection objects, Command objects, etc.

        //   initializers are thread safe.
        // If this class had a   constructor then these initialized 
        // would need to be initialized there.
        private readonly string _connectionString = ConfigHandler.ConnectionString;

        private readonly string _dbType = ConfigHandler.DataBaseType;

        private readonly ILogger _logger = LogFactory.GetInstance();

        #region Singleton instance

        private static Dictionary<int, DbConnection> conPool = new Dictionary<int, DbConnection>();

        private DbConnection CreateConnectionWithQuery()
        {
            DbConnection conn = null;
            switch (_dbType.ToUpper())
            {
                case "ORACLE":
                    {
                        conn = new OracleConnection(_connectionString);
                        break;
                    }
                case "SQLSERVER":
                    {
                        conn = new SqlConnection(_connectionString);
                        break;
                    }
                case "MYSQL":
                    {
                        conn = new MySqlConnection(_connectionString);
                        break;
                    }
            }
            return conn;
        }

        private DbConnection CreateConnection()
        {
            DbConnection conn = null;
            if (conPool.ContainsKey(Thread.CurrentThread.ManagedThreadId))
            {
                conn = conPool[Thread.CurrentThread.ManagedThreadId];
                if (conn != null)
                {
                    return conn;
                }
                conPool.Remove(Thread.CurrentThread.ManagedThreadId);
            }

            //_logger.LogInfo("Create Oracle Connection........");
            conn = CreateConnectionWithQuery();
            conPool.Add(Thread.CurrentThread.ManagedThreadId, conn);
            return conn;
        }
        /*
        private DbDataAdapter CreateAdapter()
        {
            switch (_dbType.ToUpper())
            {
                case "ORACLE":
                    {
                        return new OracleDataAdapter();
                    }
                case "SQLSERVER":
                    {
                        return new SqlDataAdapter();
                    }
                case "MYSQL":
                    {
                        return new MySqlDataAdapter();
                    }
                default:
                    return new OracleDataAdapter();
            }
        }
        */
        private void ClearPool(IDbConnection conn)
        {
            switch (_dbType.ToUpper())
            {
                case "ORACLE":
                    {
                        OracleConnection.ClearPool(conn as OracleConnection);
                        break;
                    }
                case "SQLSERVER":
                    {
                        SqlConnection.ClearPool(conn as SqlConnection);
                        break;
                    }
                case "MYSQL":
                    {
                        MySqlConnection.ClearPool(conn as MySqlConnection);
                        break;
                    }
            }
        }
        #endregion

        #region Create Transaction


        public static DbTransaction CurrentTran
        {
            get;
            set;
        }
        #endregion


        #region Create Command by DbProvider and Connection.

        //public DbConnection CommDbConnection { get; set; }

        //        public DbDataAdapter CommDataAdapter { get; set; }

        /// <summary>
        /// Create Command by DbProvider and Connection,then set values to it.
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="sql">Sql statement.</param>
        /// <param name="parameters">the parameters of the sql statement.</param>
        /// <returns>An instance of DbCommand object.</returns>
        public DbCommand CreateCommand(DbConnection connection, string sqlTxt, IDictionary<string, object> parameters)
        {
            return CreateCommand(connection, sqlTxt, CommandType.Text, parameters);
        }

        public DbCommand CreateCommand(DbConnection connection, string sqlTxt, CommandType cmdType, IDictionary<string, object> parms)
        {
            Open(connection);
            var command = connection.CreateCommand();
            try
            {
                command.CommandText = sqlTxt;
                command.CommandType = cmdType;
                if (CommDbTransaction.CurTran != null)
                {
                    command.Transaction = CommDbTransaction.CurTran;
                }
                if (parms == null) return command;

                foreach (var p in parms)
                {
                    var param = command.CreateParameter();
                    param.ParameterName = p.Key;
                    param.Value = p.Value;
                    command.Parameters.Add(param);
                }
            }
            catch (Exception ee)
            {
                Throw("CREATECOMMOND", ee, sqlTxt, parms);
            }
            return command;
        }

        /// <summary>
        /// Open connection
        /// </summary>
        /// <param name="connection"></param>
        public void Open(DbConnection connection)
        {
            if ((connection != null) && (connection.State != ConnectionState.Open))
            {
                connection.Open();
            }
        }

        /// <summary>
        /// Close the connection
        /// </summary>
        /// <param name="connection"></param>
        public void Close(IDbConnection connection, bool isReader)
        {
            if (CommDbTransaction.CurTranRun) return;
            if ((connection == null) || (connection.State == ConnectionState.Closed) || isReader) return;
            connection.Close();
            //ClearPool(connection);
            //connection.Dispose();
        }

        public void CloseAndDispose(IDbConnection connection)
        {
            if ((connection == null) || (connection.State == ConnectionState.Closed)) return;
            connection.Close();
            connection.Dispose();
        }

        #endregion

        #region Database Access Handlers

        #region ExecuteNonQuery(Execute sql statement in the database.)

        /// <summary>
        /// Execute sql statement in the database.
        /// </summary>
        /// <param name="sql">Sql statement.</param>
        /// <returns>Number of rows affected.</returns>
        public int ExecuteNonQuery(string sql)
        {
            return ExecuteNonQuery(sql, CommandType.Text, null);
        }

        /// <summary>
        /// Execute sql statement in the database.
        /// </summary>
        /// <param name="sql">Sql statement.</param>
        /// <param name="cmdType">The type of the sql statement.</param>
        /// <returns>Number of rows affected.</returns>
        public int ExecuteNonQuery(string sql, CommandType cmdType)
        {
            return ExecuteNonQuery(sql, cmdType, null);
        }

        /// <summary>
        /// Execute sql statement in the database.
        /// </summary>
        /// <param name="sql">Sql statement.</param>
        /// <param name="parameters">the parameters of the sql statement.</param>
        /// <returns>Number of rows affected.</returns>
        public int ExecuteNonQuery(string sql, IDictionary<string, object> parameters)
        {
            return ExecuteNonQuery(sql, CommandType.Text, parameters);
        }

        /// <summary>
        /// Execute sql statement in the database.
        /// </summary>
        /// <param name="sql">Sql statement.</param>
        /// <param name="cmdType">The type of the sql statement.</param>
        /// <param name="parameters">the parameters of the sql statement.</param>
        /// <returns>Number of rows affected.</returns>
        public int ExecuteNonQuery(string sql, CommandType cmdType, IDictionary<string, object> parameters)
        {
            var result = -1;
            DbConnection conn = null;
            DbCommand cmd = null;
            try
            {
                if (!CommDbTransaction.CurTranRun)
                {
                    conn = CreateConnection();
                }
                else
                {
                    conn = CommDbTransaction.AttachTranAbility(CreateConnection);
                }

                cmd = CreateCommand(conn, sql, cmdType, parameters);
                result = cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();
            }
            catch (Exception ee)
            {
                Throw("ExecuteNonQuery", ee, sql, parameters);
                CommDbTransaction.RollBack();
            }
            finally
            {
                if (cmd != null) cmd.Dispose();
                Close(conn, false);
            }
            return result;
        }

        #endregion

        #region ExecuteScalar (Executes a Sql statement and returns a scalar value.)

        /// <summary>
        /// Execute Sql statement and returns a scalar value.
        /// </summary>
        /// <param name="sql">Sql statement.</param>
        /// <returns>Scalar value.</returns>
        public object ExecuteScalar(string sql)
        {
            return ExecuteScalar(sql, CommandType.Text, null);
        }

        /// <summary>
        /// Executes a Sql statement and returns a scalar value.
        /// </summary>
        /// <param name="sql">Sql statement.</param>
        /// <param name="cmdType">The type of the sql statement.</param>
        /// <returns>Scalar value.</returns>
        public object ExecuteScalar(string sql, CommandType cmdType)
        {
            return ExecuteScalar(sql, cmdType, null);
        }

        /// <summary>
        /// Execute Sql statement and returns a scalar value.
        /// </summary>
        /// <param name="sql">Sql statement.</param>
        /// <param name="parameters">the parameters of the sql statement.</param>
        /// <returns>Scalar value.</returns>
        public object ExecuteScalar(string sql, IDictionary<string, object> parameters)
        {
            return ExecuteScalar(sql, CommandType.Text, parameters);
        }

        /// <summary>
        /// Execute Sql statement and returns a scalar value.
        /// </summary>
        /// <param name="sql">Sql statement.</param>
        /// <param name="cmdType">The type of the sql statement.</param>
        /// <param name="parameters">the parameters of the sql statement.</param>
        /// <returns>Scalar value.</returns>
        public object ExecuteScalar(string sql, CommandType cmdType, IDictionary<string, object> parameters)
        {
            object result = null;
            DbConnection conn = null;
            DbCommand cmd = null;
            try
            {
                conn = CreateConnectionWithQuery();
                cmd = CreateCommand(conn, sql, cmdType, parameters);
                result = cmd.ExecuteScalar();
                cmd.Parameters.Clear();
            }
            catch (Exception e)
            {
                Throw("EXECUTESCALAR", e, sql, parameters);
            }
            finally
            {
                if (cmd != null) cmd.Dispose();
                CloseAndDispose(conn);
            }
            return result;
        }

        #endregion

        #region ExecuteReader (Executes a Sql statement and return an instance of DataReader.)

        /// <summary>
        /// Execute Sql statement and return an instance of DataReader.
        /// </summary>
        /// <param name="sql">Sql statement.</param>
        /// <returns>An instance of DataReader.</returns>
        public DbDataReader ExecuteReader(string sql)
        {
            return ExecuteReader(sql, CommandType.Text, null);
        }

        /// <summary>
        /// Execute Sql statement and return an instance of DataReader.
        /// </summary>
        /// <param name="sql">Sql statement.</param>
        /// <param name="cmdType">The type of the sql statement.</param>
        /// <returns>An instance of DataReader.</returns>
        public DbDataReader ExecuteReader(string sql, CommandType cmdType)
        {

            return ExecuteReader(sql, cmdType, null);
        }

        /// <summary>
        /// Execute Sql statement and return an instance of DataReader.
        /// </summary>
        /// <param name="sql">Sql statement.</param>
        /// <param name="parameters">the parameters of the sql statement.</param>
        /// <returns>An instance of DataReader.</returns>
        public DbDataReader ExecuteReader(string sql, IDictionary<string, object> parameters)
        {
            return ExecuteReader(sql, CommandType.Text, parameters);
        }

        /// <summary>
        /// Execute Sql statement and return an instance of DataReader.
        /// </summary>
        /// <param name="sql">Sql statement.</param>
        /// <param name="cmdType">The type of the sql statement.</param>
        /// <param name="parameters">the parameters of the sql statement.</param>
        /// <returns>An instance of DataReader.</returns>
        public DbDataReader ExecuteReader(string sql, CommandType cmdType, IDictionary<string, object> parameters)
        {
            DbDataReader result = null;
            DbConnection connection = null;
            DbCommand cmd = null;
            var flag = Guid.NewGuid();
            try
            {
                connection = CreateConnectionWithQuery();
                cmd = CreateCommand(connection, sql, cmdType, parameters);
                result = cmd.ExecuteReader(CommandBehavior.CloseConnection);
            }
            catch (Exception e)
            {
                if (result != null && !result.IsClosed)
                {
                    result.Close();
                    result.Dispose();
                }
                Throw("EXECUTEREADER", e, sql, parameters);
            }
            finally
            {
                Close(connection, true);
            }
            return result;
        }

        #endregion

        #region ExecuteDataTable (Populates a DataTable according to a Sql statement.)

        /// Populate a DataTable according to a Sql statement.
        /// </summary>
        /// <param name="sql">Sql statement.</param>
        /// <returns>Populated DataTable.</returns>
        public DataTable ExecuteDataTable(string sql)
        {
            return ExecuteDataTable(sql, CommandType.Text, null);
        }

        /// Populate a DataTable according to a Sql statement.
        /// </summary>
        /// <param name="sql">Sql statement.</param>
        /// <param name="cmdType">The type of the sql statement.</param>
        /// <returns>Populated DataTable.</returns>
        public DataTable ExecuteDataTable(string sql, CommandType cmdType)
        {
            return ExecuteDataTable(sql, cmdType, null);
        }

        /// Populate a DataTable according to a Sql statement.
        /// </summary>
        /// <param name="sql">Sql statement.</param>
        /// <param name="parameters">the parameters of the sql statement.</param>
        /// <returns>Populated DataTable.</returns>
        public DataTable ExecuteDataTable(string sql, IDictionary<string, object> parameters)
        {
            return ExecuteDataTable(sql, CommandType.Text, parameters);
        }

        /// <summary>
        /// Populate a DataTable according to a Sql statement.
        /// </summary>
        /// <param name="sql">Sql statement.</param>
        /// <param name="cmdType">The type of the sql statement.</param>
        /// <param name="parameters">the parameters of the sql statement.</param>
        /// <returns>Populated DataTable.</returns>
        public DataTable ExecuteDataTable(string sql, CommandType cmdType, IDictionary<string, object> parameters)
        {
            DbConnection conn = null;
            DbCommand cmd = null;
            DbDataReader dataAdapter = null;
            var result = new DataTable();
            try
            {
                conn = CreateConnection();
                cmd = CreateCommand(conn, sql, cmdType, parameters);
                dataAdapter = cmd.ExecuteReader();
                ///添加表的数据  
                /*
                for (int i = 0; i < dataAdapter.FieldCount; i++)
                {
                    result.Columns.Add(new MicroDataColumn
                    {
                        ColumnName = dataAdapter.GetName(i),
                        ColumnType = dataAdapter.GetFieldType(i)
                    });
                    //myDataRow.Columns[dataAdapter.GetName(i)] = dataAdapter[i].ToString();
                }
                object[] values = new object[dataAdapter.FieldCount];
                while (dataAdapter.Read())
                {
                    dataAdapter.GetValues(values);
                    result.Rows.Add(new MicroDataRow(result.Columns, values));
                }
                */
                DataRow row;
                //DataTable test = new DataTable();
                for (int i = 0; i < dataAdapter.FieldCount; i++)
                {
                    result.Columns.Add(new DataColumn
                    {
                        ColumnName = dataAdapter.GetName(i),
                        DataType = dataAdapter.GetFieldType(i)
                    });
                    //myDataRow.Columns[dataAdapter.GetName(i)] = dataAdapter[i].ToString();
                }
                object[] values = new object[dataAdapter.FieldCount];
                while (dataAdapter.Read())
                {
                    //dataAdapter.GetValues(values);
                    //test.Rows.Add(new MicroDataRow(result.Columns, values));
                    row = result.NewRow();
                    for (int i = 0; i < dataAdapter.FieldCount; i++)
                    {
                        row[i] = dataAdapter[i];
                    }
                    result.Rows.Add(row);
                }
                //result.Load(dataAdapter);

                dataAdapter.Close();
                cmd.Parameters.Clear();
            }
            catch (Exception e)
            {
                Throw("EXECUTEDATATABLE", e, sql, parameters);
            }
            finally
            {
                if (cmd != null) cmd.Dispose();
                if (dataAdapter != null) dataAdapter.Dispose();
                Close(conn, false);
            }
            return result;

        }
        #endregion

        #endregion

        private void Throw(string methodName, Exception e, string sql, IDictionary<string, object> parameters)
        {
            var parms = String.Empty;
            if (parameters != null)
            {
                foreach (var parameter in parameters)
                {
                    parms += parameter.Key + "-" + parameter.Value;
                }
            }
            //_logger.LogInfo(String.Format("method:{0},Error:{1},SQL:{2},params:{3}", methodName, e.Message, sql, parms), 0, LogType.ERROR);
            var ee = new Exception(e.Message + ":" + sql);
            throw ee;
        }

        #region 获取用于数据ER mapping的模型属性字典

        public static ModelTableInfo GetModelDataTableInfo(Type tableType)
        {
            return CacheHandler<ModelTableInfo>.GetObject(tableType.ToString(), GetTableInfo, tableType);
        }

        private static ModelTableInfo GetTableInfo(Type tableType)
        {
            var tis = new ModelTableInfo();
            var tableAttr = tableType.GetCustomAttributes(typeof(TableAttribute), true);
            if (tableAttr.Length > 0)
            {
                var tAttr = tableAttr.GetValue(0) as TableAttribute;
                if (tAttr != null && !String.IsNullOrEmpty(tAttr.Key))
                {
                    tis.KeysName = new List<string>(tAttr.Key.Split(','));
                }
                tis.TableName = tAttr != null && !String.IsNullOrEmpty(tAttr.Name) ? tAttr.Name : tableType.Name;
            }
            tis.AllPropertyInfos = GetPropertyInfos(tableType);
            if (tis.KeysName != null && tis.KeysName.Count != 0) return tis;

            tis.KeysName = new List<string>();
            var primaryKey = tis.AllPropertyInfos.Find(t => t.IsPrimaryColumn);
            if (primaryKey != null)
                tis.KeysName.Add(primaryKey.PropertyName);
            return tis;
        }

        public static List<ModelPropertyInfo> GetModelDataPropertyInfos(Type t)
        {
            return GetModelDataTableInfo(t).AllPropertyInfos;
        }
        /// <summary>
        /// 用于获取模型的属性字典
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        private static List<ModelPropertyInfo> GetPropertyInfos(Type t)
        {
            var modelPropertyInfoList = new List<ModelPropertyInfo>();

            var pis = t.GetProperties();
            foreach (var propertyInfo in pis)
            {
                var modelPropertyInfo = new ModelPropertyInfo { PropertyName = propertyInfo.Name, PropertyType = propertyInfo.PropertyType, PropertyInfo = propertyInfo };
                modelPropertyInfo.DefaultValue = modelPropertyInfo.PropertyType.IsValueType ? Activator.CreateInstance(modelPropertyInfo.PropertyType) : null;//获取默认值
                var colummAttr = propertyInfo.GetCustomAttributes(typeof(ColumnAttribute), true);
                if (colummAttr.Length > 0)
                {
                    var colAttr = colummAttr.GetValue(0) as ColumnAttribute;
                    if (colAttr != null)
                    {
                        modelPropertyInfo.ColumnName = String.IsNullOrEmpty(colAttr.Name) ? modelPropertyInfo.PropertyName : colAttr.Name;
                        modelPropertyInfo.IsPrimaryColumn = colAttr.IsPrimaryKey;
                        modelPropertyInfo.IsAuto = colAttr.IsAuto;
                        modelPropertyInfo.IsNotNull = colAttr.IsNotNull;
                    }
                }

                modelPropertyInfoList.Add(modelPropertyInfo);
            }
            return modelPropertyInfoList;
        }
        #endregion
    }
}
