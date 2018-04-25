using Fycn.Model.Sys;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fycn.Model.Wechat
{
    [Table("table_client_sales_relation")]
    public class ClientSalesRelationModel
    {
        [Column(Name = "client_id")]
        public string ClientId
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

        [Column(Name = "pickup_no")]
        public string PickupNo
        {
            get;
            set;
        }

        [Column(Name = "code_status")]
        public int CodeStatus
        {
            get;
            set;
        }

        [Column(Name = "create_date")]
        public DateTime CreateDate
        {
            get;
            set;
        }

        [Column(Name = "end_date")]
        public DateTime EndDate
        {
            get;
            set;
        }
    }
}
