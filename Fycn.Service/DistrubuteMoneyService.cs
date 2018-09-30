using Alipay.AopSdk.Core;
using Alipay.AopSdk.Core.Request;
using Alipay.AopSdk.Core.Response;
using Fycn.Interface;
using Fycn.Model.AccountSystem;
using Fycn.Model.Sale;
using Fycn.PaymentLib;
using Fycn.PaymentLib.ali;
using Fycn.PaymentLib.wx;
using Fycn.SqlDataAccess;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Fycn.Service
{
    public class DistrubuteMoneyService : AbstractService
    {
        public void PostMoney(SaleModel saleInfo)
        {
            int position = Array.IndexOf(PathConfig.DistrubuteAccounts, saleInfo.MerchantId);
            if (position != -1)
            {
                //通过机器id取对应的分账账户
                if (string.IsNullOrEmpty(saleInfo.MachineId))
                {
                    return;
                }


                if (saleInfo.PayInterface == "微信")
                {
                    //微信分账
                    //WxTransfer(saleInfo);
                    AliTransfer(saleInfo);
                }
                else if (saleInfo.PayInterface == "支付宝")
                {
                    //支付宝分账
                    AliTransfer(saleInfo);
                }
            }
        }

        private int WxTransfer(SaleModel saleInfo)
        {
            var conditions = new List<Condition>();

            conditions.Add(new Condition
            {
                LeftBrace = " AND ",
                ParamName = "MachineId",
                DbColumnName = "a.machine_id",
                ParamValue = saleInfo.MachineId,
                Operation = ConditionOperate.Equal,
                RightBrace = "",
                Logic = ""
            });

            var lstAccounts = GenerateDal.LoadByConditions<AccountModel>(CommonSqlKey.GetAccountByMachineId, conditions);
            if (lstAccounts.Count == 0)
            {
                return 0;
            }
            AccountModel accountInfo = lstAccounts[0];
            PayService ps = new PayService();
            WxPayConfig payConfig = ps.GenerateConfigModelW(saleInfo.MachineId);
            JsApi jsApi = new JsApi();
            TransferModel transferInfo = new TransferModel();
            transferInfo.partner_trade_no = saleInfo.TradeNo;
            transferInfo.openid = accountInfo.UserOpenid;
            transferInfo.re_user_name = accountInfo.WxUserName;
            // transferInfo.amount
            //计算费率
            if (accountInfo.WxRate == 0)
            {
                transferInfo.amount = Convert.ToInt32((saleInfo.TradeAmount - saleInfo.ServiceCharge) * 100);
            }
            else
            {
                transferInfo.amount = Convert.ToInt32((saleInfo.TradeAmount - saleInfo.ServiceCharge) * (1 - accountInfo.WxRate) * 100);
            }
            transferInfo.desc = saleInfo.WaresName + "收款";
            try
            {
                WxPayData result = jsApi.GetTransferToPersonal(transferInfo, payConfig);

                TransferListModel tlInfo = new TransferListModel();
                tlInfo.Id = Guid.NewGuid().ToString();
                tlInfo.TradeNo = saleInfo.TradeNo;
                tlInfo.PayInterface = "微信";
                tlInfo.Amount = transferInfo.amount;
                tlInfo.FyRate = accountInfo.WxRate;
                tlInfo.MerchantId = saleInfo.MerchantId;
                tlInfo.ToId = accountInfo.UserOpenid;
                tlInfo.ReallyName = accountInfo.WxUserName;
                tlInfo.Desc = result.ToJson();
                if (result.GetValue("result_code").ToString().ToUpper() == "SUCCESS")
                {

                    tlInfo.TransferStatus = "1";
                }
                else
                {
                    tlInfo.TransferStatus = "0";
                }

                TransferListService transferListService = new TransferListService();
                transferListService.PostData(tlInfo);
            }
            catch (Exception exp)
            {
                Log.Write("wxtransfer", exp.Message);
                throw exp;
            }
            return 1;
        }



        private int AliTransfer(SaleModel saleInfo)
        {
            var conditions = new List<Condition>();

            conditions.Add(new Condition
            {
                LeftBrace = " AND ",
                ParamName = "MachineId",
                DbColumnName = "a.machine_id",
                ParamValue = saleInfo.MachineId,
                Operation = ConditionOperate.Equal,
                RightBrace = "",
                Logic = ""
            });

            var lstAccounts = GenerateDal.LoadByConditions<AccountModel>(CommonSqlKey.GetAccountByMachineId, conditions);
            if (lstAccounts.Count == 0)
            {
                return 0;
            }
            AccountModel accountInfo = lstAccounts[0];
            PayService ps = new PayService();
            Config config = ps.GenerateConfigModelA(saleInfo.MachineId);
            if (config.private_key.Length > 1000)
            {
                config.refund_sign_type = "RSA2";
            }
            else
            {
                config.refund_sign_type = "RSA";
            }
            DefaultAopClient client = new DefaultAopClient(config.new_gatewayUrl, config.refund_appid, config.private_key, "json", "1.0", config.refund_sign_type, config.rsa_sign, config.new_charset, false);


            Alipay.AopSdk.Core.Domain.AlipayFundTransToaccountTransferModel model = new Alipay.AopSdk.Core.Domain.AlipayFundTransToaccountTransferModel();

            //计算费率
            if (accountInfo.AliRate == 0)
            {
                model.Amount = (saleInfo.TradeAmount - saleInfo.ServiceCharge).ToString();
            }
            else
            {
                model.Amount = ((saleInfo.TradeAmount - saleInfo.ServiceCharge) * (1 - accountInfo.AliRate)).ToString();
            }

            model.OutBizNo = saleInfo.TradeNo;
            model.PayeeType = "ALIPAY_LOGONID";
            model.PayeeAccount = accountInfo.AliAccount;
            model.PayeeRealName = accountInfo.AliUserName;
            model.Remark = saleInfo.WaresName + "收款";
            AlipayFundTransToaccountTransferRequest request = new AlipayFundTransToaccountTransferRequest();
            request.SetBizModel(model);
            try
            {
                AlipayFundTransToaccountTransferResponse response = client.Execute(request);

                TransferListModel tlInfo = new TransferListModel();
                tlInfo.Id = Guid.NewGuid().ToString();
                tlInfo.TradeNo = saleInfo.TradeNo;
                tlInfo.PayInterface = saleInfo.PayInterface;
                tlInfo.Amount = float.Parse(model.Amount);
                tlInfo.FyRate = accountInfo.AliRate;
                tlInfo.MerchantId = saleInfo.MerchantId;
                tlInfo.ToId = accountInfo.AliAccount;
                tlInfo.ReallyName = accountInfo.AliUserName;
                tlInfo.Desc = response.Body;
                if (response.IsError)
                {
                    tlInfo.TransferStatus = "0";
                }
                else
                {
                    if (string.IsNullOrEmpty(response.OutBizNo))
                    {
                        tlInfo.TransferStatus = "0";
                    }
                    else
                    {
                        tlInfo.TransferStatus = "0";
                        tlInfo.PaymentNo = response.OrderId;
                    }
                }
                TransferListService transferListService = new TransferListService();
                transferListService.PostData(tlInfo);
            }
            catch (Exception exp)
            {
                Log.Write("zhifubaotransfer", exp.Message);
                throw exp;
            }
            return 1;
        }

       
    }
}
