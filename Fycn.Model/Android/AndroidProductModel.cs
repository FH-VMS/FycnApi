using Fycn.Model.Sys;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fycn.Model.Android
{
    [Table("table_goods_config")]
    public class AndroidProductModel
    {
        [Column(Name = "machine_id")]
        public string MachineId
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

        [Column(Name = "tunnel_id")]
        public string TunnelId
        {
            get;
            set;
        }

        [Column(Name = "alipay_prices")]
        public decimal APrice //支付宝价格
        {
            get;
            set;
        }

        [Column(Name = "wpay_prices")]
        public decimal WPrice  //微信价格
        {
            get;
            set;
        }

        public string WaresTypeName
        {
            get;
            set;
        }

        public string WaresTypeId
        {
            get;
            set;
        }

        public string WaresName
        {
            get;
            set;
        }

        public string PicUrl
        {
            get;
            set;
        }

        public string CurrStock
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
