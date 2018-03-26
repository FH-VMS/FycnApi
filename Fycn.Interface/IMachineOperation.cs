using Fycn.Model.Common;
using Fycn.Model.Sys;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fycn.Interface
{
    public interface IMachineOperation
    {
        [Remark("取机器列表", ParmsNote = "", ReturnNote = "返回机器列表")]
        List<CommonDic> GetMachines(CommonDic commonDic, int pageIndex, int pageSize);

        [Remark("取机器列表数量", ParmsNote = "", ReturnNote = "返回机器列表数量")]
        int GetMachinesCount(CommonDic commonDic, int pageIndex, int pageSize);

        [Remark("复制机器", ParmsNote = "", ReturnNote = "")]
        int CopyOneMachine(string oldMachineId, string newMachineId, List<string> copyItem);
    }
}
