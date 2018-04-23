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

namespace FycnApi.Controllers
{
    //[Produces("application/json")]
    //[Route("api/Wechat")]
    public class WechatController : ApiBaseController
    {
        public ResultObj<PayStateModel> GetUrl(string m, string code)
        {

            var log = LogManager.GetLogger("FycnApi", "wechat");
            log.Info("mmmmmmmmmmmmmmmm:"+m);
            log.Info("code:" + code);
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
            jsApi.GetOpenidAndAccessToken(code, payConfig, payInfo,"/wechat.html#/?clientId="+m, "snsapi_userinfo");
            
            if (string.IsNullOrEmpty(payInfo.openid))
            {
                payState.RequestState = "0";
                payState.ProductJson = "";
                payState.RequestData = payInfo.redirect_url;
                return Content(payState);
            }
            log.Info("openID:"+ payInfo.openid);
            //根据code 返回access_token
            //string urlAcess = string.Format("https://api.weixin.qq.com/sns/oauth2/access_token?appid={0}&secret={1}&code={2}&grant_type=authorization_code", payConfig.APPID,payConfig.APPSECRET,code);
            //string jsonResult = HttpService.Get(urlAcess);
            //log.Info("access_token:" + jsonResult);

            //Dictionary<string,string> dicAcess = JsonHandler.GetObjectFromJson<Dictionary<string,string>>(jsonResult);
            string accessToken = payInfo.access_token;//dicAcess["access_token"];
            log.Info("accessToken:" + payInfo.openid);
            //取授权用户信息
            string urlUserInfo = string.Format("https://api.weixin.qq.com/sns/userinfo?access_token={0}&openid={1}&lang=zh_CN",accessToken,payInfo.openid);
            string jsonUserInfo = HttpService.Get(urlUserInfo);

            payState.RequestState = "1";
            payState.ProductJson = jsonUserInfo;
            payState.RequestData = "";
            log.Info("jsonUserInfo:" + jsonUserInfo);
            IWechat iwechat = new WechatService();
            WechatMemberModel memberInfo = new WechatMemberModel();
            memberInfo.OpenId = payInfo.openid;
            List<WechatMemberModel> lstMemberInfo = iwechat.IsExistMember(memberInfo);
            if(lstMemberInfo.Count==0)
            {
                WechatMemberModel createMemberInfo = JsonHandler.GetObjectFromJson<WechatMemberModel>(jsonUserInfo);
                ClientMemberRelationModel clientMemberInfo = new ClientMemberRelationModel();
                clientMemberInfo.ClientId = m;
                clientMemberInfo.MemberId = createMemberInfo.OpenId;
                iwechat.CreateMember(createMemberInfo, clientMemberInfo);
            }
            if (lstMemberInfo.Count > 0 && string.IsNullOrEmpty(lstMemberInfo[0].ClientId))
            {
                ClientMemberRelationModel clientMemberInfo = new ClientMemberRelationModel();
                clientMemberInfo.ClientId = m;
                clientMemberInfo.MemberId = lstMemberInfo[0].OpenId;
                iwechat.CreateClientAndMemberRelation(clientMemberInfo);
            }
            //if(iwechat.IsExistMember(memberInfo))
            //log.Info("test");

            return Content(payState);
        }
    }
}