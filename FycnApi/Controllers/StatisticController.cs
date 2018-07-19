using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using FycnApi.Base;
using Fycn.Interface;
using Fycn.Service;
using Fycn.Utility;
using Fycn.Model.Sys;

namespace FycnApi.Controllers
{
    public class StatisticController : ApiBaseController
    {
        private static IStatistic _istatistic
        {
            get
            {
                return new StatisticService();
            }
        }

        public ResultObj<string> GetMobilePayStatistic(string salesDateStart, string salesDateEnd, string clientId="", string machineId="", string tradeStatus= "2^7^8")
        {
            return Content(JsonHandler.DataTable2Json(_istatistic.GetMobilePayStatistic(salesDateStart, salesDateEnd, clientId, machineId, tradeStatus)));
        }

        public ResultObj<string> GetProductStatistic(string salesDateStart, string salesDateEnd, string productName="", string clientId="", string machineId="", string tradeStatus="2^7^8",int pageIndex=1, int pageSize=10)
        {
            int count =_istatistic.GetProductStatisticCount(salesDateStart, salesDateEnd, productName, clientId, machineId, tradeStatus);
            var pagination = new Pagination { PageSize = pageSize, PageIndex = pageIndex, StartIndex = 0, TotalRows = count, TotalPage = 0 };
            return Content(JsonHandler.DataTable2Json(_istatistic.GetProductStatistic(salesDateStart, salesDateEnd, productName, clientId, machineId, tradeStatus,pageIndex,pageSize)),pagination);
        }
    }
}