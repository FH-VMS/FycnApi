using FycnApi.Base;
using Fycn.Interface;
using Fycn.Model.Sys;
using Fycn.Model.Withdraw;
using Fycn.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;


namespace FycnApi.Controllers
{
    public class TotalMoneyController : ApiBaseController
    {
        private static IBase<TotalMoneyModel> _IBase
        {
            get
            {
                return new TotalMoneyService();
            }
        }
        public ResultObj<List<TotalMoneyModel>> GetData()
        {
            // IProduct service = new ProductService();
            //List<ProductModel> products = service.GetAllProducts();
            TotalMoneyModel totalMoneyInfo = new TotalMoneyModel();

            var data = _IBase.GetAll(totalMoneyInfo);
            return Content(data);
        }
    }
}