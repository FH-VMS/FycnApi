using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using FycnApi.Base;
using Fycn.Interface;
using Fycn.Model.Product;
using Fycn.Service;
using Fycn.Model.Sys;

namespace FycnApi.Controllers
{
    public class ProductGroupController : ApiBaseController
    {
        private static IBase<ProductGroupModel> _IBase
        {
            get
            {
                return new ProductGroupService();
            }
        }

        public ResultObj<List<ProductGroupModel>> GetData(string waresName = "", string waresTypeId = "", int pageIndex = 1, int pageSize = 10)
        {
            // IProduct service = new ProductService();
            //List<ProductModel> products = service.GetAllProducts();

            ProductGroupModel productListInfo = new ProductGroupModel();
            productListInfo.WaresName = waresName;
            productListInfo.WaresTypeId = waresTypeId;
            productListInfo.PageIndex = pageIndex;
            productListInfo.PageSize = pageSize;
            var users = _IBase.GetAll(productListInfo);
            int totalcount = _IBase.GetCount(productListInfo);

            var pagination = new Pagination { PageSize = pageSize, PageIndex = pageIndex, StartIndex = 0, TotalRows = totalcount, TotalPage = 0 };
            return Content(users, pagination);
        }

        public ResultObj<int> PostData([FromBody]ProductGroupModel productListInfo)
        {
            return Content(_IBase.PostData(productListInfo));
        }

        public ResultObj<int> PutData([FromBody]ProductGroupModel productListInfo)
        {
            return Content(_IBase.UpdateData(productListInfo));
        }

        public ResultObj<int> DeleteData(string idList)
        {
            return Content(_IBase.DeleteData(idList));
        }

        public ResultObj<List<ProductGroupRelationModel>> GetProductRelationById(string waresGroupId)
        {
            if(string.IsNullOrEmpty(waresGroupId))
            {
                return Content(new List<ProductGroupRelationModel>());
            }
            IBase<ProductGroupRelationModel> baseRelation = new ProductGroupRelationService();
            ProductGroupRelationModel relationInfo = new ProductGroupRelationModel();
            relationInfo.WaresGroupId = waresGroupId;
            return Content(baseRelation.GetAll(relationInfo));
        }
    }
}