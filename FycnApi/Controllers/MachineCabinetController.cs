using FycnApi.Base;
using Fycn.Interface;
using Fycn.Model.Machine;
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
    public class MachineCabinetController : ApiBaseController
    {
        private static IBase<MachineCabinetModel> _IBase
        {
            get
            {
                return new MachineCabinetService();
            }
        }

        public ResultObj<List<MachineCabinetModel>> GetData(string cabinetName = "", int pageIndex = 1, int pageSize = 10)
        {
            // IProduct service = new ProductService();
            //List<ProductModel> products = service.GetAllProducts();

            MachineCabinetModel machineCabinetInfo = new MachineCabinetModel();
            machineCabinetInfo.CabinetName = cabinetName;
            machineCabinetInfo.PageIndex = pageIndex;
            machineCabinetInfo.PageSize = pageSize;
            var users = _IBase.GetAll(machineCabinetInfo);
            int totalcount = _IBase.GetCount(machineCabinetInfo);

            var pagination = new Pagination { PageSize = pageSize, PageIndex = pageIndex, StartIndex = 0, TotalRows = totalcount, TotalPage = 0 };
            return Content(users, pagination);
        }

        public ResultObj<int> PostData([FromBody]MachineCabinetModel machineCabinetInfo)
        {
            return Content(_IBase.PostData(machineCabinetInfo));
        }

        public ResultObj<int> PutData([FromBody]MachineCabinetModel machineCabinetInfo)
        {
            return Content(_IBase.UpdateData(machineCabinetInfo));
        }

        public ResultObj<int> DeleteData(string idList)
        {
            return Content(_IBase.DeleteData(idList));
        }

        
    }
}