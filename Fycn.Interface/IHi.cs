using Fycn.Model.Machine;
using Fycn.Model.Sys;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fycn.Interface
{
    public interface IHi
    {
        [Remark("根据机器id取地址", ParmsNote = "", ReturnNote = "")]
        List<MachineLocationModel> GetMachineLocationById(string machineId);
    }
}
