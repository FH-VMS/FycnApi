using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using Fycn.Interface;
using Fycn.Model.Android;
using Fycn.Model.Machine;
using Fycn.Model.Pay;
using Fycn.Model.Privilege;
using Fycn.Model.Product;
using Fycn.Model.Sys;
using Fycn.Model.Wechat;
using Fycn.PaymentLib;
using Fycn.PaymentLib.wx;
using Fycn.Service;
using Fycn.Utility;
using FycnApi.Base;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Payment.wx;

namespace FycnApi.Controllers
{
    //[Produces("application/json")]
    //[Route("api/Hi")]
    public class HiController : ApiBaseController
    {
        public ResultObj<List<AndroidProductModel>> GetProductByMachine(string machineId, string waresTypeId = "", int pageIndex = 1, int pageSize = 10)
        {
            if (string.IsNullOrEmpty(machineId))
            {
                return Content(new List<AndroidProductModel>());
            }
            //KeyJsonModel keyJsonInfo = AnalizeKey(k);
            // IProduct service = new ProductService();
            //List<ProductModel> products = service.GetAllProducts();
            //k = "ABC123456789";
            //机器运行情况

            /*
            DataTable dt = _IMachine.GetMachineByMachineId(k);
            if (dt == null || dt.Rows.Count == 0)
            {
                return Content(new List<ProductForMachineModel>(), ResultCode.Success, "机器不存在", new Pagination { });
            }
            //判断机器是否在线 时间大于十五分钟为离线
            if (string.IsNullOrEmpty(dt.Rows[0][0].ToString()))
            {
                return Content(new List<ProductForMachineModel>(), ResultCode.Success, "机器不在线", new Pagination { });
            }
            int intval = Convert.ToInt32(dt.Rows[0][0]);
            if (intval > 900)
            {
                return Content(new List<ProductForMachineModel>(), ResultCode.Success, "机器不在线", new Pagination { });
            }
            */
            /*
            if (!MachineHelper.IsOnline(machineId))
            {
                return Content(new List<ProductForMachineModel>(), ResultCode.Success, "机器不在线", new Pagination { });
            }
            */
            AndroidProductModel machineInfo = new AndroidProductModel();
            machineInfo.MachineId = machineId;
            machineInfo.PageIndex = pageIndex;
            machineInfo.PageSize = pageSize;
            IAndroid imachine = new AndroidService();
            var data = imachine.GetProductAndroid(machineInfo);
            //int totalcount = imachine.GetProductAndroidCount(machineInfo);

            //var pagination = new Pagination { PageSize = pageSize, PageIndex = pageIndex, StartIndex = 0, TotalRows = totalcount, TotalPage = 0 };

            return Content(data);
        }

        public ResultObj<List<AndroidProductTypeModel>> GetProductTypeByMachine(string machineId)
        {
            if (string.IsNullOrEmpty(machineId))
            {
                return Content(new List<AndroidProductTypeModel>());
            }

            IAndroid imachine = new AndroidService();
            var data = imachine.GetProductTypeByMachine(machineId);
            return Content(data);
        }

        public ResultObj<PayStateModel> WechatAuth(string machineId, string code, string retBack = "")
        {
            string clientId = string.Empty;
            //log.Info("mmmmmmmmmmmmmmmm:"+m);
            string url = string.Empty;
            //KeyJsonModel keyJsonInfo = PayHelper.AnalizeKey(k);
            IPay _ipay = new PayService();
            WxPayConfig payConfig = _ipay.GenerateConfigModelW(machineId);
            PayStateModel payState = new PayStateModel();
            if (string.IsNullOrEmpty(payConfig.APPID))
            {
                payState.RequestState = "2";
                payState.ProductJson = "";
                payState.RequestData = "";
                return Content(payState);

            }
            PayModel payInfo = new PayModel();
            //JsApi.payInfo = new PayModel();
            payInfo.k = clientId;
            JsApi jsApi = new JsApi();
            string backUrl = "/wechat.html?clientId=" + clientId;
            if (!string.IsNullOrEmpty(retBack))
            {
                backUrl = backUrl + HttpUtility.UrlDecode(retBack);
            }
            jsApi.GetOpenidAndAccessToken(code, payConfig, payInfo, backUrl, "snsapi_userinfo");

            if (string.IsNullOrEmpty(payInfo.openid))
            {
                payState.RequestState = "0";
                payState.ProductJson = "";
                payState.RequestData = payInfo.redirect_url;
                return Content(payState);
            }
            //根据code 返回access_token
            //string urlAcess = string.Format("https://api.weixin.qq.com/sns/oauth2/access_token?appid={0}&secret={1}&code={2}&grant_type=authorization_code", payConfig.APPID,payConfig.APPSECRET,code);
            //string jsonResult = HttpService.Get(urlAcess);
            //log.Info("access_token:" + jsonResult);

            //Dictionary<string,string> dicAcess = JsonHandler.GetObjectFromJson<Dictionary<string,string>>(jsonResult);
            string accessToken = payInfo.access_token;//dicAcess["access_token"];
            //取授权用户信息
            string urlUserInfo = string.Format("https://api.weixin.qq.com/sns/userinfo?access_token={0}&openid={1}&lang=zh_CN", accessToken, payInfo.openid);
            string jsonUserInfo = HttpService.Get(urlUserInfo);

            payState.RequestState = "1";
            payState.ProductJson = jsonUserInfo;
            payState.RequestData = "";
            IWechat iwechat = new WechatService();
            WechatMemberModel memberInfo = new WechatMemberModel();
            memberInfo.ClientId = clientId;
            memberInfo.OpenId = payInfo.openid;
            List<WechatMemberModel> lstMemberInfo = iwechat.IsExistMember(memberInfo);
            if (lstMemberInfo.Count == 0)
            {
                WechatMemberModel createMemberInfo = JsonHandler.GetObjectFromJson<WechatMemberModel>(jsonUserInfo);
                createMemberInfo.ClientId = clientId;
                iwechat.CreateMember(createMemberInfo);
            }
            //if(iwechat.IsExistMember(memberInfo))
            //log.Info("test");

            return Content(payState);
        }


        //微信支付
        public ResultObj<PayStateModel> PostDataW(string machineId, string openId, string privilegeIds="", string selfChosen="", [FromBody]List<ProductPayModel> lstProductPay=null)
        {
            string clientId = string.Empty;
            try
            {
                IPay _ipay = new PayService();
                //移动支付赋值
                WxPayConfig payConfig = _ipay.GenerateConfigModelW(machineId);
                payConfig.NOTIFY_URL = PathConfig.NotidyAddr + "/Hi/PostPayResultW"; //结果通知方法
                JsApi jsApi = new JsApi();
                PayModel payInfo = new PayModel();
                payInfo.openid = openId;
                //JsApi.payInfo = new PayModel();
                //生成code 根据code取微信支付的openid和access_token
                //jsApi.GetOpenidAndAccessToken(code, payConfig, payInfo, "/wechat.html#/pay?clientId=" + clientId, "");

                PayStateModel payState = new PayStateModel();


                //JSAPI支付预处理

                //string result = HttpService.Get(payInfo.redirect_url);
                //生成交易号
                payInfo.trade_no = new PayHelper().GeneraterTradeNo();
                payInfo.jsonProduct = clientId;
                //取商品信息


                decimal totalFee = 0;
                string productNames = string.Empty;
                List<ProductListModel> lstProduct = new List<ProductListModel>();
                IWechat _iwechat = new WechatService();
                string waresId = string.Empty;
                string waresGroupId = string.Empty;
                foreach (ProductPayModel productInfo in lstProductPay)
                {

                    waresId = waresId + productInfo.WaresId + ",";

                }

                lstProduct = _iwechat.GetProdcutAndGroupList(waresId.TrimEnd(','), waresGroupId.TrimEnd(','));
                //检查商品是否有套餐
                bool hasGroup = false;
                //遍历商品
                foreach (ProductListModel productInfo in lstProduct)
                {
                    var productPay = (from m in lstProductPay
                                      where m.WaresId == productInfo.WaresId
                                      select m).ToList<ProductPayModel>();
                    if (productPay.Count > 0)
                    {
                        if (productPay[0].IsGroup == 1)
                        {
                            hasGroup = true;
                        }
                        totalFee = totalFee + Convert.ToInt32(productPay[0].Number) * Convert.ToDecimal(productInfo.WaresDiscountUnitPrice == 0 ? productInfo.WaresUnitPrice : productInfo.WaresDiscountUnitPrice);
                        productNames = productNames + productInfo.WaresName + ",";
                        productPay[0].TradeNo = payInfo.trade_no;
                    }


                }


                payInfo.product_name = productNames.Length > 25 ? productNames.Substring(0, 25) : productNames;

                payState.ProductJson = JsonHandler.GetJsonStrFromObject(lstProductPay, false);
                /*******************优惠券信息**********************/
                PrivilegeMemberRelationModel privilegeInfo = new PrivilegeMemberRelationModel();
                privilegeInfo.ClientId = clientId;
                privilegeInfo.MemberId = openId;
                if (totalFee > 0.01M && !hasGroup)
                {
                    List<PrivilegeMemberRelationModel> lstPrivilege = new List<PrivilegeMemberRelationModel>();
                    if (string.IsNullOrEmpty(privilegeIds) && string.IsNullOrEmpty(selfChosen))
                    {
                        lstPrivilege = _iwechat.GetCanUsePrivilege(privilegeInfo, privilegeIds, ref totalFee, lstProductPay);
                    }
                    else if (!string.IsNullOrEmpty(privilegeIds))
                    {
                        lstPrivilege = _iwechat.GetChosenPrivilege(privilegeInfo, privilegeIds, ref totalFee, lstProductPay);
                    }



                    
                    if (lstPrivilege.Count > 0)
                    {
                        string[] lstStr = lstPrivilege.Select(m => m.Id).ToArray();
                        if (string.IsNullOrEmpty(privilegeIds))
                        {
                            payInfo.jsonProduct = payInfo.jsonProduct + "~" + string.Join(",", lstStr);
                        }
                        else
                        {
                            payInfo.jsonProduct = payInfo.jsonProduct + "~" + privilegeIds;
                        }

                        payState.PrivilegeJson = JsonHandler.GetJsonStrFromObject(lstPrivilege, false);
                    }
                }
                //string total_fee = "1";
                //检测是否给当前页面传递了相关参数

                // 1.先取购买商品的详情返回给用户   并缓存到页面   2.支付成功后跳转到支付结果页并把缓存数据插入到销售记录表
                //若传递了相关参数，则调统一下单接口，获得后续相关接口的入口参数 

                // jsApiPay.openid = openid;
                /* 
                decimal privilegeMoney = 0;
                int weixinMoney = 0;
                if (lstPrivilege.Count > 0)
                {
                    if (lstPrivilege[0].Money>0)
                    {
                        privilegeMoney = lstPrivilege[0].Money;
                        weixinMoney = Convert.ToInt32(((totalFee - privilegeMoney) * 100));
                    } 
                    else
                    {
                        privilegeMoney = lstPrivilege[0].Discount;
                        weixinMoney = Convert.ToInt32(((totalFee) * 100)* (privilegeMoney/10));
                    }
                   
                } 
                else
                {
                    */
                int weixinMoney = Convert.ToInt32((totalFee) * 100);
                //}

                payInfo.total_fee = (weixinMoney <= 0 ? 1 : weixinMoney);
                //payInfo.jsonProduct = JsonHandler.GetJsonStrFromObject(keyJsonInfo, false);
                payState.TotalMoney = (totalFee <= 0 ? Convert.ToDecimal(0.01) : totalFee);
                //写入交易中转
                if (payInfo.total_fee == 0)
                {
                    payState.RequestState = "2";
                    payState.RequestData = "";
                    return Content(payState);
                }
                RedisHelper helper = new RedisHelper(0);

                helper.StringSet(payInfo.trade_no.Trim(), JsonHandler.GetJsonStrFromObject(lstProductPay, false), new TimeSpan(0, 10, 30));

                // FileHandler.WriteFile("data/", JsApi.payInfo.trade_no + ".wa", JsApi.payInfo.jsonProduct);

                WxPayData unifiedOrderResult = jsApi.GetUnifiedOrderResult(payInfo, payConfig);
                // Log.Write("GetDataW", "step step");
                string wxJsApiParam = jsApi.GetJsApiParameters(payConfig, payInfo);//获取H5调起JS API参数       
                payState.RequestState = "1";
                payState.RequestData = wxJsApiParam;



                return Content(payState);

            }
            catch (Exception ex)
            {
                PayStateModel payStateError = new PayStateModel();
                payStateError.RequestState = "3";
                payStateError.RequestData = ex.Message;
                return Content(payStateError);
            }
            return Content(new PayStateModel());
        }

        // 微信支付结果
        public string PostPayResultW()
        {
            try
            {
                var request = Fycn.Utility.HttpContext.Current.Request;
                int len = (int)request.ContentLength;
                byte[] b = new byte[len];
                Fycn.Utility.HttpContext.Current.Request.Body.Read(b, 0, len);
                string postStr = Encoding.UTF8.GetString(b);
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(postStr);
                // 商户交易号
                XmlNode tradeNoNode = xmlDoc.SelectSingleNode("xml/out_trade_no");

                RedisHelper helper = new RedisHelper(0);
                string retProducts = helper.StringGet(tradeNoNode.InnerText);
                if (string.IsNullOrEmpty(retProducts))
                {
                    return "<xml><return_code><![CDATA[FAIL]]></return_code></xml>";
                }

                /*
                IMachine _imachine = new MachineService();
                if (_imachine.GetCountByTradeNo(tradeNoNode.InnerText) > 0)
                {
                    return Content(1);
                }
                */


                //支付结果
                XmlNode payResultNode = xmlDoc.SelectSingleNode("xml/result_code");
                if (payResultNode.InnerText.ToUpper() == "SUCCESS")
                {
                    /*******************************放到微信支付通知参数里，因参数只支付最大128个字符长度，所以注释修改*****************************/
                    //XmlNode tunnelNode = xmlDoc.SelectSingleNode("xml/attach");
                    //KeyJsonModel keyJsonModel = JsonHandler.GetObjectFromJson<KeyJsonModel>(tunnelNode.InnerText);
                    XmlNode attachNode = xmlDoc.SelectSingleNode("xml/attach");
                    string jsonProduct = attachNode.InnerText;//helper.StringGet(tradeNoNode.InnerText);
                    XmlNode mchIdNode = xmlDoc.SelectSingleNode("xml/mch_id"); // 商户号
                    XmlNode openidNode = xmlDoc.SelectSingleNode("xml/openid"); //买家唯一标识
                    XmlNode isSubNode = xmlDoc.SelectSingleNode("xml/is_subscribe"); // 是否为公众号关注者
                    XmlNode timeEndNode = xmlDoc.SelectSingleNode("xml/time_end"); // 是否为公众号关注者
                                                                                   //string jsonProduct = FileHandler.ReadFile("data/" + tradeNoNode.InnerText + ".wa");
                                                                                   //log.Info("nnnnnnn" + tradeNoNode.InnerText);
                                                                                   //log.Info("aaaaaaa"+retProducts);
                    List<ProductPayModel> lstProductPay = JsonHandler.GetObjectFromJson<List<ProductPayModel>>(retProducts);
                    IWechat _iwechat = new WechatService();
                    int result = _iwechat.PostPayResultW(lstProductPay, mchIdNode.InnerText, openidNode.InnerText, isSubNode.InnerText, timeEndNode.InnerText, jsonProduct);
                    if (result > 0)
                    {
                        helper.KeyDelete(tradeNoNode.InnerText);
                    }

                }
                return "<xml><return_code><![CDATA[SUCCESS]]></return_code></xml>";
            }
            catch (Exception ex)
            {
                //log.Info("bbbb" + ex.Message);
                return "<xml><return_code><![CDATA[FAIL]]></return_code></xml>";
            }

            //File.WriteAllText(@"c:\text.txt", postStr); 
        }


        //检查该商品是否支付一元嗨
        public ResultObj<int> IsSupportActivity(string waresId)
        {
            return Content(0);
        }

        //根据机器id取机器位置
        public ResultObj<MachineLocationModel> GetMachineLocationById(string machineId)
        {
            if(string.IsNullOrEmpty(machineId))
            {
                return null;
            }
            IHi ihi = new HiService();
            var lstMachines = ihi.GetMachineLocationById(machineId);
            if (lstMachines.Count > 0)
            {
                return Content(lstMachines[0]);
            }
            else
            {
                return null;
            }
        }
    }
}