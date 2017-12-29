using Fycn.Model.Machine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fycn.Utility;

namespace Fycn.Service
{
    public class OperationLogService : AbstractService
    {
        /// <summary>
        /// 插入操作日志
        /// </summary>
        /// <param name="operationLogInfo"></param>
        /// <returns></returns>
        public int PostData(OperationLogModel operationLogInfo)
        {
            return 1;
            /*
            try
            {
                int result;
                string operatorVal = "机器端";
                if (HttpContextHandler.GetHeaderObj("UserAccount") != null)
                {
                    operatorVal = HttpContextHandler.GetHeaderObj("UserAccount").ToString();
                }
                operationLogInfo.Operator = operatorVal;
                operationLogInfo.OptDate = DateTime.Now;
                result = GenerateDal.Create(operationLogInfo);
                return result;
            }
            catch (Exception e)
            {
                return 0;
            }
            */
        }
    }
}
