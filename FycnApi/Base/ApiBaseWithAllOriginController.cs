using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Fycn.Model.Sys;
using Fycn.Utility;
using Microsoft.AspNetCore.Cors;

namespace FycnApi.Base
{
    [EnableCors("AllowAllOrigin")]
    public class ApiBaseWithAllOriginController : Controller
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