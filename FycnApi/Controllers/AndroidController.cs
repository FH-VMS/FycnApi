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
using Fycn.Model.Machine;
using System.Linq;

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
            string json = JsonHandler.GetJsonStrFromObject(jsonModel);
            //byte[] byteSend = System.Text.Encoding.Default.GetBytes(json);
            //string hex = ByteHelper.byteToHexStr(byteSend);
            Dictionary<string, string> dicRet = new Dictionary<string, string>();
            dicRet["url"] = "http://120.27.217.224/p/m.html#/paybyproduct?k=" + json;
            dicRet["waresId"] = waresId;
            string retJson = JsonHandler.GetJsonStrFromObject(dicRet);
            return HttpUtility.UrlDecode(retJson); 
        }

        public ResultObj<List<ProductForMachineModel>> GetProductByMachine(string machineId, int pageIndex = 1, int pageSize = 10)
        {
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
            ProductForMachineModel machineInfo = new ProductForMachineModel();
            machineInfo.MachineId = machineId;
            machineInfo.PageIndex=pageIndex;
            machineInfo.PageSize=pageSize;
            IMachine imachine = new MachineService();
            var users = imachine.GetProductAndroid(machineInfo);
            int totalcount = imachine.GetProductAndroidCount(machineInfo);

            var pagination = new Pagination { PageSize = pageSize, PageIndex = pageIndex, StartIndex = 0, TotalRows = totalcount, TotalPage = 0 };
            
            return Content(users, pagination);
        }

        public void TestInt()
        {
            byte[] m = ByteHelper.strToAscii("01");
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

        public string TestRedis()
        {
            RedisHelper redis0 = new RedisHelper(0);
            return redis0.KeyExists("adfadfasd").ToString();
        }

        public string De(string code)
        {
            byte[] b = ByteHelper.strToToHexByte(code);
            ByteHelper.Deencryption(b.Length - 5, b.Skip(4).Take(b.Length - 5).ToArray()).CopyTo(b,4);
            return ByteHelper.byteToHexStr(b);
        }
    }
}
