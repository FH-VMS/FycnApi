using Fycn.Model.Sys;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fycn.Model.Product
{
    [Table("table_product_type")]
    public class ProductTypeModel
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

        [Column(Name = "wares_type_remark")]
        public string WaresTypeRemark
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

        public string ClientName
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
