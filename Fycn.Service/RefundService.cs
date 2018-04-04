using Alipay.AopSdk.Core;
using Alipay.AopSdk.Core.Request;
using Alipay.AopSdk.Core.Response;
using Fycn.Interface;
using Fycn.Model.Refund;
using Fycn.Model.Sale;
using Fycn.PaymentLib.ali;
using Fycn.PaymentLib.wx;
using Fycn.SqlDataAccess;
using Fycn.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fycn.Service
{
    public class RefundService : AbstractService, IRefund
    {
        public List<SaleModel> GetRefundOrder(List<string> lstTradeNo)
        {

            var conditions = new List<Condition>();


            conditions.Add(new Condition
            {
                LeftBrace = " AND ",
                ParamName = "TradeNo",
                DbColumnName = "trade_no",
                ParamValue = string.Join(",", lstTradeNo),
                Operation = ConditionOperate.IN,
                RightBrace = "  ",
                Logic = "AND"
            });

            conditions.Add(new Condition
            {
                LeftBrace = "  ( ",
                ParamName = "TradeStatus0",
                DbColumnName = "trade_status",
                ParamValue = 3,
                Operation = ConditionOperate.Equal,
                RightBrace = "  ",
                Logic = "OR"
            });

            conditions.Add(new Condition
            {
                LeftBrace = "  ",
                ParamName = "TradeStatus1",
                DbColumnName = "trade_status",
                ParamValue = 5,
                Operation = ConditionOperate.Equal,
                RightBrace = "  ",
                Logic = " OR "
            });

            conditions.Add(new Condition
            {
                LeftBrace = "  ",
                ParamName = "TradeStatus2",
                DbColumnName = "trade_status",
                ParamValue = 1,
                Operation = ConditionOperate.Equal,
                RightBrace = " ) ",
                Logic = ""
            });
            return GenerateDal.LoadByConditions<SaleModel>(CommonSqlKey.GetRefundData, conditions);
            //HttpContext.Current.Response.Write(sHtmlText);
        }

        public int UpdateRefundResult(SaleModel saleInfo)
        {
            return GenerateDal.Update(CommonSqlKey.UpdateRefundResult, saleInfo);
        }

        /// <summary>
        /// 新增/修改会员信息
        /// </summary>
        /// <param name="memberInfo"></param>
        /// <returns></returns>
        public int PostRefundDetail(RefundModel refundInfo)
        {
            int result=0;

            try
            {
                result = GenerateDal.Create(refundInfo);
            }
            catch (Exception e)
            {

            }

            return result;
        }

        public void UpdateOrderStatusForAli(string comId)
        {
            try
            {
                SaleModel saleInfo = new SaleModel();
                saleInfo.ComId = comId;
                GenerateDal.Execute(CommonSqlKey.UpdateOrderStatusForAli, saleInfo);
            }
            catch (Exception e)
            {

            }
           
        }

        //判断是否往退款表里插入成功
        public int IsRefundSucceed(string tradeNo)
        {
            var result = 0;

           
            var conditions = new List<Condition>();
            if (!string.IsNullOrEmpty(tradeNo))
            {
                conditions.Add(new Condition
                {
                    LeftBrace = " AND ",
                    ParamName = "TradeNo",
                    DbColumnName = "trade_no",
                    ParamValue = tradeNo,
                    Operation = ConditionOperate.Equal,
                    RightBrace = "",
                    Logic = ""
                });
                result = GenerateDal.CountByConditions(CommonSqlKey.IsRefundSucceed, conditions);
            }

            return result;
        }

        public int PostRefundA(List<SaleModel> lstSaleModel)
        {
             Log.Write("zhifubao", "9999");
            try
            {
                if (lstSaleModel.Count == 0)
                {
                    return 1;
                }
                
                PayService pay = new PayService();
                //移动支付配置赋值
                Config config = pay.GenerateConfigModelA(lstSaleModel[0].MachineId);
                /****************************旧支付宝退款接口*******************************/
                /*
                string detail_data = string.Empty;
                int batch_num = 1;
                foreach (SaleModel saleModel in lstSaleModel)
                {
                    if (saleModel.RealitySaleNumber == 0)
                    {
                        detail_data = detail_data + saleModel.ComId + "^" + saleModel.TradeAmount + "^出货失败退款" + "#";
                    }
                    else
                    {
                        detail_data = detail_data + saleModel.ComId + "^" + saleModel.TradeAmount * ((saleModel.SalesNumber - saleModel.RealitySaleNumber) / saleModel.SalesNumber) + "^出货失败退款" + "#";
                    }
                }
                if (!string.IsNullOrEmpty(detail_data))
                {

                    detail_data = detail_data.TrimEnd('#');
                    batch_num = detail_data.Split('#').Length;
                    //把请求参数打包成数组
                    SortedDictionary<string, string> sParaTemp = new SortedDictionary<string, string>();
                    sParaTemp.Add("service", config.refund_service);
                    sParaTemp.Add("partner", config.partner);
                    sParaTemp.Add("_input_charset", config.refund_input_charset.ToLower());
                    sParaTemp.Add("notify_url", config.refund_notify_url);
                    sParaTemp.Add("seller_user_id", config.seller_id);
                    sParaTemp.Add("refund_date", config.refund_date);
                    sParaTemp.Add("batch_no", PayHelper.GeneraterTradeNo());
                    sParaTemp.Add("batch_num", batch_num.ToString());//退款笔数，必填，参数detail_data的值中，“#”字符出现的数量加1，最大支持1000笔（即“#”字符出现的数量999个）
                    sParaTemp.Add("detail_data", detail_data);  //退款详细数据，必填，格式（支付宝交易号^退款金额^备注），多笔请用#隔开
                    //sParaTemp.Add("sign_type", Config.refund_sign_type);
                    //sParaTemp.Add("sign", Config.rsa_sign); 

                    //建立请求
                    try
                    {
                        string sHtmlText = config.GateWay + new Submit(config).BuildRequestParaToString(sParaTemp, Encoding.UTF8);
                        HttpHelper.CreateGetHttpResponse(sHtmlText, 2000, "", null);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }

                    //string sHtmlText = Submit.BuildRequest(sParaTemp, "get", "确认");
                    //HttpContext.Current.Response.Write(sHtmlText);
                }

                */
                /************************新支付宝退款接口****************************/

                DefaultAopClient client = new DefaultAopClient(config.new_gatewayUrl, config.refund_appid, config.private_key, "json", "1.0", config.refund_sign_type, config.rsa_sign, config.new_charset, false);
                
                foreach (SaleModel saleModel in lstSaleModel)
                {
                    Alipay.AopSdk.Core.Domain.AlipayTradeRefundModel model = new Alipay.AopSdk.Core.Domain.AlipayTradeRefundModel();
                    model.OutTradeNo = saleModel.TradeNo;
                    model.TradeNo = saleModel.ComId;
                    if (saleModel.RealitySaleNumber == 0)
                    {
                        model.RefundAmount = saleModel.TradeAmount.ToString();
                    }
                    else
                    {
                        model.RefundAmount = (saleModel.TradeAmount * ((saleModel.SalesNumber - saleModel.RealitySaleNumber) / saleModel.SalesNumber)).ToString();
                    }
                    if(saleModel.TradeStatus==1){
                        model.RefundReason = "待出货";
                    }
                    else if (saleModel.TradeStatus == 3)
                    {
                        model.RefundReason = "全部出货失败";
                    }
                    else if (saleModel.TradeStatus == 5)
                    {
                        model.RefundReason = "部分出货失败";
                    }

                    model.OutRequestNo = PayHelper.GeneraterTradeNo();

                    AlipayTradeRefundRequest request = new AlipayTradeRefundRequest();
                    request.SetNotifyUrl(config.refund_notify_url);
                    request.SetBizModel(model);

                    AlipayTradeRefundResponse response = null;
                    try
                    {
                        response = client.Execute(request);
                        if(string.IsNullOrEmpty(response.OutTradeNo))
                        {
                            return 0;
                        }
                    
                        UpdateOrderStatusForAli(response.TradeNo);

                        //插入退款信息表
                        RefundModel refundInfo = new RefundModel();
                        refundInfo.TradeNo = response.OutTradeNo;

                        refundInfo.RefundDetail = response.Body;
                        PostRefundDetail(refundInfo);
                        
                        //WIDresule.Text = response.Body;

                    }
                    catch (Exception exp)
                    {
                        Log.Write("zhifubao1",exp.Message);
                        throw exp;
                    }
                }
                 
                return 1;
            }
            catch (Exception ex)
            {
                return 0;
            }

        }


        /**********************************微信退款*******************************/
        public int PostRefundW(List<SaleModel> lstSaleModel)
        {
            try
            {

                if (lstSaleModel.Count == 0)
                {
                    return 1;
                }
                //移动支付配置赋值
                PayService pay = new PayService();
                WxPayConfig payConfig = pay.GenerateConfigModelW(lstSaleModel[0].MachineId);
                foreach (SaleModel saleModel in lstSaleModel)
                {
                    WxPayData data = new WxPayData();

                    data.SetValue("out_trade_no", saleModel.TradeNo);


                    data.SetValue("total_fee", int.Parse((saleModel.TradeAmount * 100).ToString()));//订单总金额
                    if (saleModel.RealitySaleNumber == 0)
                    {
                        data.SetValue("refund_fee", int.Parse((saleModel.TradeAmount * 100).ToString()));//退款金额
                    }
                    else
                    {
                        data.SetValue("refund_fee", int.Parse(((saleModel.TradeAmount * 100) * ((saleModel.SalesNumber - saleModel.RealitySaleNumber) / saleModel.SalesNumber)).ToString()));//退款金额
                    }

                    data.SetValue("out_refund_no", WxPayApi.GenerateOutTradeNo(payConfig));//随机生成商户退款单号
                    data.SetValue("op_user_id", payConfig.MCHID);//操作员，默认为商户号
                    //Log.Write("wwwww", "开始退款");
                    WxPayData result = WxPayApi.Refund(data, payConfig);//提交退款申请给API，接收返回数据
                    //更新销售状态
                    if (result.GetValue("result_code").ToString().ToUpper() == "SUCCESS")
                    {
                        SaleModel salInfo = new SaleModel();
                        salInfo.MachineId = saleModel.MachineId;
                        salInfo.GoodsId = saleModel.GoodsId;
                        salInfo.TradeNo = saleModel.TradeNo;
                        if (saleModel.RealitySaleNumber == 0)
                        {
                            salInfo.TradeStatus = 6;

                            //更新成6
                        }
                        else
                        {
                            //更新成3
                            salInfo.TradeStatus = 3;
                        }
                        UpdateRefundResult(salInfo);
                        RefundModel refundInfo = new RefundModel();
                        refundInfo.OutTradeNo = salInfo.TradeNo;
                        refundInfo.RefundDetail = result.ToJson();
                        PostRefundDetail(refundInfo);
                    }

                }



                return 1;
            }
            catch (Exception ex)
            {
                return 0;
            }

        }


    }
}
