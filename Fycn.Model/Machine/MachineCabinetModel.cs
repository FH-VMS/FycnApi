using Fycn.Model.Sys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fycn.Model.Machine
{
    [Table("table_cabinet_config")]
    public class MachineCabinetModel
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
        public int LayerNumber  //层数 如6
        {
            get;
            set;
        }

        [Column(Name = "layer_goods_number")]
        public string LayerGoodsNumber  //列数 以,隔开 如6,6,6,6,6,6
        {
            get;
            set;
        }

        [Column(Name = "remark")]
        public string Remark  //
        {
            get;
            set;
        }

        [Column(Name = "cabinet_display")]
        public string CabinetDisplay  // 货道编号规则首字母  例：A0101   此字段为A
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
