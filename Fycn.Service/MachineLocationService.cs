﻿using Fycn.Interface;
using Fycn.Model.Machine;
using Fycn.SqlDataAccess;
using Fycn.Utility;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fycn.Service
{
    public class MachineLocationService : AbstractService, IBase<MachineLocationModel>
    {
        public List<MachineLocationModel> GetAll(MachineLocationModel machineLocationInfo)
        {
            string userClientId = machineLocationInfo.ClientId;
            // string userClientId = HttpContextHandler.GetHeaderObj("UserClientId").ToString();
            if (string.IsNullOrEmpty(userClientId))
            {
                userClientId = HttpContextHandler.GetHeaderObj("UserClientId").ToString();
                // return null;
            }
            if(string.IsNullOrEmpty(userClientId))
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
                DbColumnName = "b.client_id",
                ParamValue = clientIds,
                Operation = ConditionOperate.INWithNoPara,
                RightBrace = " ",
                Logic = ""
            });
            if (!string.IsNullOrEmpty(machineLocationInfo.MachineId))
            {
                conditions.Add(new Condition
                {
                    LeftBrace = " AND ",
                    ParamName = "MachineId",
                    DbColumnName = "a.machine_id",
                    ParamValue = machineLocationInfo.MachineId,
                    Operation = ConditionOperate.Equal,
                    RightBrace = "",
                    Logic = ""
                });
            }

            if(string.IsNullOrEmpty(machineLocationInfo.MachineId) && string.IsNullOrEmpty(machineLocationInfo.StartLong))
            {
                return null;
            }

            if(!string.IsNullOrEmpty(machineLocationInfo.StartLong))
            {
                conditions.Add(new Condition
                {
                    LeftBrace = " AND ",
                    ParamName = "StartLong",
                    DbColumnName = "a.longitude",
                    ParamValue = machineLocationInfo.StartLong,
                    Operation = ConditionOperate.GreaterThan,
                    RightBrace = "",
                    Logic = ""
                });
            }

            if (!string.IsNullOrEmpty(machineLocationInfo.EndLong))
            {
                conditions.Add(new Condition
                {
                    LeftBrace = " AND ",
                    ParamName = "EndLong",
                    DbColumnName = "a.longitude",
                    ParamValue = machineLocationInfo.EndLong,
                    Operation = ConditionOperate.LessThan,
                    RightBrace = "",
                    Logic = ""
                });
            }

            if (!string.IsNullOrEmpty(machineLocationInfo.StartLati))
            {
                conditions.Add(new Condition
                {
                    LeftBrace = " AND ",
                    ParamName = "StartLati",
                    DbColumnName = "a.latitude",
                    ParamValue = machineLocationInfo.StartLati,
                    Operation = ConditionOperate.GreaterThan,
                    RightBrace = "",
                    Logic = ""
                });
            }

            if (!string.IsNullOrEmpty(machineLocationInfo.EndLati))
            {
                conditions.Add(new Condition
                {
                    LeftBrace = " AND ",
                    ParamName = "EndLati",
                    DbColumnName = "a.latitude",
                    ParamValue = machineLocationInfo.EndLati,
                    Operation = ConditionOperate.LessThan,
                    RightBrace = "",
                    Logic = ""
                });
            }


            // conditions.AddRange(CreatePaginConditions(machineLocationInfo.PageIndex, machineLocationInfo.PageSize));

            return GenerateDal.LoadByConditions<MachineLocationModel>(CommonSqlKey.GetMachineLocationList, conditions);
        }


        public int GetCount(MachineLocationModel machineLocationInfo)
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
                DbColumnName = "b.client_id",
                ParamValue = clientIds,
                Operation = ConditionOperate.INWithNoPara,
                RightBrace = " ",
                Logic = ""
            });
            if (!string.IsNullOrEmpty(machineLocationInfo.MachineId))
            {
                conditions.Add(new Condition
                {
                    LeftBrace = " AND ",
                    ParamName = "MachineId",
                    DbColumnName = "a.machine_id",
                    ParamValue = machineLocationInfo.MachineId,
                    Operation = ConditionOperate.Equal,
                    RightBrace = "",
                    Logic = ""
                });
            }


            result = GenerateDal.CountByConditions(CommonSqlKey.GetMachineLocationCount, conditions);

            return result;
        }


        /// <summary>
        /// 新增/修改会员信息
        /// </summary>
        /// <param name="memberInfo"></param>
        /// <returns></returns>
        public int PostData(MachineLocationModel machineLocationInfo)
        {
            int result;
            result = GenerateDal.Create(machineLocationInfo);

            return result;
        }

        /// <summary>
        /// 删除用户
        /// </summary>
        /// <returns></returns>
        public int DeleteData(string id)
        {
            MachineLocationModel machineLocationInfo = new MachineLocationModel();
            machineLocationInfo.MachineId = id;
            return GenerateDal.Delete<MachineLocationModel>(CommonSqlKey.DeleteMachineLocation, machineLocationInfo);
        }

        public int UpdateData(MachineLocationModel machineLocationInfo)
        {
            return GenerateDal.Update(CommonSqlKey.UpdateMachineLocation, machineLocationInfo);
        }
    }
}
