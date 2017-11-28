using Fycn.Model.Sys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fycn.Model.Machine
{
    [Table("table_mt_goods")]
    public class MachineTypeAndCabinetModel
    {
         [Column(Name = "machine_type_id",IsPrimaryKey=true)]
          public string MachineTypeId
          {
              get;
              set;
          }

         [Column(Name = "cabinet_type_id")]
         public string CabinetTypeId
         {
             get;
             set;
         }
    }
}
