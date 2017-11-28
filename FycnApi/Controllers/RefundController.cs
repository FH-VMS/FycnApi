using FycnApi.Base;
using Fycn.Interface;
using Fycn.Model.Pay;
using Fycn.Model.Refund;
using Fycn.Model.Sale;
using Fycn.Model.Sys;
using Newtonsoft.Json;
using Fycn.PaymentLib.ali;
using Fycn.PaymentLib.wx;
using Fycn.Service;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using Fycn.Utility;
using Microsoft.AspNetCore.Http;

namespace FycnApi.Controllers
{
    public class RefundController : ApiBaseController
    {
        public ResultObj<int> PostRefund(List<string> lstTradeNo)
        {
            if (lstTradeNo.Count == 0)
            {
                return Content(1);
            }
            IRefund irefund = new RefundService();
            List<SaleModel> lstSaleModel = irefund.GetRefundOrder(lstTradeNo);
            if (lstSaleModel.Count == 0)
            {
                return Content(1);
            }
            //支付宝
            var aPayData = from n in lstSaleModel
                           where n.PayInterface=="支付宝"
                           select n;
            if (aPayData.ToList<SaleModel>().Count > 0)
            {
                PostRefundA(aPayData.ToList<SaleModel>());
            }
            

            var wPayData = from m in lstSaleModel
                           where m.PayInterface == "微信"
                           select m;
            if (wPayData.ToList<SaleModel>().Count > 0)
            {
               
                PostRefundW(wPayData.ToList<SaleModel>());
            }
            return Content(1);
        }
        public ResultObj<int> PostRefundA(List<SaleModel> lstSaleModel)
        {

            try
            {
                if (lstSaleModel.Count == 0)
                {
                    return Content(1);
                }
                //移动支付配置赋值
                GenerateConfigModel("a", lstSaleModel[0].MachineId);
                /****************************旧支付宝退款接口*******************************/
             
                string detail_data = string.Empty;
                int batch_num = 1;
                foreach (SaleModel saleModel in lstSaleModel)
                {
                    if (saleModel.RealitySaleNumber==0)
                    {
                        detail_data = detail_data + saleModel.ComId + "^" + saleModel.TradeAmount + "^出货失败退款" + "#";
                    }
                    else
                    {
                        detail_data = detail_data + saleModel.ComId + "^" + saleModel.TradeAmount * ((saleModel.SalesNumber - saleModel.RealitySaleNumber) / saleModel.SalesNumber) + "^出货失败退款" + "#";
                    }
                }
                if(!string.IsNullOrEmpty(detail_data)) {
              
                    detail_data = detail_data.TrimEnd('#');
                    batch_num = detail_data.Split('#').Length;
                    //把请求参数打包成数组
                    SortedDictionary<string, string> sParaTemp = new SortedDictionary<string, string>();
                    sParaTemp.Add("service", Config.refund_service);
                    sParaTemp.Add("partner", Config.partner);
                    sParaTemp.Add("_input_charset", Config.refund_input_charset.ToLower());
                    sParaTemp.Add("notify_url", Config.refund_notify_url);
                    sParaTemp.Add("seller_user_id", Config.seller_id);
                    sParaTemp.Add("refund_date", Config.refund_date);
                    sParaTemp.Add("batch_no", GeneraterRefundNo());
                    sParaTemp.Add("batch_num", batch_num.ToString());//退款笔数，必填，参数detail_data的值中，“#”字符出现的数量加1，最大支持1000笔（即“#”字符出现的数量999个）
                    sParaTemp.Add("detail_data", detail_data);  //退款详细数据，必填，格式（支付宝交易号^退款金额^备注），多笔请用#隔开
                    //sParaTemp.Add("sign_type", Config.refund_sign_type);
                    //sParaTemp.Add("sign", Config.rsa_sign); 

                    //建立请求
                    try
                    {
                        string sHtmlText = Config.GateWay + Submit.BuildRequestParaToString(sParaTemp, Encoding.UTF8);
                        HttpHelper.CreateGetHttpResponse(sHtmlText, 2000, "", null);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                   
                    //string sHtmlText = Submit.BuildRequest(sParaTemp, "get", "确认");
                    //HttpContext.Current.Response.Write(sHtmlText);
                }
                
            
                /************************新支付宝退款接口****************************/
                /*
                DefaultAopClient client = new DefaultAopClient(Config.new_gatewayUrl, Config.refund_appid, Config.private_key, "json", "1.0", Config.refund_sign_type, Config.rsa_sign, Config.new_charset, false);
                foreach (SaleModel saleModel in lstSaleModel)
                {
                    AlipayTradeRefundModel model = new AlipayTradeRefundModel();
                    model.OutTradeNo = "";
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

                    model.OutRequestNo = GeneraterRefundNo();

                    AlipayTradeRefundRequest request = new AlipayTradeRefundRequest();
                    request.SetNotifyUrl(Config.refund_notify_url);
                    request.SetBizModel(model);

                    AlipayTradeRefundResponse response = null;
                    try
                    {
                        response = client.Execute(request);
                        //WIDresule.Text = response.Body;

                    }
                    catch (Exception exp)
                    {
                        throw exp;
                    }
                }
                 * */
                return Content(1);
            }
            catch (Exception ex)
            {
                return Content(0);
            }

        }

        //退款通知
        public ResultObj<int> PostRefundResultA()
        {
            try
            {
               
                SortedDictionary<string, string> sPara = GetRequestPost();
               
                if (sPara.Count > 0)//判断是否有带返回参数
                {
                    Notify aliNotify = new Notify();
                    bool verifyResult = aliNotify.Verify(sPara, Fycn.Utility.HttpContext.Current.Request.Form["notify_id"], Fycn.Utility.HttpContext.Current.Request.Form["sign"]);
                    if (verifyResult)//验证成功
                    {
                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////
                        //请在这里加上商户的业务逻辑程序代码


                        //——请根据您的业务逻辑来编写程序（以下代码仅作参考）——
                        //获取支付宝的通知返回参数，可参考技术文档中服务器异步通知参数列表

                        //批次号
                        string batch_no = Fycn.Utility.HttpContext.Current.Request.Form["batch_no"];

                        //批量退款数据中转账成功的笔数

                        string success_num = Fycn.Utility.HttpContext.Current.Request.Form["success_num"];

                        //批量退款数据中的详细信息
                        string result_details = Fycn.Utility.HttpContext.Current.Request.Form["result_details"];

                        //判断是否在商户网站中已经做过了这次通知返回的处理
                        //如果没有做过处理，那么执行商户的业务程序
                        //如果有做过处理，那么不执行商户的业务程序

                        Response.WriteAsync("success");  //请不要修改或删除
                        if (Convert.ToInt32(success_num) > 0)
                        {
                            string refundResult = result_details.Split('^')[result_details.Split('^').Length-1];
                            if (refundResult == "SUCCESS")
                            {
                                string tradeNo = result_details.Split('^')[0];
                                
                                IRefund irefund = new RefundService();
                                if (irefund.IsRefundSucceed(tradeNo) == 1)
                                {
                                    return Content(1);
                                }
                                irefund.UpdateOrderStatusForAli(tradeNo);

                                //插入退款信息表
                                RefundModel refundInfo = new RefundModel();
                                refundInfo.TradeNo = tradeNo;

                                refundInfo.RefundDetail = JsonConvert.SerializeObject(sPara);
                                irefund.PostRefundDetail(refundInfo);
                            }
                           
                        }

                        //——请根据您的业务逻辑来编写程序（以上代码仅作参考）——

                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    }
                    else//验证失败
                    {
                        Response.WriteAsync("fail");
                    }
                }
                return Content(1);
            }
            catch (Exception ex)
            {
                return Content(0);
            }

        }

        private string GeneraterRefundNo()
        {
            Random ran = new Random();
            int RandKey = ran.Next(1000, 9999);
            string out_trade_no = DateTime.Now.ToString("yyyyMMddhhmmssffff") + RandKey.ToString();
            return out_trade_no;
        }

        /// <summary>
        /// 获取支付宝POST过来通知消息，并以“参数名=参数值”的形式组成数组
        /// </summary>
        /// <returns>request回来的信息组成的数组</returns>
        public SortedDictionary<string, string> GetRequestPost()
        {
            int i = 0;
            SortedDictionary<string, string> sArray = new SortedDictionary<string, string>();
            NameValueCollection coll=new NameValueCollection();
            //Load Form variables into NameValueCollection variable.
            var forms = Fycn.Utility.HttpContext.Current.Request.Form;
            foreach (string key in forms.Keys)
            {
                coll.Add(key, forms[key]);
            }
            //coll = Fycn.Utility.HttpContext.Current.Request.Form;

            // Get names of all forms into a string array.
            String[] requestItem = coll.AllKeys;

            for (i = 0; i < requestItem.Length; i++)
            {
                sArray.Add(requestItem[i], Fycn.Utility.HttpContext.Current.Request.Form[requestItem[i]]);
            }

            return sArray;
        }




        /**********************************微信退款*******************************/
        public ResultObj<int> PostRefundW(List<SaleModel> lstSaleModel)
        {
            try
            {
                
                if (lstSaleModel.Count == 0)
                {
                    return Content(1);
                }
                //移动支付配置赋值
                GenerateConfigModel("w", lstSaleModel[0].MachineId);
                foreach (SaleModel saleModel in lstSaleModel)
                {
                    WxPayData data = new WxPayData();

                    data.SetValue("out_trade_no", saleModel.TradeNo);


                    data.SetValue("total_fee", int.Parse((saleModel.TradeAmount*100).ToString()));//订单总金额
                    if (saleModel.RealitySaleNumber == 0)
                    {
                        data.SetValue("refund_fee", int.Parse((saleModel.TradeAmount * 100).ToString()));//退款金额
                    }
                    else
                    {
                        data.SetValue("refund_fee", int.Parse(((saleModel.TradeAmount * 100)*((saleModel.SalesNumber - saleModel.RealitySaleNumber) / saleModel.SalesNumber)).ToString()));//退款金额
                    }
                    
                    data.SetValue("out_refund_no", WxPayApi.GenerateOutTradeNo());//随机生成商户退款单号
                    data.SetValue("op_user_id", WxPayConfig.MCHID);//操作员，默认为商户号
                    //Log.Write("wwwww", "开始退款");
                    WxPayData result = WxPayApi.Refund(data);//提交退款申请给API，接收返回数据
                    //更新销售状态
                    if (result.GetValue("result_code").ToString().ToUpper() == "SUCCESS")
                    {
                        
                        IRefund irefund = new RefundService();
                        SaleModel salInfo = new SaleModel();
                        salInfo.MachineId = saleModel.MachineId;
                        salInfo.GoodsId = saleModel.GoodsId;
                        salInfo.TradeNo = saleModel.TradeNo;
                        if (saleModel.RealitySaleNumber == 0)
                        {
                            salInfo.TradeStatus=6;
                            
                            //更新成6
                        }
                        else
                        {
                            //更新成3
                            salInfo.TradeStatus = 3;
                        }
                        irefund.UpdateRefundResult(salInfo);
                        RefundModel refundInfo = new RefundModel();
                        refundInfo.OutTradeNo = salInfo.TradeNo;
                        refundInfo.RefundDetail = result.ToJson();
                        irefund.PostRefundDetail(refundInfo);
                    }
                    
                }
              


                return Content(1);
            }
            catch (Exception ex)
            {
                return Content(0);
            }

        }

        public void GenerateConfigModel(string isAliOrWx, string machineId)
        {
            IPay ipay = new PayService();
            List<ConfigModel> lstConfig = ipay.GetConfig(machineId);
            if (lstConfig.Count > 0)
            {
                ConfigModel cModel = lstConfig[0];
                if (isAliOrWx == "a")
                {
                    Config.partner = cModel.AliParter;
                    Config.key = cModel.AliKey;
                    Config.seller_id = cModel.AliParter;
                    Config.refund_appid = cModel.AliRefundAppId;
                    Config.rsa_sign = cModel.AliRefundRsaSign;

                    //新支付宝接口
                    Config.new_app_id = cModel.AliAppId;
                    Config.private_key = cModel.AliPrivateKey;
                    Config.alipay_public_key = cModel.AliPublicKey;
                }
                else if (isAliOrWx == "w")
                {
                    WxPayConfig.APPID = cModel.WxAppId;
                    WxPayConfig.MCHID = cModel.WxMchId;
                    WxPayConfig.KEY = cModel.WxKey;
                    WxPayConfig.APPSECRET = cModel.WxAppSecret;
                    WxPayConfig.SSLCERT_PATH = cModel.WxSslcertPath;
                    WxPayConfig.SSLCERT_PASSWORD = cModel.WxSslcertPassword;
                }
            }
        }

    }
}