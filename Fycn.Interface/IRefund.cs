using Fycn.Model.Refund;
using Fycn.Model.Sale;
using Fycn.Model.Sys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fycn.Interface
{
    public interface IRefund
    {
        [Remark("取出货失败的接口", ParmsNote = "", ReturnNote = "")]
        List<SaleModel> GetRefundOrder(List<string> lstTradeNo);

        [Remark("更新退款结果", ParmsNote = "", ReturnNote = "")]
        int UpdateRefundResult(SaleModel saleInfo);

         [Remark("插入退款详情", ParmsNote = "", ReturnNote = "")]
        int PostRefundDetail(RefundModel refundInfo);

         [Remark("阿里退款通知更新订单状态", ParmsNote = "", ReturnNote = "")]
         void UpdateOrderStatusForAli(string comId);

         [Remark("判断阿里退款是否成功", ParmsNote = "", ReturnNote = "")]
         int IsRefundSucceed(string tradeNo);
    }
}
