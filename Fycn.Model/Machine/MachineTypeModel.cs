using Fycn.Model.Sys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fycn.Model.Machine
{
     [Table("table_machine_type")]
    public class MachineTypeModel
    {
        [Column(Name = "id")]
        public string Id
        {
            get;
            set;
        }

          [Column(Name = "type_name")]
        public string TypeName
        {
            get;
            set;
        }

          [Column(Name = "type_type")]
        public string TypeType
        {
            get;
            set;
        }

          public string TypeTypeText
          {
              get;
              set;
          }

           [Column(Name = "max_goods")]
          public int MaxGoods
          {
              get;
              set;
          }

          [Column(Name = "type_remark")]
          public string TypeRemark
          {
              get;
              set;
          }

          [Column(Name = "communicate")]
          public string Communicate
          {
              get;
              set;
          }

          public List<CabinetConfigModel> Cabinets
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
