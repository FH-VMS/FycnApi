using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using FycnApi.Base;
using Fycn.Model.Sys;
using Fycn.PaymentLib.wx;
using Fycn.Model.Pay;
using Fycn.Utility;
using Payment.wx;
using Fycn.Interface;
using Fycn.Service;
using log4net;
using Fycn.Model.Wechat;
using Fycn.Model.Product;
using System.Xml;
using System.Text;
using Fycn.PaymentLib;
using Fycn.Model.Sale;
using Fycn.Model.Privilege;

namespace FycnApi.Controllers
{
    //[Produces("application/json")]
    //[Route("api/Wechat")]
    public class WechatController : ApiBaseController
    {
        public ResultObj<PayStateModel> GetUrl(string m, string code)
        {

            //var log = LogManager.GetLogger("FycnApi", "wechat");
            //log.Info("mmmmmmmmmmmmmmmm:"+m);
            string url = string.Empty;
            //KeyJsonModel keyJsonInfo = PayHelper.AnalizeKey(k);
            IPay _ipay = new PayService();
            WxPayConfig payConfig = _ipay.GenerateConfigModelWByClientId(m);
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
            payInfo.k = m;
            JsApi jsApi = new JsApi();
            jsApi.GetOpenidAndAccessToken(code, payConfig, payInfo,"/wechat.html?clientId="+m, "snsapi_userinfo");
            
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
            string urlUserInfo = string.Format("https://api.weixin.qq.com/sns/userinfo?access_token={0}&openid={1}&lang=zh_CN",accessToken,payInfo.openid);
            string jsonUserInfo = HttpService.Get(urlUserInfo);

            payState.RequestState = "1";
            payState.ProductJson = jsonUserInfo;
            payState.RequestData = "";
            IWechat iwechat = new WechatService();
            WechatMemberModel memberInfo = new WechatMemberModel();
            memberInfo.ClientId = m;
            memberInfo.OpenId = payInfo.openid;
            List<WechatMemberModel> lstMemberInfo = iwechat.IsExistMember(memberInfo);
            if(lstMemberInfo.Count==0)
            {
                WechatMemberModel createMemberInfo = JsonHandler.GetObjectFromJson<WechatMemberModel>(jsonUserInfo);
                createMemberInfo.ClientId = m;
                iwechat.CreateMember(createMemberInfo);
            }
            //if(iwechat.IsExistMember(memberInfo))
            //log.Info("test");

            return Content(payState);
        }


        public ResultObj<List<ProductTypeModel>> GetProdcutTypeByClientId(string clientId)
        {
            IWechat iwechat = new WechatService();
            return Content(iwechat.GetProdcutTypeByClientId(clientId));
        }

        public ResultObj<List<ProductListModel>> GetProdcutByTypeAndClient(string typeId,string clientId="")
        {
            IWechat iwechat = new WechatService();
            return Content(iwechat.GetProdcutByTypeAndClient(typeId,clientId));
        }

        //微信支付
        public ResultObj<PayStateModel> PostDataW(string clientId,string openId,string privilegeIds, [FromBody]List<ProductPayModel> lstProductPay)
        {
            try
            {
                IPay _ipay = new PayService();
                //移动支付赋值
                WxPayConfig payConfig = _ipay.GenerateConfigModelWByClientId(clientId);
                payConfig.NOTIFY_URL= PathConfig.NotidyAddr + "/Wechat/PostPayResultW"; //结果通知方法
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
                foreach(ProductPayModel productInfo in lstProductPay)
                {
                   
                        waresId = waresId + productInfo.WaresId + ",";
                    
                }
                
                lstProduct = _iwechat.GetProdcutAndGroupList(waresId.TrimEnd(','),waresGroupId.TrimEnd(','));
                //遍历商品
                foreach (ProductListModel productInfo in lstProduct)
                {
                    var productPay = (from m in lstProductPay
                                      where m.WaresId == productInfo.WaresId
                                      select m).ToList<ProductPayModel>();
                    if (productPay.Count > 0)
                    {
                        totalFee = totalFee + Convert.ToInt32(productPay[0].Number) * Convert.ToDecimal(productInfo.WaresUnitPrice);
                        productNames = productNames + productInfo.WaresName + ",";
                        productPay[0].TradeNo = payInfo.trade_no;
                    }


                }


                payInfo.product_name = productNames.Length > 25 ? productNames.Substring(0, 25) : productNames;

                //string total_fee = "1";
                //检测是否给当前页面传递了相关参数

                // 1.先取购买商品的详情返回给用户   并缓存到页面   2.支付成功后跳转到支付结果页并把缓存数据插入到销售记录表
                //若传递了相关参数，则调统一下单接口，获得后续相关接口的入口参数 

                // jsApiPay.openid = openid;

                payInfo.total_fee = Convert.ToInt32((totalFee * 100));
                //payInfo.jsonProduct = JsonHandler.GetJsonStrFromObject(keyJsonInfo, false);

                //写入交易中转
               
                RedisHelper helper = new RedisHelper(0);
                
                helper.StringSet(payInfo.trade_no.Trim(), JsonHandler.GetJsonStrFromObject(lstProductPay, false), new TimeSpan(0, 10, 30));
                
                // FileHandler.WriteFile("data/", JsApi.payInfo.trade_no + ".wa", JsApi.payInfo.jsonProduct);

                WxPayData unifiedOrderResult = jsApi.GetUnifiedOrderResult(payInfo, payConfig);
                // Log.Write("GetDataW", "step step");
                string wxJsApiParam = jsApi.GetJsApiParameters(payConfig, payInfo);//获取H5调起JS API参数       
                payState.RequestState = "1";
                payState.ProductJson = JsonHandler.GetJsonStrFromObject(lstProductPay, false);
                payState.RequestData = wxJsApiParam;

                var log = LogManager.GetLogger("FycnApi", "wechat");
                log.Info("99999" + payInfo.trade_no);
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
            var log = LogManager.GetLogger("FycnApi", "wechat");
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
                    log.Info("nnnnnnn" + tradeNoNode.InnerText);
                    log.Info("aaaaaaa"+retProducts);
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
                log.Info("bbbb" + ex.Message);
                return "<xml><return_code><![CDATA[FAIL]]></return_code></xml>";
            }

            //File.WriteAllText(@"c:\text.txt", postStr); 
        }

        public ResultObj<List<SaleModel>> GetHistorySalesList(string openId, int pageIndex=0, int pageSize=15)
        {
            IWechat iwechat = new WechatService();
            return Content(iwechat.GetHistorySalesList(openId, pageIndex, pageSize));
        }

        public ResultObj<List<ClientSalesRelationModel>> GetWaitingSalesList(string openId)
        {
            IWechat iwechat = new WechatService();
            return Content(iwechat.GetWaitingSalesList(openId));
        }

        public ResultObj<WebSettingModel> GetWechatSetting(string clientId = "")
        {
            if (string.IsNullOrEmpty(clientId))
            {
                return null;
            }
            IBase<WebSettingModel> iwebSetting = new WebSettingService();
            WebSettingModel webSettingInfo = new WebSettingModel();
            webSettingInfo.ClientId = clientId;
            List<WebSettingModel> lstWebSetting = iwebSetting.GetAll(webSettingInfo);
            if (lstWebSetting.Count > 0)
            {
                lstWebSetting[0].StaticUrl = ConfigHandler.ResourceUrl;
            }
            return Content(lstWebSetting[0]);
        }

        //获取符合活动规则的券
        public ResultObj<List<PrivilegeModel>> GetActivityPrivilegeList(string clientId = "", string principleType="")
        {
            PrivilegeModel privilegeInfo=new PrivilegeModel();
            privilegeInfo.ClientId=clientId;
            privilegeInfo.PrincipleType=principleType;

            IWechat iwechat=new WechatService();
            return Content(iwechat.GetActivityPrivilegeList(privilegeInfo));
        }

        public ResultObj<int> GetTicket([FromBody]PrivilegeMemberRelationModel privilegeMemberInfo)
        {
            IWechat iwechat=new WechatService();
            int count=iwechat.IsExistTicket(privilegeMemberInfo);
            if(count>0)
            {
                return Content(0);
            }
            if(!string.IsNullOrEmpty(privilegeMemberInfo.TimeRule))
            {
               if(privilegeMemberInfo.TimeRule=="1")
               {
                   privilegeMemberInfo.ExpireTime=Convert.ToDateTime(DateTime.Now.ToString("yyyyy-MM-dd") + " 23:59:59");
               }
            }
            privilegeMemberInfo.PrivilegeStatus=1;
            return Content(iwechat.PostTicket(privilegeMemberInfo));
        }

        public ResultObj<List<PrivilegeMemberRelationModel>> GetPrivilegeByMemberId(string memberId = "", int pageIndex=1, int pageSize=10)
        {
            PrivilegeMemberRelationModel privilegeMemberInfo=new PrivilegeMemberRelationModel();
            privilegeMemberInfo.MemberId=memberId;
            privilegeMemberInfo.PageIndex=pageIndex;
            privilegeMemberInfo.PageSize=pageSize;

            IWechat iwechat=new WechatService();
            return Content(iwechat.GetPrivilegeByMemberId(privilegeMemberInfo));
        }

        /// <summary>
        /// 取得会员可领优惠券的次数
        /// </summary>
        /// <param name="memberId"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public ResultObj<int> GetCanTakeTicketCount(string memberId = "", string clientId = "", string principleType="")
        {
            PrivilegeMemberRelationModel privilegeMemberInfo = new PrivilegeMemberRelationModel();
            privilegeMemberInfo.MemberId = memberId;
            privilegeMemberInfo.ClientId = clientId;
            privilegeMemberInfo.PrincipleType = principleType;

            IWechat iwechat = new WechatService();
            return Content(iwechat.GetCanTakeTicketCount(privilegeMemberInfo));
        }
    }
}