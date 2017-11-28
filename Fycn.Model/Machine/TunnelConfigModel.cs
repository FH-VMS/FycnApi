using Fycn.Model.Sys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fycn.Model.Machine
{
    [Table("table_goods_config")]
    public class TunnelConfigModel
    {
        [Column(Name = "machine_id")]
        public string MachineId
        {
            get;
            set;
        }

        [Column(Name = "cabinet_id")]
        public string CabinetId
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

        [Column(Name = "max_puts")]
        public int MaxPuts
        {
            get;
            set;
        }

        [Column(Name = "cash_prices")]
        public decimal CashPrices
        {
            get;
            set;
        }

        [Column(Name = "wpay_prices")]
        public decimal WpayPrices
        {
            get;
            set;
        }

        [Column(Name = "alipay_prices")]
        public decimal AlipayPrices
        {
            get;
            set;
        }

        [Column(Name = "ic_prices")]
        public decimal IcPrices
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

        [Column(Name = "is_used")]
        public int IsUsed
        {
            get;
            set;
        }

         [Column(Name = "tunnel_position")]
        public string TunnelPosition
        {
            get;
            set;
        }
    }
}
