using Fycn.Model.Sys;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using Fycn.Utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace FycnApi.Base
{
    public class ApiBaseController:Controller
    {
        protected string ContentWithJson<T>(T obj, ResultCode retCode, string retMsg)
        {
            return JsonHandler.GetJsonStrFromObject(Content(obj, retCode, retMsg));
        }

        protected ResultObj<T> Content<T>(T obj)
        {
            return Content(obj, ResultCode.Success, string.Empty);
        }

        protected ResultObj<T> Content<T>(T obj, Pagination pagination)
        {
            return Content(obj, ResultCode.Success, string.Empty, pagination);
        }
        protected ResultObj<T> Content<T>(T obj, ResultCode retCode, string retMsg)
        {
            if (String.IsNullOrEmpty(retMsg))
            {
                switch (retCode)
                {
                    case ResultCode.Success:
                        retMsg = "操作成功！";
                        break;
                }
            }
            return ResultObj<T>.GetResult(obj, retCode, retMsg);
        }

        protected ResultObj<T> Content<T>(T obj, ResultCode retCode, string retMsg, Pagination pagination)
        {
            if (String.IsNullOrEmpty(retMsg))
            {
                switch (retCode)
                {
                    case ResultCode.Success:
                        retMsg = "操作成功！";
                        break;
                }
            }
            return ResultObj<T>.GetResult(obj, retCode, retMsg, pagination);
        }
        protected static ILogger Logger
        {
            get { return LogFactory.GetInstance(); }
        }


        
    }
}
