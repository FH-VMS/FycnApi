using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Fycn.Utility
{
    public class CommDbTransaction
    {
        private static Dictionary<string, DbTransaction> TranPool = new Dictionary<string, DbTransaction>();
        private static Dictionary<string, bool> TranRunFlag = new Dictionary<string, bool>();

        public static DbTransaction CurTran
        {
            get
            {
                if (TranPool.ContainsKey(CurThreadId))
                {
                    return TranPool[CurThreadId];
                }
                return null;
            }
            set
            {
                if (value == null && TranPool.ContainsKey(CurThreadId))
                {
                    TranPool.Remove(CurThreadId);
                    TranRunFlag.Remove(CurThreadId);
                }
                else
                {
                    TranPool[CurThreadId] = value;
                }
            }
        }

        public static bool CurTranRun
        {
            get
            {
                if (TranRunFlag.ContainsKey(CurThreadId))
                {
                    return TranRunFlag[CurThreadId];
                }
                return false;
            }
            set { TranRunFlag[CurThreadId] = value; }
        }

        private static string CurThreadId
        {
            get { return Thread.CurrentThread.ManagedThreadId.ToString(); }
        }

        public static void BeginTran()
        {
            CurTranRun = true;
        }

        /// <summary>
        /// 如果没有事务，则直接返回链接，如果启动事务则判断是否创建事务，没有则创建。
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="getDbConnFunc"></param>
        /// <returns></returns>
        public static DbConnection AttachTranAbility(Func<DbConnection> getDbConnFunc)
        {

            DbConnection conn;
            if (CurTran != null)
            {
                conn = CurTran.Connection;
                CurTranRun = true;
            }
            else
            {
                conn = getDbConnFunc();
                if (conn.State != ConnectionState.Open)
                {
                    conn.Open();
                }
                CurTran = conn.BeginTransaction();
            }
            return conn;
        }

        public static void Commit()
        {
            if (CurTran != null)
                CurTran.Commit();
            Dispose();
        }

        public static void RollBack()
        {
            if (CurTran != null && CurTran.Connection != null)
            {
                CurTran.Rollback();
                Dispose();
            }
            CurTran = null;
        }

        public static void Dispose()
        {
            CurTranRun = false;
            if (CurTran == null) return;
            if (CurTran.Connection != null)
            {
                CurTran.Connection.Close();
                CurTran.Connection.Dispose();
            }
            CurTran.Dispose();
            CurTran = null;
        }
    }
}
