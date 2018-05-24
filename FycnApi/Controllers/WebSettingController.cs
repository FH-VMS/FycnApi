using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using FycnApi.Base;
using Fycn.Model.Wechat;
using Fycn.Interface;
using Fycn.Service;
using Fycn.Model.Sys;
using Fycn.Utility;

namespace FycnApi.Controllers
{
    public class WebSettingController : ApiBaseController
    {
        private static IBase<WebSettingModel> _IBase
        {
            get
            {
                return new WebSettingService();
            }
        }

        public ResultObj<List<WebSettingModel>> GetData(string clientId = "")
        {
            WebSettingModel webSettingInfo = new WebSettingModel();
            webSettingInfo.ClientId = clientId;
            List<WebSettingModel> lstWebSetting = _IBase.GetAll(webSettingInfo);
            if (lstWebSetting.Count > 0)
            {
                lstWebSetting[0].StaticUrl = ConfigHandler.ResourceUrl;
            }
            return Content(lstWebSetting);
        }

        public ResultObj<int> CreateWebInfo([FromBody]WebSettingModel webSettingInfo)
        {
            int count = _IBase.GetCount(webSettingInfo);
            int result = 0;
            if(count>0)
            {
                result=_IBase.UpdateData(webSettingInfo);
            }
            else
            {
                result = _IBase.PostData(webSettingInfo);
            }

            return Content(result);
        }
    }
}