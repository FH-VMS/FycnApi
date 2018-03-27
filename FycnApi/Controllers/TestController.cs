﻿using System;
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

        public string TestCertifcate()
        {

            WxPayData jsApiParam = new WxPayData();
            jsApiParam.SetValue("mch_id", "mch_id");
            jsApiParam.SetValue("nonce_str", WxPayApi.GenerateNonceStr());
            jsApiParam.SetValue("sign", jsApiParam.MakeSign());

            return HttpService.Post(jsApiParam.ToXml(), "https://apitest.mch.weixin.qq.com/sandboxnew/pay/getsignkey", true, 10000);
        }
        
    }
}