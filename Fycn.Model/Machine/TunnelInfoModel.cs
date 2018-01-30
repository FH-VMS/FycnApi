using Fycn.Model.Sys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fycn.Model.Machine
{
    [Table("table_goods_status")]
    public class TunnelInfoModel
    {
        [Column(Name = "goods_stu_id")]
        public string GoodsStuId
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

        public string TunnelId
        {
            get;
            set;
        }

        [Column(Name = "curr_stock")]
        public int CurrStock
        {
            get;
            set;
        }

        [Column(Name = "curr_missing")]
        public int CurrMissing
        {
            get;
            set;
        }

        [Column(Name = "fault_code")]
        public string FaultCode
        {
            get;
            set;
        }

        [Column(Name = "fault_describe")]
        public string FaultDescribe
        {
            get;
            set;
        }

        [Column(Name = "curr_status")]
        public string CurrStatus
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

        [Column(Name = "cabinet_id")]
        public string CabinetId
        {
            get;
            set;
        }

        public string WaresId
        {
            get;
            set;
        }

        public string ProductName
        {
            get;
            set;
        }

        public string MaxPuts
        {
            get;
            set;
        }

        public decimal Price
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
