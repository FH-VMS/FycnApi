using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using FycnApi.Base;
using Fycn.Service;
using Fycn.Model.Product;
using Fycn.Interface;
using Fycn.Model.Sys;

namespace FycnApi.Controllers
{
    public class ProductTypeController : ApiBaseController
    {
        private static IBase<ProductTypeModel> _IBase
        {
            get
            {
                return new ProductTypeService();
            }
        }

        public ResultObj<List<ProductTypeModel>> GetData(string waresTypeName, string clientName, int pageIndex = 1, int pageSize = 10)
        {
            ProductTypeModel productTypeInfo = new ProductTypeModel();
            productTypeInfo.WaresTypeName = waresTypeName;
            productTypeInfo.ClientName = clientName;
            productTypeInfo.PageIndex = pageIndex;
            productTypeInfo.PageSize = pageSize;
            int totalcount = _IBase.GetCount(productTypeInfo);

            var pagination = new Pagination { PageSize = pageSize, PageIndex = pageIndex, StartIndex = 0, TotalRows = totalcount, TotalPage = 0 };

            var products = _IBase.GetAll(productTypeInfo);
            return Content(products,pagination);
        }

        public ResultObj<int> PostData([FromBody]ProductTypeModel productTypeInfo)
        {
            return Content(_IBase.PostData(productTypeInfo));
        }

        public ResultObj<int> PutData([FromBody]ProductTypeModel productTypeInfo)
        {
            return Content(_IBase.UpdateData(productTypeInfo));
        }

        public ResultObj<int> DeleteData(string idList)
        {
            return Content(_IBase.DeleteData(idList));
        }
        
    }
}