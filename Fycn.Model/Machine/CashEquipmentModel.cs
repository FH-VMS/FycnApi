using Fycn.Model.Sys;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fycn.Model.Machine
{
    [Table("table_cash_equipment")]
    public class CashEquipmentModel
    {
        [Column(Name = "id",IsPrimaryKey =true)]
        public string Id
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

        [Column(Name = "cash_status")]
        public string CashStatus
        {
            get;
            set;
        }

        [Column(Name = "cash_stock")]
        public string CashStock
        {
            get;
            set;
        }

        [Column(Name = "coin_status")]
        public string CoinStatus
        {
            get;
            set;
        }

        [Column(Name = "coin_stock")]
        public string CoinStock
        {
            get;
            set;
        }

        public string UpdateType
        {
            get;
            set;
        }

        public string MachineName
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
