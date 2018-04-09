using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Fycn.Utility;
using FycnApi.Base;
using Fycn.Interface;
using Fycn.Service;
using Fycn.Model.Pay;
using Fycn.PaymentLib.wx;
using System.IO;
using System.Text;

namespace FycnApi.Controllers
{
    public class TestController : ApiBaseWithAllOriginController
    {
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
            ByteHelper.Deencryption(b.Length - 5, b.Skip(4).Take(b.Length - 5).ToArray()).CopyTo(b, 4);
            return ByteHelper.byteToHexStr(b);
        }

        public void TestZhiFu()
        {
            IMachine imachine = new MachineService();
            KeyJsonModel jsonModel = new KeyJsonModel();
            jsonModel.m = "ABC000000001";
            jsonModel.t = new List<KeyTunnelModel>();
            jsonModel.t.Add(new KeyTunnelModel() {
                 tid="A0101",
                  wid= "f2408a65-3355-49dc-b0ad-0f98861943fa",
                  tn="20180123456789"
            });
           // imachine.PostPayResultA(jsonModel, "20180123456789", "20180123456789");
        }

       


        public void TestUpdateOnline()
        {
            string strdate = "20150427154557";
            if (strdate.Length == 14)
            {
                string year = strdate.Substring(0, 4);
                string month = strdate.Substring(4, 2);
                string day = strdate.Substring(6, 2);
                string hour = strdate.Substring(8, 2);
                string minute = strdate.Substring(10, 2);
                string second = strdate.Substring(12, 2);
                DateTime time = Convert.ToDateTime(string.Format("{0}-{1}-{2} {3}:{4}:{5}",year,month,day,hour,minute,second));
            }
           
        }


        public void GetBody()
        {
            var request = Fycn.Utility.HttpContext.Current.Request;
            int len = (int)request.ContentLength;
            byte[] b = new byte[len];
            Fycn.Utility.HttpContext.Current.Request.Body.Read(b,0,len);
            string postStr = Encoding.UTF8.GetString(b);
            
        }

        public void TestRedisValue()
        {
            MachineHelper.ClearCode("123","3434");
        }
        
    }
}