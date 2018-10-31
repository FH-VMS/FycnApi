using Fycn.Model.Ad;
using Fycn.Model.Machine;
using Fycn.Model.Member;
using Fycn.Model.Pay;
using Fycn.Model.Privilege;
using Fycn.Model.Sale;
using Fycn.Model.Sys;
using Fycn.Model.Wechat;
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
        int DoReward(KeyJsonModel keyJsonModel, string tradeNo, string memberId,string waresName, bool isGoal);

        [Remark("根据单号查询单号状态", ParmsNote = "", ReturnNote = "")]
        List<SaleModel> GetTradeStatusByTradeNo(string tradeNo);

        [Remark("取广告资源", ParmsNote = "", ReturnNote = "")]
        List<SourceToMachineModel> GetAdSource(string machineId, string adType);

        [Remark("取货卡列表", ParmsNote = "", ReturnNote = "")]
        List<ClientSalesRelationModel> GetWaitingPickupByMachine(string machineId, string openId);

        [Remark("立即取货验证", ParmsNote = "", ReturnNote = "")]
        List<ClientSalesRelationModel> VerifyPickupByTradeNo(string tradeNo);

        [Remark("根据商品id取出对应的货道", ParmsNote = "", ReturnNote = "")]
        List<ProductModel> GetProducInfoByWaresId(string machineId, string waresId);

        [Remark("根据会员id取对应客户的账户信息", ParmsNote = "", ReturnNote = "")]
        MemberAccountModel GetMemberAccountByMember(string memberId, string clientId);

        [Remark("根据机器id取机器信息", ParmsNote = "", ReturnNote = "")]
        List<MachineListModel> GetMachineByMachineId(string machineId);
    }
}
