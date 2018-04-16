using FycnApi.Base;
using Fycn.Interface;
using Fycn.Model.Pay;
using Fycn.Model.Sys;
using Fycn.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc;
using Fycn.Utility;

namespace FycnApi.Controllers
{
    public class PayConfigController : ApiBaseController
    {
        private static IBase<ConfigModel> _IBase
        {
            get
            {
                return new PayConfigService();
            }
        }

        public ResultObj<List<ConfigModel>> GetData(int pageIndex = 1, int pageSize = 10)
        {
            // IProduct service = new ProductService();
            //List<ProductModel> products = service.GetAllProducts();

            ConfigModel configInfo = new ConfigModel();
            configInfo.PageIndex = pageIndex;
            configInfo.PageSize = pageSize;
            var users = _IBase.GetAll(configInfo);
            int totalcount = _IBase.GetCount(configInfo);

            var pagination = new Pagination { PageSize = pageSize, PageIndex = pageIndex, StartIndex = 0, TotalRows = totalcount, TotalPage = 0 };
            return Content(users, pagination);
        }

        public ResultObj<int> PostData([FromBody]ConfigModel configInfo)
        {
            IPayConfig payConfig = new PayConfigService();
            List<ConfigModel> lstConfigs = payConfig.GetWxConfigByMchId(configInfo.WxMchId);
            if (lstConfigs.Count > 0)
            {
               return Content(0, ResultCode.Fail, "保存失败,已存在微信商户号", new Pagination { });
            }
            //写入微信txt文本
            int result = _IBase.PostData(configInfo);
            if(result>0)
            {
                if (!string.IsNullOrEmpty(configInfo.WxTxtKey.Trim()))
                {
                    FileHandler.DeleteFile(ConfigHandler.WeixinTextAddress + "/MP_verify_" + configInfo.WxTxtKey+".txt");
                    FileHandler.WriteFile(ConfigHandler.WeixinTextAddress, "MP_verify_" + configInfo.WxTxtKey + ".txt", configInfo.WxTxtKey);
                }
            }
            return Content(result);
        }

        public ResultObj<int> PutData([FromBody]ConfigModel configInfo)
        {
            IPayConfig payConfig = new PayConfigService();
            List<ConfigModel> lstConfigs = payConfig.GetWxConfigByMchId(configInfo.WxMchId);
            if(lstConfigs.Count > 0 && lstConfigs[0].Id!= configInfo.Id)
            {
                return Content(0, ResultCode.Fail, "更新失败,已存在微信商户号", new Pagination { });
            }
            int result = _IBase.UpdateData(configInfo);
            if (result > 0)
            {
                if(!string.IsNullOrEmpty(configInfo.WxTxtKey.Trim()))
                {
                    FileHandler.DeleteFile(ConfigHandler.WeixinTextAddress + "/MP_verify_" + configInfo.WxTxtKey + ".txt");
                    FileHandler.WriteFile(ConfigHandler.WeixinTextAddress, "MP_verify_" + configInfo.WxTxtKey + ".txt", configInfo.WxTxtKey);
                }
               
            }
            return Content(result);
        }

        public ResultObj<int> DeleteData(string idList)
        {

            return Content(_IBase.DeleteData(idList));
        }

        public ResultObj<int> UpdateWxCert(string mchId,string id)
        {
            ConfigModel configInfo = new ConfigModel();
            configInfo.WxSslcertPassword = mchId;
            configInfo.WxSslcertPath = "cert/" + mchId + "/apiclient_cert.p12";
            configInfo.Id = id;
            IPayConfig payConfig = new PayConfigService();
            return Content(payConfig.UpdateWxCert(configInfo));
        }


    }
}