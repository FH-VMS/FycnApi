using Fycn.Model.Common;
using Fycn.Model.Machine;
using Fycn.Model.Sys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fycn.Interface
{
    public interface ICabinet
    {
         [Remark("根据机型取货柜", ParmsNote = "", ReturnNote = "返回返页列表")]
        List<CabinetConfigModel> GetCabinetByMachineTypeId(string machineTypeId);

         [Remark("根据机型取货柜", ParmsNote = "", ReturnNote = "返回返页列表")]
         List<CommonDic> GetCabinetByMachineId(string machineId);
    }
}
