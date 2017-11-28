using FycnApi.Base;
using Fycn.Interface;
using Fycn.Model.Machine;
using Fycn.Model.Sys;
using Fycn.Service;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace FycnApi.Controllers
{
    public class TunnelConfigController : ApiBaseController
    {

        private static ITunnel _IBase
        {
            get
            {
                return new TunnelConfigService();
            }
        }

        public ResultObj<List<TunnelConfigModel>> GetData(string machineId = "", string cabinetId = "")
        {
            // IProduct service = new ProductService();
            //List<ProductModel> products = service.GetAllProducts();

            TunnelConfigModel tunnelConfigInfo = new TunnelConfigModel();
            tunnelConfigInfo.MachineId = machineId;
            tunnelConfigInfo.CabinetId = cabinetId;
            var tunnels = _IBase.GetAll(tunnelConfigInfo);
            return Content(tunnels);
        }

        public ResultObj<int> PostData([FromBody]List<TunnelConfigModel> tunnelConfigInfo)
        {
            return Content(_IBase.PostData(tunnelConfigInfo));
        }

        public ResultObj<int> PutData([FromBody]TunnelConfigModel tunnelConfigInfo)
        {
            return Content(_IBase.UpdateData(tunnelConfigInfo));
        }

        public ResultObj<int> DeleteData(string idList)
        {
            return Content(_IBase.DeleteData(idList));
        }
    }
}
