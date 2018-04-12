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

        public string GetMobilePayStatistic(string salesDateStart, string salesDateEnd, string clientId="", string machineId="", string tradeStatus="2")
        {
            return JsonHandler.DataTable2Json(_istatistic.GetMobilePayStatistic(salesDateStart, salesDateEnd, clientId, machineId, tradeStatus));
        }

        public string GetProductStatistic(string salesDateStart, string salesDateEnd, string productName="", string clientId="", string machineId="", string tradeStatus="2")
        {
            return JsonHandler.DataTable2Json(_istatistic.GetProductStatistic(salesDateStart, salesDateEnd, productName, clientId, machineId, tradeStatus));
        }
    }
}