using Fycn.Interface;
using Fycn.Model.Common;
using Fycn.Model.Machine;
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
            if (string.IsNullOrEmpty(clientId))
            {
                return null;
            }
            var conditions = new List<Condition>();
            string clientIds = new CommonService().GetClientIds(clientId);
            if (clientIds.Contains("self"))
            {
                clientIds = "'" + clientIds.Replace(",", "','") + "'";
            }
            conditions.Add(new Condition
            {
                LeftBrace = " AND ",
                ParamName = "ClientId",
                DbColumnName = "client_id",
                ParamValue = clientIds,
                Operation = ConditionOperate.INWithNoPara,
                RightBrace = "",
                Logic = ""
            });
            if(!string.IsNullOrEmpty(commonDic.Name))
            {
                conditions.Add(new Condition
                {
                    LeftBrace = " AND ",
                    ParamName = "MachineId",
                    DbColumnName = " machine_id ",
                    ParamValue = "%" + commonDic.Name + "%",
                    Operation = ConditionOperate.Like,
                    RightBrace = "",
                    Logic = ""
                });

                conditions.Add(new Condition
                {
                    LeftBrace = " Or ",
                    ParamName = "Remark",
                    DbColumnName = " remark ",
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
            if (string.IsNullOrEmpty(clientId))
            {
                return 0;
            }
            var conditions = new List<Condition>();
            string clientIds = new CommonService().GetClientIds(clientId);
            if (clientIds.Contains("self"))
            {
                clientIds = "'" + clientIds.Replace(",", "','") + "'";
            }
            conditions.Add(new Condition
            {
                LeftBrace = " AND ",
                ParamName = "ClientId",
                DbColumnName = "client_id",
                ParamValue = clientIds,
                Operation = ConditionOperate.INWithNoPara,
                RightBrace = "",
                Logic = ""
            });
            if (!string.IsNullOrEmpty(commonDic.Name))
            {
                conditions.Add(new Condition
                {
                    LeftBrace = " AND ",
                    ParamName = "MachineId",
                    DbColumnName = "machine_id ",
                    ParamValue = "%" + commonDic.Name + "%",
                    Operation = ConditionOperate.Like,
                    RightBrace = "",
                    Logic = ""
                });

                conditions.Add(new Condition
                {
                    LeftBrace = " Or ",
                    ParamName = "Remark",
                    DbColumnName = " remark ",
                    ParamValue = "%" + commonDic.Name + "%",
                    Operation = ConditionOperate.Like,
                    RightBrace = "",
                    Logic = ""
                });
            }
            
            return GenerateDal.CountByConditions(CommonSqlKey.GetMachineDicCount, conditions);
        }

        public int CopyOneMachine(string oldMachineId, string newMachineId, List<string> copyItem,string machineName)
        {
            try
            {
                GenerateDal.BeginTransaction();
                var condition = new List<Condition>();
                condition.Add(new Condition
                {
                    LeftBrace = " AND ",
                    ParamName = "MachineId",
                    DbColumnName = "machine_id",
                    ParamValue = oldMachineId,
                    Operation = ConditionOperate.Equal,
                    RightBrace = "",
                    Logic = ""
                });

                var machineList = GenerateDal.LoadByConditions<MachineListModel>(CommonSqlKey.GetCopyMachineById, condition);
                if (machineList.Count == 0)
                {
                    return 0;
                }
                var newMachineInfo = machineList[0];
                newMachineInfo.MachineId = newMachineId;
                newMachineInfo.DeviceId = newMachineId;
                newMachineInfo.Remark = string.IsNullOrEmpty(machineName)? machineList[0].Remark + "-复制":machineName;// machineList[0].Remark + "-复制";
                newMachineInfo.CreateDate = DateTime.Now;
                newMachineInfo.LatestDate = null;
                newMachineInfo.IpV4 = "";
                GenerateDal.Create(newMachineInfo);

               
                foreach (string item in copyItem)
                {
                    switch (item)
                    {
                        case "机器配置":
                            
                            var machineConfig = GenerateDal.LoadByConditions<MachineConfigModel>(CommonSqlKey.GetMachineConfigById, condition);
                            if (machineConfig.Count == 0)
                            {
                                
                            } else
                            {
                                var newMachineConfig = machineConfig[0];
                                newMachineConfig.MachineId = newMachineId;
                                GenerateDal.Create(newMachineConfig);
                            }
                           
                            break;
                        case "货道配置":
                            var tunnelConfig = GenerateDal.LoadByConditions<TunnelConfigModel>(CommonSqlKey.GetTunnelConfigById, condition);
                            if (tunnelConfig.Count == 0)
                            {

                            }
                            else
                            {
                               foreach(TunnelConfigModel config in tunnelConfig)
                                {
                                    config.MachineId = newMachineId;
                                    GenerateDal.Create(config);
                                }
                                
                            }

                            var tunnelStatus = GenerateDal.LoadByConditions<TunnelInfoModel>(CommonSqlKey.GetTunnelStatusById, condition);
                            if (tunnelStatus.Count == 0)
                            {

                            }
                            else
                            {
                                foreach (TunnelInfoModel tunnelInfo in tunnelStatus)
                                {
                                    tunnelInfo.MachineId = newMachineId;
                                    GenerateDal.Create(tunnelInfo);
                                }

                            }
                            break;
                    }
                }

                GenerateDal.CommitTransaction();

                return 1;
            }
            catch (Exception e)
            {
                GenerateDal.RollBack();
                return 0;
            }
        }
    }
}
