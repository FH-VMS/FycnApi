using Fycn.Model.Sys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fycn.Model.Refund
{
    [Table("table_refund")]
    public class RefundModel
    {
        [Column(Name = "out_trade_no")]
        public string OutTradeNo
        {
            get;
            set;
        }

        [Column(Name = "trade_no")]
        public string TradeNo
        {
            get;
            set;
        }

        [Column(Name = "refund_no")]
        public string RefundNo
        {
            get;
            set;
        }


        [Column(Name = "refund_detail")]
        public string RefundDetail
        {
            get;
            set;
        }
    }
}
