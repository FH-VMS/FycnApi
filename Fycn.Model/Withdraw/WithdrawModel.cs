using Fycn.Model.Sys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fycn.Model.Withdraw
{
    [Table("table_withdraw_record")]
    public class WithdrawModel
    {
        [Column(Name = "id")]
        public string Id
        {
            get;
            set;
        }

        [Column(Name = "client_id")]
        public string ClientId
        {
            get;
            set;
        }

        [Column(Name = "money")]
        public float Money
        {
            get;
            set;
        }

        [Column(Name = "opt_type")]
        public string OptType
        {
            get;
            set;
        }

        [Column(Name = "opt_status")]
        public string OptStatus
        {
            get;
            set;
        }

        [Column(Name = "opt_datetime")]
        public DateTime OptDatetime
        {
            get;
            set;
        }
    }
}
