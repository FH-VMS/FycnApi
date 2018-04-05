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
using log4net;
using Microsoft.AspNetCore.Mvc;

namespace FycnApi.Controllers
{
    public class RefundController : ApiBaseController
    {
        public ResultObj<int> PostRefund([FromBody]List<string> lstTradeNo)
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
                irefund.PostRefundA(aPayData.ToList<SaleModel>());
            }
            

            var wPayData = from m in lstSaleModel
                           where m.PayInterface == "微信"
                           select m;
            if (wPayData.ToList<SaleModel>().Count > 0)
            {
                irefund.PostRefundW(wPayData.ToList<SaleModel>());
            }
            return Content(1);
        }
       

        //退款通知
       
        public string PostRefundResultA()
        {
            try
            {
                SortedDictionary<string, string> sPara = GetRequestPost();
               
                if (sPara.Count > 0)//判断是否有带返回参数
                {
                    Notify aliNotify = new Notify();
                    var log = LogManager.GetLogger(Startup.repository.Name, typeof(Startup));
                    //log.Info("test");
                    log.Info(Fycn.Utility.HttpContext.Current.Request.Form["notify_id"]);
                    foreach(string key in Fycn.Utility.HttpContext.Current.Request.Form.Keys)
                    {
                        log.Info(key+":"+Fycn.Utility.HttpContext.Current.Request.Form[key]);
                    }
                    bool verifyResult = aliNotify.Verify(sPara, Fycn.Utility.HttpContext.Current.Request.Form["notify_id"], Fycn.Utility.HttpContext.Current.Request.Form["sign"]);
                    log.Info(verifyResult);
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

                        // Response.WriteAsync("success");  //请不要修改或删除
                        if (Convert.ToInt32(success_num) > 0)
                        {
                            string refundResult = result_details.Split('^')[result_details.Split('^').Length-1];
                            if (refundResult == "SUCCESS")
                            {
                                string tradeNo = result_details.Split('^')[0];
                                
                                IRefund irefund = new RefundService();
                                if (irefund.IsRefundSucceed(tradeNo) == 1)
                                {
                                    return "success";
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
                        return "fail";
                    }
                }
                return "success";
            }
            catch (Exception ex)
            {
                return "fail";
            }

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




       

    }
}