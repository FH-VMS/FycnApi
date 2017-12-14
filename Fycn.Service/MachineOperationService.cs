using Fycn.Interface;
using Fycn.Model.Common;
using Fycn.SqlDataAccess;
using Fycn.Utility;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fycn.Service
{
    public class MachineOperationService : AbstractService, IMachineOperation
    {
        public List<CommonDic> GetMachines(CommonDic commonDic,int pageIndex,int pageSize)
        {
            string clientId = HttpContextHandler.GetHeaderObj("UserClientId").ToString();
            var conditions = new List<Condition>();
            conditions.Add(new Condition
            {
                LeftBrace = " ",
                ParamName = "ClientId",
                DbColumnName = "",
                ParamValue = clientId,
                Operation = ConditionOperate.None,
                RightBrace = "",
                Logic = ""
            });
            if(!string.IsNullOrEmpty(commonDic.Name))
            {
                conditions.Add(new Condition
                {
                    LeftBrace = " AND ",
                    ParamName = "MachineId",
                    DbColumnName = " a.machine_id ",
                    ParamValue = "%" + commonDic.Name + "%",
                    Operation = ConditionOperate.Like,
                    RightBrace = "",
                    Logic = ""
                });

                conditions.Add(new Condition
                {
                    LeftBrace = " Or ",
                    ParamName = "Remark",
                    DbColumnName = " a.remark ",
                    ParamValue = "%" + commonDic.Name + "%",
                    Operation = ConditionOperate.Like,
                    RightBrace = "",
                    Logic = ""
                });
            }
            conditions.AddRange(CreatePaginConditions(pageIndex, pageSize));

            List<CommonDic> machines = GenerateDal.LoadByConditions<CommonDic>(CommonSqlKey.GetMachineDic, conditions);
            return machines;
        }

        public int GetMachinesCount(CommonDic commonDic, int pageIndex, int pageSize)
        {
            string clientId = HttpContextHandler.GetHeaderObj("UserClientId").ToString();
            var conditions = new List<Condition>();
            conditions.Add(new Condition
            {
                LeftBrace = " ",
                ParamName = "ClientId",
                DbColumnName = "",
                ParamValue = clientId,
                Operation = ConditionOperate.None,
                RightBrace = "",
                Logic = ""
            });
            if (!string.IsNullOrEmpty(commonDic.Name))
            {
                conditions.Add(new Condition
                {
                    LeftBrace = " AND ",
                    ParamName = "MachineId",
                    DbColumnName = " a.machine_id ",
                    ParamValue = "%" + commonDic.Name + "%",
                    Operation = ConditionOperate.Like,
                    RightBrace = "",
                    Logic = ""
                });

                conditions.Add(new Condition
                {
                    LeftBrace = " Or ",
                    ParamName = "Remark",
                    DbColumnName = " a.remark ",
                    ParamValue = "%" + commonDic.Name + "%",
                    Operation = ConditionOperate.Like,
                    RightBrace = "",
                    Logic = ""
                });
            }
            
            return GenerateDal.CountByConditions(CommonSqlKey.GetMachineDicCount, conditions);
        }
    }
}
