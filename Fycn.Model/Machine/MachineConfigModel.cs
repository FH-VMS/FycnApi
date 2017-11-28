using Fycn.Model.Sys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fycn.Model.Machine
{
    [Table("table_machine_config")]
    public class MachineConfigModel
    {
        [Column(Name = "machine_id")]
        public string MachineId
        {
            get;
            set;
        }

        public string MachineIdB
        {
            get;
            set;
        }

        [Column(Name = "mc_status")]
        public int McStatus
        {
            get;
            set;
        }

        [Column(Name = "mc_activity")]
        public string McActivity
        {
            get;
            set;
        }
        [Column(Name = "mc_alipay_enable")]
        public int McAlipayEnable
        {
            get;
            set;
        }
        [Column(Name = "mc_wpay_enable")]
        public int McWpayEnable
        {
            get;
            set;
        }
        [Column(Name = "mc_billpay_enable")]
        public int McBillpayEnable
        {
            get;
            set;
        }
        [Column(Name = "mc_billchange_enable")]
        public int McBillchangeEnable
        {
            get;
            set;
        }
        [Column(Name = "mc_coinpay_enable")]
        public int McCoinpayEnable
        {
            get;
            set;
        }
        [Column(Name = "mc_coinchange_enable")]
        public int McCoinchangeEnable
        {
            get;
            set;
        }
        [Column(Name = "mc_upay_enable")]
        public int McUpayEnable
        {
            get;
            set;
        }
        [Column(Name = "mc_icpay_enable")]
        public int McIcpayEnable
        {
            get;
            set;
        }
        [Column(Name = "mc_lift_enable")]
        public int McLiftEnable
        {
            get;
            set;
        }
        [Column(Name = "mc_infrared_enable")]
        public int McInfraredEnable
        {
            get;
            set;
        }
        [Column(Name = "mc_area1_temp")]
        public string McArea1Temp
        {
            get;
            set;
        }
        [Column(Name = "mc_area2_temp")]
        public string McArea2Temp
        {
            get;
            set;
        }
        [Column(Name = "mc_area3_temp")]
        public string McArea3Temp
        {
            get;
            set;
        }
        [Column(Name = "mc_area4_temp")]
        public string McArea4Temp
        {
            get;
            set;
        }
        [Column(Name = "mc_goods_used")]
        public string McGoodsUsed
        {
            get;
            set;
        }
        [Column(Name = "mc_longitude")]
        public string McLongitude
        {
            get;
            set;
        }
        [Column(Name = "mc_dimension")]
        public string McDimension
        {
            get;
            set;
        }
        [Column(Name = "updater")]
        public string Updater
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
        [Column(Name = "remark")]
        public string Remark
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

        public string DeviceId
        {
            get;
            set;
        }
    }
}
