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

    }
}
