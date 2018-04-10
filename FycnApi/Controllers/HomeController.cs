using FycnApi.Base;
using Fycn.Interface;
using Fycn.Model.Sys;
using Fycn.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using Fycn.Utility;
using Fycn.Model.Statistic;

namespace FycnApi.Controllers
{
    public class HomeController : ApiBaseController
    {
        public IEnumerable<RemoteServiceEntity> GetAllProducts()
        {
            // IProduct service = new ProductService();
            //List<ProductModel> products = service.GetAllProducts();

            var servicelist = ServiceInfoHelper.RemoteServiceList;
            return servicelist;
        }

        //机器 状态数
        public ResultObj<string> GetTotalMachineCount()
        {
            ICommon icommon = new CommonService();
            string retutStr = JsonHandler.DataTable2Json(icommon.GetTotalMachineCount());
            return Content(retutStr);
        }


        //机器 销售额
        public ResultObj<string> GetSalesAmountByMachine(string salesDateStart = "", string salesDateEnd = "",string machineId="", bool needPage = false, int pageIndex = 1, int pageSize = 10)
        {
            if (string.IsNullOrEmpty(salesDateStart) || string.IsNullOrEmpty(salesDateEnd))
            {
                return Content("");
            }
            IStatistic istatistic = new StatisticService();
            string retutStr = JsonHandler.DataTable2Json(istatistic.GetSalesAmountByMachine(salesDateStart, salesDateEnd, machineId, needPage, pageIndex, pageSize));
            if (!needPage)
            {
                return Content(retutStr);
            }

            int totalcount = istatistic.GetSalesAmountByMachineCount(salesDateStart, salesDateEnd, machineId, needPage, pageIndex, pageSize);

            var pagination = new Pagination { PageSize = pageSize, PageIndex = pageIndex, StartIndex = 0, TotalRows = totalcount, TotalPage = 0 };
            return Content(retutStr, pagination);

        }

        public ResultObj<List<ClassModel>> GetPayNumbers(string clientId="")
        {
            IStatistic istatistic = new StatisticService();
            return Content(istatistic.GetPayNumbers(clientId));
        }

        public ResultObj<List<ClassModel>> GetGroupSalesMoney(string salesDateStart = "", string salesDateEnd = "", string type = "", string clientId = "")
        {
            if (string.IsNullOrEmpty(salesDateStart)|| string.IsNullOrEmpty(salesDateEnd) || string.IsNullOrEmpty(type))
            {
                return null;
            }
            IStatistic istatistic = new StatisticService();
            return Content(istatistic.GetGroupSalesMoney( salesDateStart,  salesDateEnd,  type, clientId));
        }

        public ResultObj<List<ClassModel>> GetPayNumbersByDate(string salesDateStart = "", string salesDateEnd = "", string type = "year", string clientId = "")
        {
            if (string.IsNullOrEmpty(salesDateStart) || string.IsNullOrEmpty(salesDateEnd) || string.IsNullOrEmpty(type))
            {
                return null;
            }
            IStatistic istatistic = new StatisticService();
            return Content(istatistic.GetPayNumbersByDate(salesDateStart, salesDateEnd, type, clientId));
        }

        public ResultObj<List<ClassModel>> GetGroupProduct(string salesDateStart = "", string salesDateEnd = "", string clientId = "", bool needPage=false, int pageIndex=1, int pageSize=10)
        {
            if (string.IsNullOrEmpty(salesDateStart) || string.IsNullOrEmpty(salesDateEnd))
            {
                return null;
            }
            IStatistic istatistic = new StatisticService();
            return Content(istatistic.GetGroupProduct(salesDateStart, salesDateEnd, clientId, needPage, pageIndex,pageSize));
        }

        public ResultObj<List<ClassModel>> GetGroupMoneyByMachine(string salesDateStart = "", string salesDateEnd = "", string clientId = "", bool needPage = true, int pageIndex = 1, int pageSize = 10)
        {
            if (string.IsNullOrEmpty(salesDateStart) || string.IsNullOrEmpty(salesDateEnd))
            {
                return null;
            }
            IStatistic istatistic = new StatisticService();
            return Content(istatistic.GetGroupMoneyByMachine(salesDateStart, salesDateEnd, clientId, needPage, pageIndex, pageSize));
        }
        
    }
}
