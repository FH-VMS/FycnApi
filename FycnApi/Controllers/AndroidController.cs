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
        public string RequestPayUrl(string machineId,string waresId)
        {
            KeyJsonModel jsonModel = new KeyJsonModel();
            jsonModel.m = machineId;
            jsonModel.t = new List<KeyTunnelModel>();
            jsonModel.t.Add(new KeyTunnelModel() { tid = waresId, n = "1" });
            string json = HttpUtility.UrlDecode(JsonHandler.GetJsonStrFromObject(jsonModel));
            byte[] byteSend = System.Text.Encoding.Default.GetBytes(json);
            string hex = ByteHelper.byteToHexStr(byteSend);
            Dictionary<string, string> dicRet = new Dictionary<string, string>();
            dicRet["url"] = "http://120.27.217.224/p/m.html#/paybyproduct?k=" + hex;
            dicRet["waresId"] = waresId;
            string retJson = JsonHandler.GetJsonStrFromObject(dicRet);
            return HttpUtility.UrlDecode(retJson); 
        }

        public void TestInt()
        {
            byte[] x = ByteHelper.strToToHexByte("03E8");
            string y = ByteHelper.GenerateRealityData(x, "intval");
            int value = 4;
            int hValue = (value >> 8) & 0xFF;
            int lValue = value & 0xFF;
            byte[] arr = new byte[] { (byte)hValue, (byte)lValue };
        }

        public string TestStr(string k)
        {
            
            return System.Text.Encoding.Default.GetString(ByteHelper.strToToHexByte(k));
        }
    }
}
