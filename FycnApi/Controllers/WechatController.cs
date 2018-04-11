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
            WxPayConfig payConfig = _ipay.GenerateConfigModelW(m);
            PayModel payInfo = new PayModel();
            //JsApi.payInfo = new PayModel();
            payInfo.k = m;
            JsApi jsApi = new JsApi();
            jsApi.GetOpenidAndAccessToken(code, payConfig, payInfo,"#/w/"+m, "snsapi_userinfo");
            PayStateModel payState = new PayStateModel();
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
            log.Info("jsonUserInfo:" + jsonUserInfo);

            //log.Info("test");

            return Content(payState);
        }
    }
}