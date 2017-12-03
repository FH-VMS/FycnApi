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
       public string TestSendMessage(string ip , string message)
       {
           //SocketHelper.SendMessage()
          return "OK";
       }

    }
}
