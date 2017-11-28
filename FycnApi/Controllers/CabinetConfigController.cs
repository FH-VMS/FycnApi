using FycnApi.Base;
using Fycn.Interface;
using Fycn.Model.Common;
using Fycn.Model.Machine;
using Fycn.Model.Sys;
using Fycn.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;


namespace FycnApi.Controllers
{
    public class CabinetConfigController : ApiBaseController
    {
        public ResultObj<List<CabinetConfigModel>> GetCabinetByMachineTypeId(string machineTypeId)
        {
            ICabinet cabinetService = new CabinetService();
            var cabinetList = cabinetService.GetCabinetByMachineTypeId(machineTypeId);
            return Content(cabinetList);
        }

        public ResultObj<List<CommonDic>> GetCabinetByMachineId(string machineId)
        {
            ICabinet cabinetService = new CabinetService();
            var cabinetList = cabinetService.GetCabinetByMachineId(machineId);
            return Content(cabinetList);
        }
    }
}
