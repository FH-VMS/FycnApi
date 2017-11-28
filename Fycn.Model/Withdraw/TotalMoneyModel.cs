using Fycn.Model.Sys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fycn.Model.Withdraw
{
    [Table("table_total_money")]
    public class TotalMoneyModel
    {
        [Column(Name = "client_id")]
        public string ClientId
        {
            get;
            set;
        }

        [Column(Name = "ali_account")]
        public decimal AliAccount
        {
            get;
            set;
        }

        [Column(Name = "wx_account")]
        public decimal WxAccount
        {
            get;
            set;
        }
    }
}
