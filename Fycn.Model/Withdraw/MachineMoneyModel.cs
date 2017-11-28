using Fycn.Model.Sys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fycn.Model.Withdraw
{
    [Table("table_machine_money")]
    public class MachineMoneyModel
    {
        [Column(Name = "machine_id")]
        public string MachineId
        {
            get;
            set;
        }

        [Column(Name = "ali_money")]
        public float AliMoney
        {
            get;
            set;
        }

        [Column(Name = "wx_money")]
        public float WxMoney
        {
            get;
            set;
        }
    }
}
