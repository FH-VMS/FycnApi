using Fycn.Model.Sys;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fycn.Model.Android
{
    [Table("table_product_type")]
    public class AndroidProductTypeModel
    {
        [Column(Name = "wares_type_id")]
        public string WaresTypeId
        {
            get;
            set;
        }

        [Column(Name = "wares_type_name")]
        public string WaresTypeName
        {
            get;
            set;
        }

        [Column(Name = "sequence")]
        public int Sequence
        {
            get;
            set;
        }
    }
}
