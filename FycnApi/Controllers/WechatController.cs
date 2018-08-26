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
using System.Security.Cryptography;
using Fycn.Model.Machine;
using System.Web;
using Microsoft.AspNetCore.Cors;

namespace FycnApi.Controllers
{
    //[Produces("application/json")]
    //[Route("api/Wechat")]
    public class WechatController : ApiBaseController
    {
        public ResultObj<PayStateModel> GetUrl(string m, string code, string retBack = "")
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
            string backUrl = "/wechat.html?clientId="+m;
            if(!string.IsNullOrEmpty(retBack)) {
                backUrl = backUrl + HttpUtility.UrlDecode(retBack);
            }
            jsApi.GetOpenidAndAccessToken(code, payConfig, payInfo,backUrl, "snsapi_userinfo");
            
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

        public ResultObj<List<ProductListModel>> GetProdcutByTypeAndClient(string typeId="",string clientId="")
        {
            IWechat iwechat = new WechatService();
            return Content(iwechat.GetProdcutByTypeAndClient(typeId,clientId));
        }

        //微信支付
        public ResultObj<PayStateModel> PostDataW(string clientId,string openId,string privilegeIds,string selfChosen, [FromBody]List<ProductPayModel> lstProductPay)
        {
            var log = LogManager.GetLogger("FycnApi", "wechat");
               
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
                        if(productPay[0].IsGroup == 1)
                        {
                            hasGroup = true;
                        }
                        totalFee = totalFee + Convert.ToInt32(productPay[0].Number) * Convert.ToDecimal(productInfo.WaresDiscountUnitPrice == 0 ? productInfo.WaresUnitPrice: productInfo.WaresDiscountUnitPrice);
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
                if(totalFee>0.01M && !hasGroup)
                {
                    List<PrivilegeMemberRelationModel> lstPrivilege=new List<PrivilegeMemberRelationModel>();
                  if (string.IsNullOrEmpty(privilegeIds) && string.IsNullOrEmpty(selfChosen))
                  {
                        lstPrivilege = _iwechat.GetCanUsePrivilege(privilegeInfo, privilegeIds, ref totalFee, lstProductPay);
                  }
                  else if (!string.IsNullOrEmpty(privilegeIds))
                  {
                        lstPrivilege = _iwechat.GetChosenPrivilege(privilegeInfo, privilegeIds, ref totalFee, lstProductPay);
                    }
                   
            


                   log.Info("ddddd" + lstPrivilege.Count);
                    if (lstPrivilege.Count > 0)
                    {
                        string[] lstStr =lstPrivilege.Select(m=>m.Id).ToArray();
                        if(string.IsNullOrEmpty(privilegeIds))
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
               payState.TotalMoney=(totalFee <= 0 ? Convert.ToDecimal(0.01) : totalFee);
               //写入交易中转
               if(payInfo.total_fee==0)
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
                    //log.Info("nnnnnnn" + tradeNoNode.InnerText);
                    //log.Info("aaaaaaa"+retProducts);
                    List<ProductPayModel> lstProductPay = JsonHandler.GetObjectFromJson<List<ProductPayModel>>(retProducts);
                    log.Info("sssss" + JsonHandler.GetJsonStrFromObject(lstProductPay, false));
                    log.Info("mmmmm" + jsonProduct);
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
        public ResultObj<List<PrivilegeModel>> GetActivityPrivilegeList(string clientId = "", string activityType="")
        {
            IWechat iwechat = new WechatService();
            ActivityModel activityInfo = new ActivityModel();
            activityInfo.ClientId = clientId;
            activityInfo.ActivityType = activityType;
            //取活动列表
            List<ActivityModel> lstActivityInfo = iwechat.GetActivityList(activityInfo);
            string activityId = string.Empty;
            if (lstActivityInfo.Count == 0)
            {
                return Content(new List<PrivilegeModel>());
            }
            else
            {
                activityId = lstActivityInfo[0].Id;
            }
            ActivityPrivilegeRelationModel activityRelationInfo =new ActivityPrivilegeRelationModel();
            activityRelationInfo.ActivityId = activityId;

            
            return Content(iwechat.GetActivityPrivilegeListById(activityRelationInfo));
        }

        public ResultObj<int> GetTicket([FromBody]PrivilegeMemberRelationModel privilegeMemberInfo)
        {
            IWechat iwechat=new WechatService();
            int count=iwechat.IsExistTicket(privilegeMemberInfo);
             ActivityModel activityInfo = new ActivityModel();
            activityInfo.ClientId = privilegeMemberInfo.ClientId;
            activityInfo.ActivityType = privilegeMemberInfo.PrincipleType;
            List<ActivityModel> lstActivity = iwechat.GetActivityList(activityInfo);
            if(lstActivity.Count == 0)
            {
                return Content(0);
            }
            if(count>=lstActivity[0].CountPerPerson)
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
        public ResultObj<int> GetCanTakeTicketCount(string memberId = "", string clientId = "", string activityType="")
        {
            PrivilegeMemberRelationModel privilegeMemberInfo = new PrivilegeMemberRelationModel();
            privilegeMemberInfo.MemberId = memberId;
            privilegeMemberInfo.ClientId = clientId;
            privilegeMemberInfo.ActivityType = activityType;

            IWechat iwechat = new WechatService();
            
            //
            int nowTicket = iwechat.GetTicketCountByTime(privilegeMemberInfo);
            ActivityModel activityInfo = new ActivityModel();
            activityInfo.ClientId = clientId;
            activityInfo.ActivityType = activityType;
            List<ActivityModel> lstActivity = iwechat.GetActivityList(activityInfo);
            int settingCount = 0;
            if (lstActivity.Count == 0)
            {
                return Content(0);
            }
            else
            {
                settingCount = lstActivity[0].CountPerPerson;
            }
            int canTake = settingCount - nowTicket;
            if(canTake <= 0)
            {
                return Content(0);
            }
            
            return Content(canTake);
        }

        #region 取货
        [EnableCors("AllowAllOrigin")]
        [HttpPost]
        public string VerifyCode(string machid="",string pickcode="")
        {
            var log = LogManager.GetLogger("FycnApi", "wechat");

            log.Info("pickup:machid:" + machid + "pickcode:" + pickcode);
            RedisHelper redisHelper3 = new RedisHelper(3);
            if(!redisHelper3.KeyExists(pickcode))
            {
                return "NG取货码错误";
            }
            ICommon icommon = new CommonService();
            if(string.IsNullOrEmpty(machid))
            {
                return "NG机器编号错误";
            }
            if(string.IsNullOrEmpty(pickcode))
            {
                return "NG取货码错误";
            }
            int machineCount = icommon.CheckMachineId(machid, "");
            if(machineCount==0)
            {
                return "NG机器编号不存在";
            }
            IWechat iwechat = new WechatService();
            ClientSalesRelationModel clientSalesRelation = new ClientSalesRelationModel();
            clientSalesRelation.MachineId = machid;
            clientSalesRelation.PickupNo = pickcode;
            var lstRelation = iwechat.VerifyPickupCode(clientSalesRelation);
            if(lstRelation.Count==0)
            {
                return "NG取货码不存在";
            }
            if(lstRelation.Count>1)
            {
                return "NG取货码错误";
            }
            
            redisHelper3.KeyDelete(pickcode);
            return "OK" + lstRelation[0].WaresName;
            /*
            var request = Fycn.Utility.HttpContext.Current.Request;
            int len = (int)request.ContentLength;
            byte[] b = new byte[len];
            Fycn.Utility.HttpContext.Current.Request.Body.Read(b, 0, len);
            string postStr = Encoding.UTF8.GetString(b);
            log.Info("postStr" + postStr);
            */
        }

        [EnableCors("AllowAllOrigin")]
        [HttpPost]
        public string PickupResult(string machid,string pickcode,string name,string price,string trackno,string saletime)
        {
            var log = LogManager.GetLogger("FycnApi", "wechat");

            log.Info("PickupResult:" + machid + "pickcode:" + pickcode + "name:"+name+"price:"+price+"trackno:"+trackno+"saletime:"+saletime);
            IWechat iwechat = new WechatService();
            ClientSalesRelationModel clientSalesRelation = new ClientSalesRelationModel();
            clientSalesRelation.TotalNum = 1;
            clientSalesRelation.TunnelId = trackno;
            clientSalesRelation.MachineId = machid;
            clientSalesRelation.PickupNo = pickcode;
            clientSalesRelation.Remark = "成功";
            int result = iwechat.PutPayResultByPickupCode(clientSalesRelation);
            if(result==3)
            {
                return "NG取货码不存在";
            }
            if(result==0)
            {
                return "NG系统内部错误";
            }

            return "OK";
            /*
            var request = Fycn.Utility.HttpContext.Current.Request;
            int len = (int)request.ContentLength;
            byte[] b = new byte[len];
            Fycn.Utility.HttpContext.Current.Request.Body.Read(b, 0, len);
            string postStr = Encoding.UTF8.GetString(b);
            log.Info("result postStr" + postStr);
            return "NG 测试";
            */
        }
        #endregion


        public ResultObj<PayStateModel> GetWeixinJsConfig(string clientId)
        {
            var log = LogManager.GetLogger("FycnApi", "wechat");
            string url = string.Empty;
            PayStateModel payState = new PayStateModel();
            //KeyJsonModel keyJsonInfo = PayHelper.AnalizeKey(k);
            try
            {
                IPay _ipay = new PayService();
                WxPayConfig payConfig = _ipay.GenerateConfigModelWByClientId(clientId);
               
                if (string.IsNullOrEmpty(payConfig.APPID))
                {
                    payState.RequestState = "2";
                    payState.ProductJson = "";
                    payState.RequestData = "";
                    return Content(payState);

                }
                RedisHelper rh = new RedisHelper(4);
                string ticket = rh.StringGet(clientId + "-ticket");
                if (string.IsNullOrEmpty(ticket))
                {
                    string urlAcess = string.Format("https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid={0}&secret={1}", payConfig.APPID, payConfig.APPSECRET);
                    string jsonResult = HttpService.Get(urlAcess);
                    log.Info("access_token:" + jsonResult);
                    Dictionary<string, string> dicAcess = JsonHandler.GetObjectFromJson<Dictionary<string, string>>(jsonResult);

                    string urlTicket = string.Format("https://api.weixin.qq.com/cgi-bin/ticket/getticket?type=jsapi&access_token={0}", dicAcess["access_token"]);
                    string jsonTicket = HttpService.Get(urlTicket);
                    log.Info("ticket:" + jsonTicket);
                    Dictionary<string, string> dicTicket = JsonHandler.GetObjectFromJson<Dictionary<string, string>>(jsonTicket);
                    if (dicTicket["errmsg"] == "ok")
                    {
                        ticket = dicTicket["ticket"];
                        rh.StringSet(clientId + "-ticket", dicTicket["ticket"], new TimeSpan(1, 50, 50));
                    }
                }

                payState.RequestState = "1";
                payState.ProductJson = "";
                string retJson = MakeJsSign(payConfig.APPID, ticket);
                log.Info("result:" + retJson);
                payState.RequestData = retJson;
            }
            catch(Exception ex)
            {

            }
            return Content(payState);
        }

        private string MakeJsSign(string appId,string ticket)
        {
            string timeStamp = WxPayApi.GenerateTimeStamp();
            string onceStr = WxPayApi.GenerateNonceStr();
            string signature = string.Empty;
            WxPayData jsApiParam = new WxPayData();
            jsApiParam.SetValue("appId", appId);
            jsApiParam.SetValue("timestamp", timeStamp);
            jsApiParam.SetValue("nonceStr", onceStr);
            //jsApiParam.SetValue("signature", signature);
            //string keys = string.Format("jsapi_ticket={0}&noncestr={1}&timestamp={2}&url={3}", ticket,onceStr,timeStamp, PathConfig.DomainConfig+"/wechat.html");
            Dictionary<string, string> dicParam = new Dictionary<string, string>();
            dicParam["jsapi_ticket"] = ticket;
            dicParam["noncestr"] = onceStr;
            dicParam["timestamp"] = timeStamp;
            dicParam["url"] = PathConfig.DomainConfig + "/wechat.html";

            signature = Sha1(dicParam, Encoding.UTF8).ToLower();
            jsApiParam.SetValue("signature", signature);
            return jsApiParam.ToJson();
        }

        private string Sha1(Dictionary<string, string> dicParam, Encoding encode)
        {
            try
            {
                var vDic = (from objDic in dicParam orderby objDic.Key ascending select objDic);
                StringBuilder str = new StringBuilder();
                foreach (KeyValuePair<string, string> kv in vDic)
                {
                    string pkey = kv.Key;
                    string pvalue = kv.Value;
                    str.Append(pkey + "=" + pvalue + "&");
                }
                SHA1 sha1 = new SHA1CryptoServiceProvider();
                byte[] bytes_in = encode.GetBytes(str.ToString().TrimEnd('&'));
                byte[] bytes_out = sha1.ComputeHash(bytes_in);
                sha1.Dispose();
                string result = BitConverter.ToString(bytes_out);
                result = result.Replace("-", "");
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("SHA1加密出错：" + ex.Message);
            }
        }

        //取附近的机器
        public ResultObj<List<MachineLocationModel>> GetMachineLocations(string longitude="", string latitude="",string clientId="", int pageIndex=1,int pageSize=10)
        {
            IWechat iwechat = new WechatService();
            MachineLocationModel machineLocations = new MachineLocationModel();
            machineLocations.PageIndex = pageIndex;
            machineLocations.PageSize = pageSize;
            machineLocations.Longitude = longitude;
            machineLocations.Latitude = latitude;
            machineLocations.ClientId=clientId;
            return Content(iwechat.GetMachineLocations(machineLocations));
        }

        //取未过期的券
        public ResultObj<List<PrivilegeMemberRelationModel>> GetNoneExpirePrivilegeByMemberId(string memberId = "")
        {
            PrivilegeMemberRelationModel privilegeMemberInfo = new PrivilegeMemberRelationModel();
            privilegeMemberInfo.MemberId = memberId;

            IWechat iwechat = new WechatService();
            return Content(iwechat.GetNoneExpirePrivilegeByMemberId(privilegeMemberInfo));
        }

        //根据商品id取对应的商品或商品组
        public ResultObj<ProductListModel> GetProdcutAndGroupByWaresId(string waresId)
        {
            if (string.IsNullOrEmpty(waresId)) 
            {
               return null;
            }
            IWechat iwechat = new WechatService();
            List<ProductListModel> lstProduct = iwechat.GetProdcutAndGroupByWaresId(waresId);
            if (lstProduct.Count != 1)  
            {
                return null;
            }
            else 
            {
                return Content(lstProduct[0]);
            }
        }

        [HttpPost]
        public ResultObj<int> PostFriendShare(string otherOpenId,string myOpenId,string pickupNo)
        {
            ClientSalesRelationModel clientSalesInfo = new ClientSalesRelationModel();
            clientSalesInfo.MemberId = myOpenId;
            clientSalesInfo.PickupNo = pickupNo;
            IWechat iwechat = new WechatService();
            var lstResult = iwechat.GetClientSalesByPickNo(otherOpenId, clientSalesInfo);
            if(lstResult.Count==0)
            {
                return Content(0);
            }
            if(lstResult[0].MemberId== myOpenId)
            {
                return Content(2);
            }
            return Content(iwechat.ExchangeFromFriend(clientSalesInfo));
        }
    }
}