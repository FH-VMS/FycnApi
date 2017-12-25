using FycnApi.Base;
using Fycn.Interface;
using Fycn.Model.Common;
using Fycn.Model.Sys;
using Fycn.Model.User;
using Fycn.Service;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Fycn.Utility;
using Fycn.Model.Socket;
using Fycn.Model.Pay;
using System.Threading;
using log4net;
using System.Web;

namespace FycnApi.Controllers
{
    public class AndroidController : ApiBaseController
    {
        public ResultObj<string> RequestPayUrl(string machineId,string waresId)
        {
            KeyJsonModel jsonModel = new KeyJsonModel();
            jsonModel.m = machineId;
            jsonModel.t = new List<KeyTunnelModel>();
            jsonModel.t.Add(new KeyTunnelModel() { tid = waresId, n = "1" });
            string json = JsonHandler.GetJsonStrFromObject(jsonModel);
            byte[] byteSend = System.Text.Encoding.Default.GetBytes(json);
            string hex = ByteHelper.byteToHexStr(byteSend);
            return Content("http://120.27.217.224/p/m.html#/paybyproduct?k=" + hex); 
        }
    }
}
