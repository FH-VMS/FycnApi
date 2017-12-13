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

namespace FycnApi.Controllers
{
    public class AndroidController : ApiBaseController
    {
       public string TestSendMessage(string message)
       {
            //49 10 31 32 33 34 35 36 37 38 39 30 41 42 31 32 33 34 35 36 EE
            SocketHelper.SendMessage(message);
          return "OK";
       }


        public string TestSendStrMessage(string ip,string message)
        {
            //49 10 31 32 33 34 35 36 37 38 39 30 41 42 31 32 33 34 35 36 EE
            SocketHelper.SendStrMessageTest(ip,message);
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

        public void TestByte()
        {
            char x = (char)88;
            ByteHelper.GenerateRealityData(ByteHelper.strToToTenByte("584230423137313030303031"), "stringval");
        }
    }
}
