using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using FycnApi.Base;
using Fycn.Model.Sale;
using Fycn.Interface;
using Fycn.Service;
using Fycn.Model.Sys;

namespace FycnApi.Controllers
{
    public class SaleCashController : ApiBaseController
    {
        private static IBase<CashSaleModel> _IBase
        {
            get
            {
                return new CashSaleService();
            }
        }

        public ResultObj<List<CashSaleModel>> GetData(string machineId = "", string salesDate = "", string tradeNo = "", int pageIndex = 1, int pageSize = 10)
        {
            // IProduct service = new ProductService();
            //List<ProductModel> products = service.GetAllProducts();

            CashSaleModel saleInfo = new CashSaleModel();
            saleInfo.MachineId = machineId;
            saleInfo.TradeNo = tradeNo;

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