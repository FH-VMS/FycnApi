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

    public class MachineListService : AbstractService, IBase<MachineListModel>
    {

        public List<MachineListModel> GetAll(MachineListModel machineListInfo)
        {
            string userClientId = HttpContextHandler.GetHeaderObj("UserClientId").ToString();
            if (string.IsNullOrEmpty(userClientId))
            {
                return null;
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
            if (!string.IsNullOrEmpty(machineListInfo.DeviceId))
            {
                conditions.Add(new Condition{
                    LeftBrace = " AND ",
                    ParamName = "DeviceId",
                    DbColumnName = "a.device_id",
                    ParamValue = "%" + machineListInfo.DeviceId + "%",
                    Operation = ConditionOperate.Like,
                    RightBrace = "",
                    Logic = ""
                });
            }

            if (!string.IsNullOrEmpty(machineListInfo.ClientText))
            {
                conditions.Add(new Condition
                {
                    LeftBrace = " AND ",
                    ParamName = "ClientName",
                    DbColumnName = "b.client_name",
                    ParamValue = "%" + machineListInfo.ClientText + "%",
                    Operation = ConditionOperate.Like,
                    RightBrace = "",
                    Logic = ""
                });
            }

            if (!string.IsNullOrEmpty(machineListInfo.UserAccount))
            {
                conditions.Add(new Condition
                {
                    LeftBrace = " AND ",
                    ParamName = "UserAccount",
                    DbColumnName = "c.usr_account",
                    ParamValue = "%" + machineListInfo.UserAccount + "%",
                    Operation = ConditionOperate.Like,
                    RightBrace = "",
                    Logic = ""
                });
            }

            if (!string.IsNullOrEmpty(machineListInfo.TypeId))
            {
                conditions.Add(new Condition
                {
                    LeftBrace = " AND ",
                    ParamName = "TypeId",
                    DbColumnName = "a.type_id",
                    ParamValue = machineListInfo.TypeId,
                    Operation = ConditionOperate.Equal,
                    RightBrace = "",
                    Logic = ""
                });
            }

            conditions.Add(new Condition
            {
                LeftBrace = "  ",
                ParamName = "LatestOnline",
                DbColumnName = "LatestOnline",
                ParamValue = "asc",
                Operation = ConditionOperate.OrderBy,
                RightBrace = "",
                Logic = ""
            });
            
            conditions.AddRange(CreatePaginConditions(machineListInfo.PageIndex, machineListInfo.PageSize));

            return GenerateDal.LoadByConditions<MachineListModel>(CommonSqlKey.GetMachineList, conditions);
        }


        public int GetCount(MachineListModel machineListInfo)
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
            if (!string.IsNullOrEmpty(machineListInfo.DeviceId))
            {
                conditions.Add(new Condition
                {
                    LeftBrace = " AND ",
                    ParamName = "DeviceId",
                    DbColumnName = "a.device_id",
                    ParamValue = "%"+machineListInfo.DeviceId + "%",
                    Operation = ConditionOperate.Like,
                    RightBrace = "",
                    Logic = ""
                });
            }

            if (!string.IsNullOrEmpty(machineListInfo.ClientText))
            {
                conditions.Add(new Condition
                {
                    LeftBrace = " AND ",
                    ParamName = "ClientName",
                    DbColumnName = "b.client_name",
                    ParamValue = "%" + machineListInfo.ClientText+"%",
                    Operation = ConditionOperate.Like,
                    RightBrace = "",
                    Logic = ""
                });
            }

            if (!string.IsNullOrEmpty(machineListInfo.UserAccount))
            {
                conditions.Add(new Condition
                {
                    LeftBrace = " AND ",
                    ParamName = "UserAccount",
                    DbColumnName = "c.usr_account",
                    ParamValue = "%" + machineListInfo.UserAccount+"%",
                    Operation = ConditionOperate.Like,
                    RightBrace = "",
                    Logic = ""
                });
            }

            if (!string.IsNullOrEmpty(machineListInfo.TypeId))
            {
                conditions.Add(new Condition
                {
                    LeftBrace = " AND ",
                    ParamName = "TypeId",
                    DbColumnName = "a.type_id",
                    ParamValue = machineListInfo.TypeId,
                    Operation = ConditionOperate.Equal,
                    RightBrace = "",
                    Logic = ""
                });
            }


            result = GenerateDal.CountByConditions(CommonSqlKey.GetMachineListCount, conditions);

            return result;
        }


        /// <summary>
        /// 新增/修改会员信息
        /// </summary>
        /// <param name="memberInfo"></param>
        /// <returns></returns>
        public int PostData(MachineListModel machineListInfo)
        {
            int result;

            string userAccount = HttpContextHandler.GetHeaderObj("UserAccount").ToString();
            machineListInfo.MachineId = machineListInfo.DeviceId;
            machineListInfo.CreateDate = DateTime.Now;
            machineListInfo.Creator = userAccount;
            result = GenerateDal.Create(machineListInfo);

            //操作日志
            OperationLogService operationService = new OperationLogService();
            operationService.PostData(new OperationLogModel() { MachineId = machineListInfo.MachineId, OptContent = "添加机器" });


            return result;
        }

        /// <summary>
        /// 删除用户
        /// </summary>
        /// <returns></returns>
        public int DeleteData(string id)
        {
           
            try
            {
                GenerateDal.BeginTransaction();

                MachineListModel machineListInfo = new MachineListModel();
                machineListInfo.MachineId = id;
                GenerateDal.Delete<MachineListModel>(CommonSqlKey.DeleteMachineList, machineListInfo);
                MachineConfigService mcService = new MachineConfigService();
                mcService.DeleteData(id);
                TunnelConfigService tcService = new TunnelConfigService();
                tcService.DeleteData(id);
                TunnelInfoService tiService = new TunnelInfoService();
                tiService.DeleteData(id);
                GenerateDal.CommitTransaction();
                //操作日志
                OperationLogService operationService = new OperationLogService();
                operationService.PostData(new OperationLogModel() { MachineId = machineListInfo.MachineId, OptContent = "删除机器" });
                return 1;
            }
            catch (Exception e)
            {
                GenerateDal.RollBack();
                return 0;
            }
        }

        public int UpdateData(MachineListModel machineListInfo)
        {
            machineListInfo.UpdateDate = DateTime.Now;
             string userAccount = HttpContextHandler.GetHeaderObj("UserAccount").ToString();
             machineListInfo.Updater = userAccount;
             //操作日志
             OperationLogService operationService = new OperationLogService();
             operationService.PostData(new OperationLogModel() { MachineId = machineListInfo.MachineId, OptContent = "更新机器" });
            return GenerateDal.Update(CommonSqlKey.UpdateMachineList, machineListInfo);
        }
       
    }
}
