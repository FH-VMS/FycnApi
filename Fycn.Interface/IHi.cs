using Fycn.Model.Machine;
using Fycn.Model.Pay;
using Fycn.Model.Privilege;
using Fycn.Model.Sale;
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

        [Remark("根据机器id取判断是否支持一元嗨", ParmsNote = "", ReturnNote = "")]
        List<ActivityPrivilegeRelationModel> IsSupportActivity(string machineId);

        [Remark("订单结果插入表中", ParmsNote = "", ReturnNote = "")]
        int PostPayResultW(KeyJsonModel keyJsonModel, string tradeNo, string sellerId, string buyerId, string isConcern, string payDate);

        [Remark("嗨的结果插入表中", ParmsNote = "", ReturnNote = "")]
        int DoReward(KeyJsonModel keyJsonModel, string tradeNo, string memberId, bool isGoal);

        [Remark("根据单号查询单号状态", ParmsNote = "", ReturnNote = "")]
        List<SaleModel> GetTradeStatusByTradeNo(string tradeNo);
    }
}
