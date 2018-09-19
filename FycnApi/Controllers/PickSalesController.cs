using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using FycnApi.Base;
using Fycn.Interface;
using Fycn.Model.Sale;
using Fycn.Model.Sys;
using Fycn.Service;

namespace FycnApi.Controllers
{
    public class PickSalesController : ApiBaseController
    {
        private static IBase<SaleModel> _IBase
        {
            get
            {
                return new PickSalesService();
            }
        }

        public ResultObj<List<SaleModel>> GetData(string machineId = "", string pickupCode = "", string tradeStatus = "", string salesDate = "", string tradeNo = "", int pageIndex = 1, int pageSize = 10)
        {
            // IProduct service = new ProductService();
            //List<ProductModel> products = service.GetAllProducts();

            SaleModel saleInfo = new SaleModel();
            saleInfo.MachineId = machineId;
            saleInfo.PickupCode = pickupCode;
            saleInfo.TradeNo = tradeNo;
            if (!string.IsNullOrEmpty(tradeStatus))
            {
                saleInfo.TradeStatus = Convert.ToInt32(tradeStatus);
            }

            if (!string.IsNullOrEmpty(salesDate))
            {
                saleInfo.SaleDateStart = salesDate.Split('^')[0];
                saleInfo.SaleDateEnd = salesDate.Split('^')[1];
            }

            saleInfo.PageIndex = pageIndex;
            saleInfo.PageSize = pageSize;
            var sales = _IBase.GetAll(saleInfo);
            int totalcount = _IBase.GetCount(saleInfo);

            var pagination = new Pagination { PageSize = pageSize, PageIndex = pageIndex, StartIndex = 0, TotalRows = totalcount, TotalPage = 0 };
            return Content(sales, pagination);
        }
    }
}