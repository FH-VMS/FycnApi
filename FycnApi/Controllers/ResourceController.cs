using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using FycnApi.Base;
using Fycn.Model.Resource;
using Fycn.Interface;
using Fycn.Service;
using Fycn.Model.Sys;

namespace FycnApi.Controllers
{
    public class ResourceController : ApiBaseController
    {
        private static IBase<PictureModel> _IBase
        {
            get
            {
                return new PictureService();
            }
        }

        public ResultObj<List<PictureModel>> GetData(string fileType="",string belong="", int pageIndex = 1, int pageSize = 10)
        {
            // IProduct service = new ProductService();
            //List<ProductModel> products = service.GetAllProducts();

            PictureModel picInfo = new PictureModel();
            picInfo.FileType = fileType;
            picInfo.Belong = belong;
            picInfo.PageIndex = pageIndex;
            picInfo.PageSize = pageSize;
            var resources = _IBase.GetAll(picInfo);
            int totalcount = _IBase.GetCount(picInfo);

            var pagination = new Pagination { PageSize = pageSize, PageIndex = pageIndex, StartIndex = 0, TotalRows = totalcount, TotalPage = 0 };
            return Content(resources, pagination);
        }

        public ResultObj<int> PostData([FromBody]PictureModel picInfo)
        {
            return Content(_IBase.PostData(picInfo));
        }

        public ResultObj<int> PutData([FromBody]PictureModel picInfo)
        {
            return Content(_IBase.UpdateData(picInfo));
        }

        public ResultObj<int> DeleteData(string idList)
        {
            return Content(_IBase.DeleteData(idList));
        }
    }
}