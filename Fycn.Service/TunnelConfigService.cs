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
    public class TunnelConfigService : AbstractService, ITunnel
    {

        public List<TunnelConfigModel> GetAll(TunnelConfigModel tunnelConfigInfo)
        {
            int result = GetCount(tunnelConfigInfo);
            if (result > 0)
            {
                var conditions = new List<Condition>();

                if (!string.IsNullOrEmpty(tunnelConfigInfo.MachineId))
                {
                    conditions.Add(new Condition
                    {
                        LeftBrace = " AND ",
                        ParamName = "MachineId",
                        DbColumnName = "machine_id",
                        ParamValue = tunnelConfigInfo.MachineId,
                        Operation = ConditionOperate.Equal,
                        RightBrace = "",
                        Logic = ""
                    });
                }

                if (!string.IsNullOrEmpty(tunnelConfigInfo.CabinetId))
                {
                    conditions.Add(new Condition
                    {
                        LeftBrace = " AND ",
                        ParamName = "CabinetId",
                        DbColumnName = "cabinet_id",
                        ParamValue = tunnelConfigInfo.CabinetId,
                        Operation = ConditionOperate.Equal,
                        RightBrace = "",
                        Logic = ""
                    });
                }

                return GenerateDal.LoadByConditionsWithOrder<TunnelConfigModel>(CommonSqlKey.GetTunnelConfig, conditions, "tunnel_id", "asc");
            }
            else
            {
                return GenerateTunnelConfig(tunnelConfigInfo.CabinetId, tunnelConfigInfo.MachineId);
            }

           
        }

        private List<TunnelConfigModel> GenerateTunnelConfig(string cabinetId, string machineId)
        {
            List<TunnelConfigModel> lstTunnelConfig = new List<TunnelConfigModel>();
            var conditions = new List<Condition>();

            if (!string.IsNullOrEmpty(cabinetId))
            {
                conditions.Add(new Condition
                {
                    LeftBrace = " AND ",
                    ParamName = "CabinetId",
                    DbColumnName = "cabinet_id",
                    ParamValue = cabinetId,
                    Operation = ConditionOperate.Equal,
                    RightBrace = "",
                    Logic = ""
                });
            }

            List<CabinetConfigModel> lstCabinetConfig = GenerateDal.LoadByConditions<CabinetConfigModel>(CommonSqlKey.GetCabinetConfig, conditions);
            if (lstCabinetConfig == null || lstCabinetConfig.Count==0)
            {
                return null;
            }
            int layerNumber = lstCabinetConfig[0].LayerNumber;
            string cabinetDispaly = lstCabinetConfig[0].CabinetDisplay;
            string goodsNumber = lstCabinetConfig[0].LayerGoodsNumber;
            string[] arrGoodsNumber = goodsNumber.Split(',');
            for (int i = 1; i <= layerNumber; i++)
            {
                for (int j = 1; j <= Convert.ToInt32(arrGoodsNumber[i-1]); j++)
                {
                    TunnelConfigModel tunnelConfigModel = new TunnelConfigModel();
                    tunnelConfigModel.TunnelPosition = i + "-" + j;
                    tunnelConfigModel.TunnelId = cabinetDispaly + (i < 10 ? "0" + i : i.ToString()) + (j < 10 ? "0" + j : j.ToString());
                    tunnelConfigModel.CabinetId = cabinetId;
                    tunnelConfigModel.MachineId = machineId;
                    lstTunnelConfig.Add(tunnelConfigModel);
                }
            }

            return lstTunnelConfig;

        }


        public int GetCount(TunnelConfigModel tunnelConfigInfo)
        {
            var result = 0;


            var conditions = new List<Condition>();

            if (!string.IsNullOrEmpty(tunnelConfigInfo.MachineId))
            {
                conditions.Add(new Condition
                {
                    LeftBrace = " AND ",
                    ParamName = "MachineId",
                    DbColumnName = "machine_id",
                    ParamValue = tunnelConfigInfo.MachineId,
                    Operation = ConditionOperate.Equal,
                    RightBrace = "",
                    Logic = ""
                });
            }

            if (!string.IsNullOrEmpty(tunnelConfigInfo.CabinetId))
            {
                conditions.Add(new Condition
                {
                    LeftBrace = " AND ",
                    ParamName = "CabinetId",
                    DbColumnName = "cabinet_id",
                    ParamValue = tunnelConfigInfo.CabinetId,
                    Operation = ConditionOperate.Equal,
                    RightBrace = "",
                    Logic = ""
                });
            }



            result = GenerateDal.CountByConditions(CommonSqlKey.GetTunnelConfigCount, conditions);

            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="memberInfo"></param>
        /// <returns></returns>
        public int PostData(List<TunnelConfigModel> lstTunnelConfigInfo)
        {
            try
            {
                GenerateDal.BeginTransaction();
                if (lstTunnelConfigInfo.Count > 0)
                {
                    GenerateDal.Delete<TunnelConfigModel>(CommonSqlKey.DeleteTunnelConfig, lstTunnelConfigInfo[0]);
                }
                foreach (TunnelConfigModel tunnelConfigInfo in lstTunnelConfigInfo)
                {
                    tunnelConfigInfo.IsUsed = 1;
                    GenerateDal.Create(tunnelConfigInfo);
                }
                //向机器下行表里插入数据
                if (lstTunnelConfigInfo.Count > 0)
                {
                    MachineService ms = new MachineService();
                    ms.PostToMachine(lstTunnelConfigInfo[0].MachineId,"p");
                    //操作日志
                    OperationLogService operationService = new OperationLogService();
                    operationService.PostData(new OperationLogModel() { MachineId = lstTunnelConfigInfo[0].MachineId, OptContent = "货道配置添加" });
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

        /// <summary>
        /// 删除用户
        /// </summary>
        /// <returns></returns>
        public int DeleteData(string id)
        {
            TunnelConfigModel tunnelConfigInfo = new TunnelConfigModel();
            tunnelConfigInfo.MachineId = id;
            //操作日志
            OperationLogService operationService = new OperationLogService();
            operationService.PostData(new OperationLogModel() { MachineId = id, OptContent = "货道配置删除" });
            return GenerateDal.Delete<TunnelConfigModel>(CommonSqlKey.DeleteTunnelConfig, tunnelConfigInfo);
        }

        public int UpdateData(TunnelConfigModel tunnelConfigInfo)
        {
            //操作日志
            OperationLogService operationService = new OperationLogService();
            operationService.PostData(new OperationLogModel() { MachineId = tunnelConfigInfo.MachineId, OptContent = "货道配置更新" });

            return GenerateDal.Update(CommonSqlKey.UpdateTunnelConfig, tunnelConfigInfo);
        }
    }
}
