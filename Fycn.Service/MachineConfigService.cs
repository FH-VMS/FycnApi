using Fycn.Interface;
using Fycn.Model.Machine;
using Fycn.SqlDataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fycn.Utility;

namespace Fycn.Service
{
    public class MachineConfigService : AbstractService, IBase<MachineConfigModel>
    {

        public List<MachineConfigModel> GetAll(MachineConfigModel machineConfigInfo)
        {
            string userClientId = HttpContextHandler.GetHeaderObj("UserClientId").ToString();
            if (string.IsNullOrEmpty(userClientId))
            {
                return null;
            }
            var conditions = new List<Condition>();

            string clientIds = new CommonService().GetClientIds(userClientId.ToString());
            if (clientIds.Contains("self"))
            {
                clientIds = "'" + clientIds.Replace(",", "','") + "'";
            }
            conditions.Add(new Condition
            {
                LeftBrace = " AND ",
                ParamName = "ClientId",
                DbColumnName = "a.client_id",
                ParamValue = clientIds,
                Operation = ConditionOperate.INWithNoPara,
                RightBrace = " ",
                Logic = ""
            });
            if (!string.IsNullOrEmpty(machineConfigInfo.MachineId))
            {
                conditions.Add(new Condition
                {
                    LeftBrace = " AND ",
                    ParamName = "MachineId",
                    DbColumnName = "a.machine_id",
                    ParamValue = "%" + machineConfigInfo.MachineId + "%",
                    Operation = ConditionOperate.Like,
                    RightBrace = "",
                    Logic = ""
                });
            }
           
        
            conditions.AddRange(CreatePaginConditions(machineConfigInfo.PageIndex, machineConfigInfo.PageSize));

            return GenerateDal.LoadByConditions<MachineConfigModel>(CommonSqlKey.GetMachineConfig, conditions);
        }


        public int GetCount(MachineConfigModel machineConfigInfo)
        {
            var result = 0;

            string userClientId = HttpContextHandler.GetHeaderObj("UserClientId").ToString();
            if (string.IsNullOrEmpty(userClientId))
            {
                return 0;
            }
            var conditions = new List<Condition>();

            string clientIds = new CommonService().GetClientIds(userClientId);
            if (clientIds.Contains("self"))
            {
                clientIds = "'" + clientIds.Replace(",", "','") + "'";
            }
            conditions.Add(new Condition
            {
                LeftBrace = " AND ",
                ParamName = "ClientId",
                DbColumnName = "a.client_id",
                ParamValue = clientIds,
                Operation = ConditionOperate.INWithNoPara,
                RightBrace = " ",
                Logic = ""
            });
            if (!string.IsNullOrEmpty(machineConfigInfo.DeviceId))
            {
                conditions.Add(new Condition
                {
                    LeftBrace = " AND ",
                    ParamName = "DeviceId",
                    DbColumnName = "a.device_id",
                    ParamValue = "%" + machineConfigInfo.DeviceId + "%",
                    Operation = ConditionOperate.Like,
                    RightBrace = "",
                    Logic = ""
                });
            }



            result = GenerateDal.CountByConditions(CommonSqlKey.GetMachineConfigCount, conditions);

            return result;
        }


        /// <summary>
        /// 新增/修改会员信息
        /// </summary>
        /// <param name="memberInfo"></param>
        /// <returns></returns>
        public int PostData(MachineConfigModel machineConfigInfo)
        {
            try
            {
                GenerateDal.BeginTransaction();

                if (!string.IsNullOrEmpty(machineConfigInfo.MachineId))
                {
                    DeleteData(machineConfigInfo.MachineId);
                }
                string userAccount = HttpContextHandler.GetHeaderObj("UserAccount").ToString();
                machineConfigInfo.UpdateDate = DateTime.Now;
                machineConfigInfo.Updater = userAccount;
                GenerateDal.Create(machineConfigInfo);
                //需要插入机器下行表中待机器取
                MachineService mc = new MachineService();
                mc.PostToMachine(machineConfigInfo.MachineId, "t");
                GenerateDal.CommitTransaction();
                //操作日志
                OperationLogService operationService = new OperationLogService();
                operationService.PostData(new OperationLogModel() { MachineId = machineConfigInfo.MachineId, OptContent = "机器配置添加" });
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
            MachineConfigModel machineConfigInfo = new MachineConfigModel();
            machineConfigInfo.MachineId = id;
            //操作日志
            OperationLogService operationService = new OperationLogService();
            operationService.PostData(new OperationLogModel() { MachineId = machineConfigInfo.MachineId, OptContent = "机器配置删除" });
            return GenerateDal.Delete<MachineConfigModel>(CommonSqlKey.DeleteMachineConfig, machineConfigInfo);
        }

        public int UpdateData(MachineConfigModel machineConfigInfo)
        {
            machineConfigInfo.UpdateDate = DateTime.Now;
            string userAccount = HttpContextHandler.GetHeaderObj("UserAccount").ToString();
            machineConfigInfo.Updater = userAccount;
            //操作日志
            OperationLogService operationService = new OperationLogService();
            operationService.PostData(new OperationLogModel() { MachineId = machineConfigInfo.MachineId, OptContent = "机器配置更新" });
            return GenerateDal.Update(CommonSqlKey.UpdateMachineConfig, machineConfigInfo);
        }
    }
}
