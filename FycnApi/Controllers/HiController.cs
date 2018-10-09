using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Fycn.Interface;
using Fycn.Model.Android;
using Fycn.Model.Pay;
using Fycn.Model.Sys;
using Fycn.Model.Wechat;
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
    }
}