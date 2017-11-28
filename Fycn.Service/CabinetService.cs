using Fycn.Interface;
using Fycn.Model.Common;
using Fycn.Model.Machine;
using Fycn.SqlDataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fycn.Service
{
    public class CabinetService : AbstractService, ICabinet
    {
        // 取货柜作字典
        public List<CabinetConfigModel> GetCabinetByMachineTypeId(string machineTypeId)
        {
            var conditions = new List<Condition>();
            conditions.Add(new Condition
            {
                LeftBrace = " AND ",
                ParamName = "MachineTypeId",
                DbColumnName = "b.machine_type_id",
                ParamValue = machineTypeId,
                Operation = ConditionOperate.Equal,
                RightBrace = "",
                Logic = ""
            });
            return GenerateDal.LoadByConditions<CabinetConfigModel>(CommonSqlKey.GetCabinetByMachineTypeId, conditions);
        }


        public List<CommonDic> GetCabinetByMachineId(string machineId)
        {
            var innerConditions = new List<Condition>();
            innerConditions.Add(new Condition
            {
                LeftBrace = " AND ",
                ParamName = "MachineId",
                DbColumnName = "c.machine_id",
                ParamValue = machineId,
                Operation = ConditionOperate.Equal,
                RightBrace = "",
                Logic = ""
            });
            return GenerateDal.LoadByConditions<CommonDic>(CommonSqlKey.GetCabinetByMachineId, innerConditions);
        }


        public int PostCabinetRelationData(MachineTypeAndCabinetModel cabinetConfigInfo)
        {
            try
            {
                GenerateDal.BeginTransaction();

                GenerateDal.Create(cabinetConfigInfo);
                GenerateDal.CommitTransaction();

                return 1;
            }
            catch (Exception e)
            {
                GenerateDal.RollBack();
                return 0;
            }
        }

        /// <summary>
        /// 删除用户
        /// </summary>
        /// <returns></returns>
        public int DeleteData(string id)
        {
            MachineTypeAndCabinetModel machineConfigInfo = new MachineTypeAndCabinetModel();
            machineConfigInfo.MachineTypeId = id;
            return GenerateDal.Delete<MachineTypeAndCabinetModel>(CommonSqlKey.DeleteMachineTypeAndCabinet, machineConfigInfo);
        }
    }
}
