using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fycn.Model.Sys
{
    public class ResultObj<T>
    {
        public ResultCode RetCode { get; set; }

        public string RetMsg { get; set; }

        public T RetObj { get; set; }

        public Pagination RetPagination { get; set; }
        public static ResultObj<T> GetResult(T retObj, ResultCode retCode, string retMsg)
        {
            return new ResultObj<T>() { RetCode = retCode, RetMsg = retMsg, RetObj = retObj == null ? default(T) : retObj };
        }

        public static ResultObj<T> GetResult(T retObj, ResultCode retCode, string retMsg, Pagination retPagination)
        {
            return new ResultObj<T>() { RetCode = retCode, RetMsg = retMsg, RetObj = retObj == null ? default(T) : retObj, RetPagination = retPagination };
        }
    }

    public enum ResultCode
    {
        Success = 1,
        Fail = 20,
        ParamsNull = 30,
        NoAccess = 100,
        ErrorToken = 101,

        Exception = 200,
        IoException = 300,
        BizException = 400
    }
}
