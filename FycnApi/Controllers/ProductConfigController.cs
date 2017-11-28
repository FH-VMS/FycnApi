using FycnApi.Base;
using Fycn.Interface;
using Fycn.Model.Product;
using Fycn.Model.Sys;
using Fycn.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc;

namespace FycnApi.Controllers
{
    public class ProductConfigController : ApiBaseController
    {
        private static IBase<ProductConfigModel> _IBase
        {
            get
            {
                return new ProductConfigService();
            }
        }

        public ResultObj<List<ProductConfigModel>> GetData(string waresName = "", int pageIndex = 1, int pageSize = 10)
        {
            // IProduct service = new ProductService();
            //List<ProductModel> products = service.GetAllProducts();

            ProductConfigModel productConfigInfo = new ProductConfigModel();
            productConfigInfo.WaresConfigName = waresName;
            productConfigInfo.PageIndex = pageIndex;
            productConfigInfo.PageSize = pageSize;
            var users = _IBase.GetAll(productConfigInfo);
            int totalcount = _IBase.GetCount(productConfigInfo);

            var pagination = new Pagination { PageSize = pageSize, PageIndex = pageIndex, StartIndex = 0, TotalRows = totalcount, TotalPage = 0 };
            return Content(users, pagination);
        }

        public ResultObj<int> PostData([FromBody]ProductConfigModel productConfigInfo)
        {
            return Content(_IBase.PostData(productConfigInfo));
        }

        public ResultObj<int> PutData([FromBody]ProductConfigModel productConfigInfo)
        {
            return Content(_IBase.UpdateData(productConfigInfo));
        }

        public ResultObj<int> DeleteData(string idList)
        {
            return Content(_IBase.DeleteData(idList));
        }
    }
}