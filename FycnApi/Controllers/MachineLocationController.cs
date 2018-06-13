using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using FycnApi.Base;
using Fycn.Model.Machine;
using Fycn.Interface;
using Fycn.Service;
using Fycn.Model.Sys;

namespace FycnApi.Controllers
{
    public class MachineLocationController : ApiBaseController
    {
        private static IBase<MachineLocationModel> _IBase
        {
            get
            {
                return new MachineLocationService();
            }
        }

        public ResultObj<List<MachineLocationModel>> GetData(string machineId = "",string startLong="", string endLong="",string startLati="", string endLati="", int pageIndex = 1, int pageSize = 10)
        {
            // IProduct service = new ProductService();
            //List<ProductModel> products = service.GetAllProducts();

            MachineLocationModel machineLocationInfo = new MachineLocationModel();
            machineLocationInfo.MachineId = machineId;
            machineLocationInfo.StartLong = startLong;
            machineLocationInfo.EndLong = endLong;
            machineLocationInfo.StartLati = startLati;
            machineLocationInfo.EndLati = endLati;
            machineLocationInfo.PageIndex = pageIndex;
            machineLocationInfo.PageSize = pageSize;
            var data = _IBase.GetAll(machineLocationInfo);
            //int totalcount = 0;
            /*
            if (string.IsNullOrEmpty(machineId))
            {
                totalcount = _IBase.GetCount(machineLocationInfo);
            }
            */

            //var pagination = new Pagination { PageSize = pageSize, PageIndex = pageIndex, StartIndex = 0, TotalRows = totalcount, TotalPage = 0 };
            return Content(data);
        }

        public ResultObj<int> PostData([FromBody]MachineLocationModel machineLocationInfo)
        {
            machineLocationInfo.Id = Guid.NewGuid().ToString();
             return Content(_IBase.PostData(machineLocationInfo));
        }

        public ResultObj<int> PutData([FromBody]MachineLocationModel machineLocationInfo)
        {
            return Content(_IBase.UpdateData(machineLocationInfo));
        }

        public ResultObj<int> DeleteData(string idList)
        {
            return Content(_IBase.DeleteData(idList));
        }

        public ResultObj<MachineLocationModel> GetLocationByMachineId(string machineId)
        {
            if(string.IsNullOrEmpty(machineId))
            {
                return Content(new MachineLocationModel());
            }
            MachineLocationModel machineLocationInfo = new MachineLocationModel();
            machineLocationInfo.MachineId = machineId;
            var lst = _IBase.GetAll(machineLocationInfo);
            if(lst.Count==0)
            {
                return Content(new MachineLocationModel());
            }
            else
            {
                return Content(lst[0]);
            }
        }

    }
}