using Fycn.Model.Machine;
using Fycn.Model.Sys;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Fycn.Interface
{
    public interface IFullfilBill
    {
         [Remark("生成补货单", ParmsNote = "实体", ReturnNote = "返回实体列表")]
        List<TunnelInfoModel> GetFullfilAll(TunnelInfoModel tunnelInfoInfo);

         [Remark("生成补货单的数量", ParmsNote = "", ReturnNote = "int")]
        int GetFullfilCount(TunnelInfoModel tunnelInfoInfo);

         [Remark("手机端补充库存", ParmsNote = "列表实体", ReturnNote = "int")]
         int UpdateStockWithMobile(List<TunnelInfoModel> lstTunnelInfo);

         [Remark("按产品导出", ParmsNote = "机器编号", ReturnNote = "datatable")]
        MicroDataTable ExportByProduct(string machineId);

         [Remark("按货道导出", ParmsNote = "机器编号", ReturnNote = "datatable")]
        MicroDataTable ExportByTunnel(string machineId);
    }
}
