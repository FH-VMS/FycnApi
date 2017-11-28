using Fycn.Model.Sys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fycn.Model.Machine
{
    [Table("table_operation")]
    public class OperationLogModel
    {
        [Column(Name = "machine_id")]
        public string MachineId
        {
            get;
            set;
        }

        [Column(Name = "opt_content")]
        public string OptContent
        {
            get;
            set;
        }

        [Column(Name = "opt_date")]
        public DateTime OptDate
        {
            get;
            set;
        }

        [Column(Name = "operator")]
        public string Operator
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
    }
}
