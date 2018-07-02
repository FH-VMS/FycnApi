using Fycn.Interface;
using Fycn.Model.Machine;
using Fycn.Model.Pay;
using Fycn.SqlDataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fycn.Utility;
using Fycn.PaymentLib.ali;
using Fycn.PaymentLib.wx;

namespace Fycn.Service
{
    public class PayConfigService : AbstractService, IBase<ConfigModel>, IPayConfig
    {
        public List<ConfigModel> GetAll(ConfigModel configInfo)
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
                DbColumnName = "client_id",
                ParamValue = clientIds,
                Operation = ConditionOperate.INWithNoPara,
                RightBrace = "",
                Logic = ""
            });

            conditions.AddRange(CreatePaginConditions(configInfo.PageIndex, configInfo.PageSize));

            return GenerateDal.LoadByConditions<ConfigModel>(CommonSqlKey.GetPayConfigList, conditions);
        }


        public int GetCount(ConfigModel configInfo)
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
                DbColumnName = "client_id",
                ParamValue = clientIds,
                Operation = ConditionOperate.INWithNoPara,
                RightBrace = "",
                Logic = ""
            });



            result = GenerateDal.CountByConditions(CommonSqlKey.GetPayConfigListCount, conditions);

            return result;
        }


        /// <summary>
        /// 新增/修改会员信息
        /// </summary>
        /// <param name="memberInfo"></param>
        /// <returns></returns>
        public int PostData(ConfigModel configInfo)
        {
            int result;

            string userClientId = HttpContextHandler.GetHeaderObj("UserClientId").ToString();
            if (string.IsNullOrEmpty(userClientId))
            {
                return 0;
            }
            if (string.IsNullOrEmpty(configInfo.ClientId))
            {
                configInfo.ClientId = userClientId;
            }

            //configInfo.WxSslcertPath = "cert/"+configInfo.WxMchId+"/apiclient_cert.p12";
            //configInfo.WxSslcertPassword = configInfo.WxMchId;
            configInfo.WxMchId = configInfo.WxMchId.Trim();
            result = GenerateDal.Create(configInfo);

            //操作日志
            OperationLogService operationService = new OperationLogService();
            operationService.PostData(new OperationLogModel() { Remark = configInfo.ClientId, OptContent = "添加支付配置" });


            return result;
        }

        /// <summary>
        /// 删除用户
        /// </summary>
        /// <returns></returns>
        public int DeleteData(string id)
        {
            ConfigModel configInfo = new ConfigModel();
            configInfo.Id = id;
            return GenerateDal.Delete<ConfigModel>(CommonSqlKey.DeletePayConfig, configInfo);
        }

        public int UpdateData(ConfigModel configInfo)
        {
            if (string.IsNullOrEmpty(configInfo.ClientId))
            {
                string userClientId = HttpContextHandler.GetHeaderObj("UserClientId").ToString();
                configInfo.ClientId = userClientId;
            }
            configInfo.WxMchId = configInfo.WxMchId.Trim();
            //操作日志
            OperationLogService operationService = new OperationLogService();
            operationService.PostData(new OperationLogModel() { Remark = configInfo.ClientId, OptContent = "更新支付配置" });
            //configInfo.WxSslcertPath = "cert/" + configInfo.WxMchId + "/apiclient_cert.p12";
            //configInfo.WxSslcertPassword = configInfo.WxMchId;
            return GenerateDal.Update(CommonSqlKey.UpdatePayConfig, configInfo);
        }

        public int UpdateWxCert(ConfigModel configInfo)
        {
            return GenerateDal.Update(CommonSqlKey.UpdateWxCert, configInfo);
        }

        public List<ConfigModel> GetWxConfigByMchId(string mchId)
        {
            var conditions = new List<Condition>();
            conditions.Add(new Condition
            {
                LeftBrace = " AND ",
                ParamName = "WxMchId",
                DbColumnName = "wx_mchid",
                ParamValue = mchId.Trim(),
                Operation = ConditionOperate.Equal,
                RightBrace = "",
                Logic = ""
            });



            return GenerateDal.LoadByConditions<ConfigModel>(CommonSqlKey.GetWxConfigByMchId, conditions);
        }
        

    }
}
