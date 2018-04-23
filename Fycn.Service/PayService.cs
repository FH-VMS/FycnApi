using Fycn.Interface;
using Fycn.Model.Pay;
using Fycn.PaymentLib.ali;
using Fycn.PaymentLib.wx;
using Fycn.SqlDataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fycn.Service
{
    public class PayService : AbstractService, IPay
    {
        public List<ProductModel> GetProducInfo(string machineId,  List<KeyTunnelModel> lstTunnels)
        {
            var conditions = new List<Condition>();

            if (!string.IsNullOrEmpty(machineId))
            {
                conditions.Add(new Condition
                {
                    LeftBrace = " AND ",
                    ParamName = "MachineId",
                    DbColumnName = "a.machine_id",
                    ParamValue = machineId,
                    Operation = ConditionOperate.Equal,
                    RightBrace = "  ",
                    Logic = ""
                });
            }
            if(lstTunnels.Count>0) {
               for(int i=0;i<lstTunnels.Count;i++)
               {
                   if(i==0) {
                       if (lstTunnels.Count == 1)
                       {
                           conditions.Add(new Condition
                           {
                               LeftBrace = " AND (",
                               ParamName = "TunnelId" + i,
                               DbColumnName = "a.tunnel_id",
                               ParamValue = lstTunnels[i].tid,
                               Operation = ConditionOperate.Equal,
                               RightBrace = " )",
                               Logic = ""
                           });
                       }
                       else
                       {
                           conditions.Add(new Condition
                           {
                               LeftBrace = " AND (",
                               ParamName = "TunnelId" + i,
                               DbColumnName = "a.tunnel_id",
                               ParamValue = lstTunnels[i].tid,
                               Operation = ConditionOperate.Equal,
                               RightBrace = "",
                               Logic = ""
                           });
                       }
                        
                   } else if(i==lstTunnels.Count-1) {
                       conditions.Add(new Condition
                        {
                            LeftBrace = " OR ",
                            ParamName = "TunnelId" + i,
                            DbColumnName = "a.tunnel_id",
                            ParamValue = lstTunnels[i].tid,
                            Operation = ConditionOperate.Equal,
                            RightBrace = ")",
                            Logic = ""
                        });
                   } else {
                        conditions.Add(new Condition
                        {
                            LeftBrace = " OR ",
                            ParamName = "TunnelId" + i,
                            DbColumnName = "a.tunnel_id",
                            ParamValue = lstTunnels[i].tid,
                            Operation = ConditionOperate.Equal,
                            RightBrace = "",
                            Logic = ""
                        });
                   }
                   
                }
            }

            return GenerateDal.LoadByConditions<ProductModel>(CommonSqlKey.GetProductInfo, conditions);


        }


        public List<ProductModel> GetProducInfoByWaresId(string machineId, List<KeyTunnelModel> lstTunnels)
        {
            var conditions = new List<Condition>();

            if (!string.IsNullOrEmpty(machineId))
            {
                conditions.Add(new Condition
                {
                    LeftBrace = " AND ",
                    ParamName = "MachineId",
                    DbColumnName = "a.machine_id",
                    ParamValue = machineId,
                    Operation = ConditionOperate.Equal,
                    RightBrace = "  ",
                    Logic = ""
                });
            }
            if (lstTunnels.Count > 0)
            {
                for (int i = 0; i < lstTunnels.Count; i++)
                {
                    if (i == 0)
                    {
                        if (lstTunnels.Count == 1)
                        {
                            conditions.Add(new Condition
                            {
                                LeftBrace = " AND (",
                                ParamName = "WaresId" + i,
                                DbColumnName = "a.wares_id",
                                ParamValue = lstTunnels[i].tid,
                                Operation = ConditionOperate.Equal,
                                RightBrace = " )",
                                Logic = ""
                            });
                        }
                        else
                        {
                            conditions.Add(new Condition
                            {
                                LeftBrace = " AND (",
                                ParamName = "WaresId" + i,
                                DbColumnName = "a.wares_id",
                                ParamValue = lstTunnels[i].tid,
                                Operation = ConditionOperate.Equal,
                                RightBrace = "",
                                Logic = ""
                            });
                        }

                    }
                    else if (i == lstTunnels.Count - 1)
                    {
                        conditions.Add(new Condition
                        {
                            LeftBrace = " OR ",
                            ParamName = "WaresId" + i,
                            DbColumnName = "a.wares_id",
                            ParamValue = lstTunnels[i].tid,
                            Operation = ConditionOperate.Equal,
                            RightBrace = ")",
                            Logic = ""
                        });
                    }
                    else
                    {
                        conditions.Add(new Condition
                        {
                            LeftBrace = " OR ",
                            ParamName = "WaresId" + i,
                            DbColumnName = "a.wares_id",
                            ParamValue = lstTunnels[i].tid,
                            Operation = ConditionOperate.Equal,
                            RightBrace = "",
                            Logic = ""
                        });
                    }

                }
            }

            conditions.Add(new Condition
            {
                LeftBrace = "  ",
                ParamName = "",
                DbColumnName = "a.wares_id",
                ParamValue = "",
                Operation = ConditionOperate.GroupBy,
                RightBrace = "",
                Logic = ""
            });

            return GenerateDal.LoadByConditions<ProductModel>(CommonSqlKey.GetProductInfoByWaresId, conditions);


        }


        //取商品列表
        public List<ConfigModel> GetConfig(string machindId)
        {
            var conditions = new List<Condition>();

            if (string.IsNullOrEmpty(machindId))
            {
                return null;
            }

            conditions.Add(new Condition
            {
                LeftBrace = "  AND ",
                ParamName = "MachineId",
                DbColumnName = "b.machine_id",
                ParamValue = machindId,
                Operation = ConditionOperate.Equal,
                RightBrace = "",
                Logic = ""
            });

            return GenerateDal.LoadByConditions<ConfigModel>(CommonSqlKey.GetPayConfig, conditions);


        }


        public Config GenerateConfigModelA(string machineId)
        {
            Config aPayConfig = new Config();
            List<ConfigModel> lstConfig = GetConfig(machineId);
            if (lstConfig.Count > 0)
            {
                ConfigModel cModel = lstConfig[0];
                aPayConfig.partner = cModel.AliParter;
                aPayConfig.key = cModel.AliKey;
                aPayConfig.seller_id = cModel.AliParter;
                aPayConfig.refund_appid = cModel.AliRefundAppId;
                aPayConfig.rsa_sign = cModel.AliRefundRsaSign;

                //新支付宝接口
                aPayConfig.new_app_id = cModel.AliAppId;
                aPayConfig.private_key = cModel.AliPrivateKey;
                aPayConfig.alipay_public_key = cModel.AliPublicKey;
                if (aPayConfig.private_key.Length > 1000)
                {
                    aPayConfig.new_sign_type = "RSA2";
                }
                else
                {
                    aPayConfig.new_sign_type = "RSA";
                }

            }
            return aPayConfig;
        }

        public WxPayConfig GenerateConfigModelW(string machineId)
        {
            WxPayConfig payConfig = new WxPayConfig();
            List<ConfigModel> lstConfig = GetConfig(machineId);
            if (lstConfig.Count > 0)
            {
                ConfigModel cModel = lstConfig[0];

                payConfig.APPID = cModel.WxAppId;
                payConfig.MCHID = cModel.WxMchId;
                payConfig.KEY = cModel.WxKey;
                payConfig.APPSECRET = cModel.WxAppSecret;
                payConfig.SSLCERT_PATH = cModel.WxSslcertPath;
                payConfig.SSLCERT_PASSWORD = cModel.WxSslcertPassword;

            }
            return payConfig;
        }

        public WxPayConfig GenerateConfigModelWByClientId(string clientId)
        {
            var conditions = new List<Condition>();
            WxPayConfig payConfig = new WxPayConfig();
            if (string.IsNullOrEmpty(clientId))
            {
                return payConfig;
            }

            conditions.Add(new Condition
            {
                LeftBrace = "  AND ",
                ParamName = "ClientId",
                DbColumnName = "client_id",
                ParamValue = clientId,
                Operation = ConditionOperate.Equal,
                RightBrace = "",
                Logic = ""
            });

            List<ConfigModel> lstConfig = GenerateDal.LoadByConditions<ConfigModel>(CommonSqlKey.GetPayConfigByClientId, conditions);
            
           
            if (lstConfig.Count > 0)
            {
                ConfigModel cModel = lstConfig[0];

                payConfig.APPID = cModel.WxAppId;
                payConfig.MCHID = cModel.WxMchId;
                payConfig.KEY = cModel.WxKey;
                payConfig.APPSECRET = cModel.WxAppSecret;
                payConfig.SSLCERT_PATH = cModel.WxSslcertPath;
                payConfig.SSLCERT_PASSWORD = cModel.WxSslcertPassword;

            }
            return payConfig;
        }
    }
}
