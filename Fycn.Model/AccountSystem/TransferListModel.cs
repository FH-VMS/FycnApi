using Fycn.Model.Sys;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fycn.Model.AccountSystem
{
    [Table("table_transfer_fund_info")]
    public class TransferListModel
    {
        [Column(Name = "id")]
        public string Id
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

        [Column(Name = "pay_interface")]
        public string PayInterface
        {
            get;
            set;
        }

        [Column(Name = "amount")]
        public float Amount
        {
            get;
            set;
        }

        [Column(Name = "merchant_id")]
        public string MerchantId
        {
            get;
            set;
        }


        [Column(Name = "to_id")]
        public string ToId
        {
            get;
            set;
        }

        [Column(Name = "really_name")]
        public string ReallyName
        {
            get;
            set;
        }

        [Column(Name = "transfer_date")]
        public DateTime TrasferDate
        {
            get;
            set;
        }

        [Column(Name = "transfer_status")]
        public string TransferStatus
        {
            get;
            set;
        }

        [Column(Name = "payment_no")]
        public string PaymentNo
        {
            get;
            set;
        }

        [Column(Name = "desc")]
        public string Desc
        {
            get;
            set;
        }

        [Column(Name = "fy_rate")]
        public float FyRate
        {
            get;
            set;
        }

        [Column(Name = "operator")]
        public string Operator
        {
            get;
            set;
        }
    }
}
