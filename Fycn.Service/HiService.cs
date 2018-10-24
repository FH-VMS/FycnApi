using Fycn.Interface;
using Fycn.Model.Machine;
using Fycn.SqlDataAccess;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fycn.Service
{
    public class HiService : AbstractService, IHi
    {
        public List<MachineLocationModel> GetMachineLocationById(string machineId)
        {
            var conditions = new List<Condition>();
          
            conditions.Add(new Condition
            {
                LeftBrace = " AND ",
                ParamName = "MachineId",
                DbColumnName = "a.machine_id",
                ParamValue = machineId,
                Operation = ConditionOperate.Equal,
                RightBrace = " ",
                Logic = ""
            });

            return GenerateDal.LoadByConditions<MachineLocationModel>(CommonSqlKey.GetMachineLocationById, conditions);
        }
    }
}
