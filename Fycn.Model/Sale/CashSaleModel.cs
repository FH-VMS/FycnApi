using Fycn.Model.Sys;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fycn.Model.Sale
{
    [Table("table_sales_cashless")]
    public class CashSaleModel
    {
        [Column(Name = "sales_no")]
        public string SalesNo
        {
            get;
            set;
        }

        [Column(Name = "machine_id")]
        public string MachineId
        {
            get;
            set;
        }

        [Column(Name = "sales_date")]
        public DateTime SalesDate
        {
            get;
            set;
        }

        [Column(Name = "sales_type")]
        public string SalesType
        {
            get;
            set;
        }

        [Column(Name = "goods_id")]
        public string GoodsId
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

        [Column(Name = "sales_number")]
        public int SalesNumber
        {
            get;
            set;
        }

        [Column(Name = "sales_prices")]
        public string SalesPrices
        {
            get;
            set;
        }

        [Column(Name = "pay_way")]
        public string PayWay
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

        public int PageIndex
        {
            get;
            set;
        }

        public int PageSize
        {
            get;
            set;
        }
    }
}
