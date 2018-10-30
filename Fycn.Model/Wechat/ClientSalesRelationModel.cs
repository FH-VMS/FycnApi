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

        [Column(Name = "pickup_code")]
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

        [Column(Name = "total_num")]
        public int TotalNum
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

        [Column(Name = "wares_id")]
        public string WaresId
        {
            get;
            set;
        }

        [Column(Name = "wares_name")]
        public string WaresName
        {
            get;
            set;
        }

        [Column(Name = "member_id")]
        public string MemberId
        {
            get;
            set;
        }

        [Column(Name = "remark")]
        public string Remark
        {
            get;
            set;
        }

        public string MachineId
        {
            get;
            set;
        }

        public string TunnelId
        {
            get;
            set;
        }

        public int CurrentStock
        {
            get;
            set;
        }

        public int CurrentStatus
        {
            get;
            set;
        }

        public string PicUrl
        {
            get;
            set;
        }
    }
}
