using Fycn.Model.Sys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fycn.Model.Machine
{
    [Table("table_cabinet_config")]
    public class CabinetConfigModel
    {
        [Column(Name = "cabinet_id")]
        public string CabinetId
        {
            get;
            set;
        }

        [Column(Name = "cabinet_name")]
        public string CabinetName
        {
            get;
            set;
        }

        [Column(Name = "cabinet_type")]
        public string CabinetType
        {
            get;
            set;
        }



        [Column(Name = "layer_number")]
        public int LayerNumber
        {
            get;
            set;
        }

        [Column(Name = "layer_goods_number")]
        public string LayerGoodsNumber
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

        [Column(Name = "cabinet_display")]
        public string CabinetDisplay
        {
            get;
            set;
        }
    }
}
