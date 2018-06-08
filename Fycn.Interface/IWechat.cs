using Fycn.Model.Pay;
using Fycn.Model.Privilege;
using Fycn.Model.Product;
using Fycn.Model.Sale;
using Fycn.Model.Sys;
using Fycn.Model.Wechat;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fycn.Interface
{
    public interface IWechat
    {
        [Remark("新建会员", ParmsNote = "会员实体", ReturnNote = "int")]
        int CreateMember(WechatMemberModel memberInfo);

        [Remark("判断该客户是否存在该会员", ParmsNote = "会员实体", ReturnNote = "int")]
        List<WechatMemberModel> IsExistMember(WechatMemberModel clientMemberInfo);

        [Remark("根据客户id取对应商品类型", ParmsNote = "客户id", ReturnNote = "实体列表")]
        List<ProductTypeModel> GetProdcutTypeByClientId(string clientId);

        [Remark("根据商品类型取商品", ParmsNote = "类型id,商户id", ReturnNote = "实体列表")]
        List<ProductListModel> GetProdcutByTypeAndClient(string typeId, string clientId);

        [Remark("根据商品ids取列表", ParmsNote = "商户ids", ReturnNote = "实体列表")]
        List<ProductListModel> GetWechatProductInfo(string waresIds);

        [Remark("根据商品ids取商品和商品组列表", ParmsNote = "商户ids", ReturnNote = "实体列表")]
        List<ProductListModel> GetProdcutAndGroupList(string waresIds, string waresGroupIds);

        [Remark("根据opnid取历史订单", ParmsNote = "会员id", ReturnNote = "实体列表")]
        List<SaleModel> GetHistorySalesList(string openId, int pageIndex, int pageSize);

        [Remark("根据opnid取待取货订单", ParmsNote = "会员id", ReturnNote = "实体列表")]
        List<ClientSalesRelationModel> GetWaitingSalesList(string openId);

        [Remark("微信公众号支付通知", ParmsNote = "", ReturnNote = "")]
        int PostPayResultW(List<ProductPayModel> lstProductPay, string sellerId, string buyerId, string isConcern, string payDate,string clientId);

        [Remark("获取符合条件的活动券", ParmsNote = "", ReturnNote = "")]
        List<PrivilegeModel> GetActivityPrivilegeList(PrivilegeModel privilegeInfo);

        [Remark("会员领取优惠券", ParmsNote = "", ReturnNote = "")]
        int PostTicket(PrivilegeMemberRelationModel privilegeMemberInfo);

        [Remark("避免重复领取", ParmsNote = "", ReturnNote = "")]
        int IsExistTicket(PrivilegeMemberRelationModel privilegeMemberInfo);

        [Remark("取得会员的优惠券", ParmsNote = "", ReturnNote = "")]
        List<PrivilegeMemberRelationModel> GetPrivilegeByMemberId(PrivilegeMemberRelationModel privilegeMemberInfo);

        [Remark("取得会员的优惠券总数", ParmsNote = "", ReturnNote = "")]
        int GetPrivilegeCountByMemberId(PrivilegeMemberRelationModel privilegeMemberInfo);

        [Remark("取得会员可领优惠券的次数", ParmsNote = "", ReturnNote = "")]
        int GetCanTakeTicketCount(PrivilegeMemberRelationModel privilegeMemberInfo);

        [Remark("取可用优惠券", ParmsNote = "", ReturnNote = "")]
        List<PrivilegeMemberRelationModel> GetCanUsePrivilege(PrivilegeMemberRelationModel privilegeMemberInfo, string privilegeIds,ref decimal totalFee, List<ProductPayModel> lstPayInfo);
    }
}
