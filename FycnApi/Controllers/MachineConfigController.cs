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
    public class MachineConfigController : ApiBaseController
    {

        private static IBase<MachineConfigModel> _IBase
        {
            get
            {
                return new MachineConfigService();
            }
        }

        public ResultObj<List<MachineConfigModel>> GetData(string deviceId = "", int pageIndex = 1, int pageSize = 10)
        {
            // IProduct service = new ProductService();
            //List<ProductModel> products = service.GetAllProducts();

            MachineConfigModel machineConfigInfo = new MachineConfigModel();
            machineConfigInfo.DeviceId = deviceId;
            machineConfigInfo.PageIndex = pageIndex;
            machineConfigInfo.PageSize = pageSize;
            var users = _IBase.GetAll(machineConfigInfo);
            int totalcount = _IBase.GetCount(machineConfigInfo);

            var pagination = new Pagination { PageSize = pageSize, PageIndex = pageIndex, StartIndex = 0, TotalRows = totalcount, TotalPage = 0 };
            return Content(users, pagination);
        }

        public ResultObj<int> PostData([FromBody]MachineConfigModel machineConfigInfo)
        {
            return Content(_IBase.PostData(machineConfigInfo));
        }

        public ResultObj<int> PutData([FromBody]MachineConfigModel machineConfigInfo)
        {
            return Content(_IBase.UpdateData(machineConfigInfo));
        }

        public ResultObj<int> DeleteData(string idList)
        {
            return Content(_IBase.DeleteData(idList));
        }

        
    }
}
