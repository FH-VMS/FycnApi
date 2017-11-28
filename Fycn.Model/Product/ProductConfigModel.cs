using Fycn.Model.Sys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fycn.Model.Product
{
    [Table("table_product")]
    public class ProductConfigModel
    {
        [Column(Name = "wares_config_id")]
        public string WaresConfigId
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

        [Column(Name = "wares_config_name")]
        public string WaresConfigName
        {
            get;
            set;
        }

        public string WaresName
        {
            get;
            set;
        }

        [Column(Name = "low_missing")]
        public int LowMissing
        {
            get;
            set;
        }

        [Column(Name = "low_missing_alarm")]
        public int LowMissingAlarm
        {
            get;
            set;
        }

        [Column(Name = "purchase_price")]
        public decimal PurchasePrice
        {
            get;
            set;
        }

        [Column(Name = "price_unit")]
        public string PriceUnit
        {
            get;
            set;
        }

        [Column(Name = "wares_status")]
        public int WaresStatus
        {
            get;
            set;
        }

        [Column(Name = "update_date")]
        public DateTime UpdateDate
        {
            get;
            set;
        }

        [Column(Name = "wares_config_remark")]
        public string WaresConfigRemark
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
