﻿using FycnApi.Base;
using Fycn.Interface;
using Fycn.Model.Machine;
using Fycn.Model.Pay;
using Fycn.Model.Sale;
using Fycn.Model.Sys;
using Newtonsoft.Json;
using Fycn.PaymentLib.wx;
using Fycn.Service;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;

using System.Xml;
using Fycn.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using log4net;

namespace FycnApi.Controllers
{
    public class MachineController : ApiBaseController
    {
        private static IMachine _IMachine
        {
            get
            {
                return new MachineService();
            }
        }

        #region 机器需要接口
        // 返回支付结果，需要出货的东东
        public string GetPayResult(string k)
        {
            KeyJsonModel keyJsonInfo = AnalizeKey(k);
            ISale _isale = new SalesService();
            //trade_status: (0:待支付，1:支付成功待出货,2：支付成功且已全部出货,3：支付成功部分出货成功未退款，4:支付成功部分出货成功已退款，5：支付成功此货道出货全部出货失败未退款，5：支付成功此货道出货全部出货失败已退款)
            List<KeyTunnelModel> lstSales = _isale.GetPayResult("", "1", keyJsonInfo.m);
            if (lstSales.Count == 0)
            {
                return "";
            }
            return JsonHandler.GetJsonStrFromObject(lstSales,false);
        }

        //出货后，告诉汇报出货情况并更新库存
        public string GetOutResult(string k)
        {
           
            KeyJsonModel keyJsonInfo = AnalizeKey(k);
            int result = _IMachine.PutPayResult(keyJsonInfo);
            if (result == 1)
            {
                var tns = from m in keyJsonInfo.t
                          where m.s=="3" || m.s=="5"
                          select m.tn;
                
                RefundController refund = new RefundController();
                refund.PostRefund(tns.ToList<string>());
            }

            return result==1?"OK":"NG";
        }

        // 心跳
        //[EnableCors("AllowSpecificOrigin")]
        
        public string GetHeartBeep(string k)
        {
            KeyJsonModel keyJsonInfo = AnalizeKey(k);
            DataTable dt = _IMachine.GetBeepHeart(keyJsonInfo.m);
            if (dt.Rows.Count > 0)
            {
                return "{\"OK\":"+JsonHandler.DataTable2Json(dt) + "}";
            }
            else
            {
                return "{\"OK\":\"\"}";
            }
           
        }

        // 上报机器下行处理结果
        public string GetHandleResult(string k)
        {
            ToMachineModel toMachineInfo = JsonHandler.GetObjectFromJson<ToMachineModel>(k);
            int result = _IMachine.GetHandleResult(toMachineInfo.m, toMachineInfo.s);
            return result == 1 ? "OK" : "NG";
        }

        //向机器下行价格
        public string GetToMachinePrice(string k)
        {
            Dictionary<string, string> values = JsonConvert.DeserializeObject<Dictionary<string, string>>(k);
            return JsonHandler.DataTable2Json(_IMachine.GetToMachinePrice(values["m"], Convert.ToInt32(values["start"]), Convert.ToInt32(values["len"])));
        }

         //向机器下行当前补货库存
        public string GetToMachineStock(string k)
        {
            Dictionary<string, string> values = JsonConvert.DeserializeObject<Dictionary<string, string>>(k);
            return JsonHandler.DataTable2Json(_IMachine.GetToMachineStock(values["m"], Convert.ToInt32(values["start"]), Convert.ToInt32(values["len"])));
        }

        //一键补货
        public string GetFullfilGood(string k)
        {
            KeyJsonModel keyJsonInfo = AnalizeKey(k);
            int result = _IMachine.GetFullfilGood(keyJsonInfo.m);
            return result == 1 ? "OK" : "NG";
        }

        //按货道补货
        public string GetFullfilGoodByTunnel(string k)
        {
            KeyJsonModel keyJsonInfo = AnalizeKey(k);
            int result = _IMachine.GetFullfilGoodByTunnel(keyJsonInfo);
            return result == 1 ? "OK" : "NG";
        }

        //机器端设置价格和最大库存上报
        public string GetReportMaxStockAndPrice(string k)
        {
            //FileHandler.LogMachineData(new string[] { "GetReportMaxStockAndPrice", k, DateTime.Now.ToString() });
            //LogMachineData(new string[] {"GetReportMaxStockAndPrice",k,DateTime.Now.ToString()});
            PriceAndMaxStock priceAndMaxStock = JsonHandler.GetObjectFromJson<PriceAndMaxStock>(k);
            int result = _IMachine.PostMaxStockAndPrice(priceAndMaxStock.t, priceAndMaxStock.m);
            return result == 1 ? "OK" : "NG";
        }

        //取机器设置接口
        public string GetMachineSetting(string k)
        {
            KeyJsonModel keyJsonInfo = AnalizeKey(k);
            DataTable dt = _IMachine.GetMachineSetting(keyJsonInfo.m);
            return JsonHandler.DataTable2Json(dt);
        }


        #endregion

        //取销售的商品列表
        public ResultObj<List<ProductForMachineModel>> GetProductByMachine(string k = "", int pageIndex = 1, int pageSize = 10)
        {
            //KeyJsonModel keyJsonInfo = AnalizeKey(k);
            // IProduct service = new ProductService();
            //List<ProductModel> products = service.GetAllProducts();
            //k = "ABC123456789";
            //机器运行情况
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
            ProductForMachineModel machineInfo = new ProductForMachineModel();
            machineInfo.MachineId = k;
            machineInfo.PageIndex = pageIndex;
            machineInfo.PageSize = pageSize;
            var users = _IMachine.GetProductByMachine(machineInfo);
            int totalcount = _IMachine.GetCount(machineInfo);

            var pagination = new Pagination { PageSize = pageSize, PageIndex = pageIndex, StartIndex = 0, TotalRows = totalcount, TotalPage = 0 };
            return Content(users, pagination);
        }


        //对k进行解码 k格式：{"m":"123128937","t":[{"tid":"1-2","n":3},{"tid":"1-3","n":2}]}
        private KeyJsonModel AnalizeKey(string key)
        {
           KeyJsonModel keyJsonInfo = JsonHandler.GetObjectFromJson<KeyJsonModel>(key);
           return keyJsonInfo;
        }


        #region 支付结果通知
        // 微信支付结果
        public ResultObj<int> PostPayResultW()
        {
            try
            {
                Stream s = Stream.Null;
                Fycn.Utility.HttpContext.Current.Request.Body.CopyTo(s);
                byte[] b = new byte[s.Length];
                s.Read(b, 0, (int)s.Length);
                string postStr = Encoding.UTF8.GetString(b);
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(postStr);
                // 商户交易号
                XmlNode tradeNoNode = xmlDoc.SelectSingleNode("xml/out_trade_no");
                IMachine _imachine = new MachineService();
                if (_imachine.GetCountByTradeNo(tradeNoNode.InnerText) > 0)
                {
                    return Content(1);
                }
                //支付结果
                XmlNode payResultNode = xmlDoc.SelectSingleNode("xml/result_code");
                if (payResultNode.InnerText.ToUpper() == "SUCCESS")
                {
                    /*******************************放到微信支付通知参数里，因参数只支付最大128个字符长度，所以注释修改*****************************/
                    //XmlNode tunnelNode = xmlDoc.SelectSingleNode("xml/attach");
                    //KeyJsonModel keyJsonModel = JsonHandler.GetObjectFromJson<KeyJsonModel>(tunnelNode.InnerText);

                    string jsonProduct = FileHandler.ReadFile("data/" + tradeNoNode.InnerText + ".wa");
                    KeyJsonModel keyJsonModel = JsonHandler.GetObjectFromJson<KeyJsonModel>(jsonProduct);

                    _imachine.PostPayResultW(keyJsonModel, tradeNoNode.InnerText);
                    //删除文件
                    FileHandler.DeleteFile("data/" + tradeNoNode.InnerText + ".wa");
                }

                return Content(1);
            }
            catch (Exception ex)
            {
                return Content(0);
            }
            
            //File.WriteAllText(@"c:\text.txt", postStr); 
        }

        //支付宝支付结果
        /*
        public ResultObj<int> PostPayResultA(List<ProductModel> listProductInfo)
        {
            
            IMachine _imachine = new MachineService();
            _imachine.PostPayResultA(listProductInfo);
            return Content(1);
        }
        */
        public ResultObj<int> PostPayResultA()
        {
            try
            {
                string outTradeNo = Fycn.Utility.HttpContext.Current.Request.Form["out_trade_no"];
                IMachine _imachine = new MachineService();
                if (_imachine.GetCountByTradeNo(outTradeNo) > 0)
                {
                    return Content(1);
                }

                string tradeStatus = Fycn.Utility.HttpContext.Current.Request.Form["trade_status"].ToString().ToUpper();
                if (tradeStatus == "TRADE_SUCCESS")
                {
                    /*******************************放到微信支付通知参数里，因参数只支付最大128个字符长度，所以注释修改*****************************/
                    //string jsonProduct = Fycn.Utility.HttpContext.Current.Request.Form["body"];
                    //KeyJsonModel keyJsonModel = JsonHandler.GetObjectFromJson<KeyJsonModel>(jsonProduct);
                    string tradeNo = Fycn.Utility.HttpContext.Current.Request.Form["trade_no"];

                    //var log = LogManager.GetLogger(Startup.repository.Name, typeof(Startup));
                    //log.Info("test");
                    //log.Info(Directory.GetCurrentDirectory());
                    //log.Info(outTradeNo);
                    string jsonProduct = FileHandler.ReadFile("data/" + outTradeNo + ".wa");
                    //log.Info(jsonProduct);

                    KeyJsonModel keyJsonModel = JsonHandler.GetObjectFromJson<KeyJsonModel>(jsonProduct);
                    int result = _imachine.PostPayResultA(keyJsonModel, outTradeNo, tradeNo);
                    if (result == 1)
                    {
                        //Fycn.Utility.HttpContext.Current.Response.Write("success");
                        Response.WriteAsync("success");
                    }
                    //删除文件
                    FileHandler.DeleteFile("data/" + outTradeNo + ".wa");
                }
                return Content(1);
            }
            catch (Exception ex)
            {
                return Content(0);
            }
            
        }

        #endregion

        
        /*
        private void LogMachineData(string[] data)
        {
            File.AppendAllLines("c:/test.txt", data);
        }
         */
    }
}