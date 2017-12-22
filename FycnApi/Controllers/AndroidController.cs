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

namespace FycnApi.Controllers
{
    public class AndroidController : ApiBaseController
    {
        public string TestPerform(string k)
        {
            ISale _isale = new SalesService();
            //trade_status: (0:待支付，1:支付成功待出货,2：支付成功且已全部出货,3：支付成功部分出货成功未退款，4:支付成功部分出货成功已退款，5：支付成功此货道出货全部出货失败未退款，5：支付成功此货道出货全部出货失败已退款)
            List<KeyTunnelModel> lstSales = _isale.GetPayResult("", "1", k);
            if (lstSales.Count == 0)
            {
                return "";
            }
            
            return JsonHandler.GetJsonStrFromObject(lstSales, false);
        }
       public string TestSendMessage(string message)
       {
            //49 10 31 32 33 34 35 36 37 38 39 30 41 42 31 32 33 34 35 36 EE
            SocketHelper.SendMessage(message);
          return "OK";
       }

        public string LogThread()
        {
            var log = LogManager.GetLogger("FycnApi", typeof(Startup));
            //log.Info("test");
            Thread.CurrentThread.Abort();
            log.Info(Thread.CurrentThread.ManagedThreadId);
            return "Ok";
        }


        public string TestSendStrMessage()
        {
            //49 10 31 32 33 34 35 36 37 38 39 30 41 42 31 32 33 34 35 36 EE
            SocketHelper.SendStrMessageTest("","aaaaaa");
            return "OK";
        }

        public void TestRedis()
        {
            RedisHelper redisHelper = new RedisHelper(0);
            redisHelper.StringSet("2017121310330774981665", "123456789", new TimeSpan(0,2,0));
        }
        public string GetRedis()
        {
            RedisHelper redisHelper = new RedisHelper(0);
            return redisHelper.StringGet("2017121310330774981665");
        }

        public bool ExistRedis()
        {
            RedisHelper redisHelper = new RedisHelper(0);
            return redisHelper.KeyExists("2017121310330774981665");
        }

        public void TestSendCommand()
        {
            List<CommandModel> lstCommand = new List<CommandModel>();
            lstCommand.Add(new CommandModel()
            {
                Content = "XB0B17100001",
                Size = 12
            });
            lstCommand.Add(new CommandModel()
            {
                Content = "2017121204215003142952",
                Size = 22
            });
            lstCommand.Add(new CommandModel()
            {
                Content = "A0106",
                Size = 5
            });
            lstCommand.Add(new CommandModel()
            {
                Content = "4",
                Size = 1
            });
            SocketHelper.GenerateCommand(10, 41,66, lstCommand);
        }

        public string TestByte()
        {
            return ByteHelper.Ten2Hex("83").ToString();
        }
        


    }
}
