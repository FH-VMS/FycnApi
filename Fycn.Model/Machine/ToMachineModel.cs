using Fycn.Model.Sys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fycn.Model.Machine
{
    [Table("table_to_machine")]
    public class ToMachineModel
    {
        [Column(Name = "machine_id")]
        public string MachineId
        {
            get;
            set;
        }

        [Column(Name = "machine_status")]
        public string MachineStatus
        {
            get;
            set;
        }


        public string m
        {
            get;
            set;
        }

        public string s
        {
            get;
            set;
        }
    }
}
