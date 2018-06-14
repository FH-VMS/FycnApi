using Fycn.Model.Sys;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fycn.Model.Product
{
    [Table("table_product_group_relation")]
    public class ProductGroupRelationModel
    {
        [Column(Name = "client_id")]
        public string ClientId
        {
            get;
            set;
        }

        [Column(Name = "wares_id")]
        public string WaresId
        {
            get;
            set;
        }

        [Column(Name = "wares_group_id")]
        public string WaresGroupId
        {
            get;
            set;
        }

        [Column(Name = "numbers")]
        public int Numbers
        {
            get;
            set;
        }
    }
}
