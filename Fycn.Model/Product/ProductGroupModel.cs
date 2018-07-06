using Fycn.Model.Sys;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fycn.Model.Product
{
    [Table("table_product_group")]
    public class ProductGroupModel
    {
        [Column(Name = "wares_id")]
        public string WaresId
        {
            get;
            set;
        }

        [Column(Name = "wares_name")]
        public string WaresName
        {
            get;
            set;
        }

        [Column(Name = "wares_unitprice")]
        public decimal WaresUnitPrice
        {
            get;
            set;
        }

        [Column(Name = "wares_discount_unitprice")]
        public decimal WaresDiscountUnitPrice
        {
            get;
            set;
        }

        [Column(Name = "wares_status")]
        public string WaresStatus
        {
            get;
            set;
        }

        public string WaresStatusText
        {
            get;
            set;
        }

        [Column(Name = "wares_weight")]
        public decimal WaresWeight
        {
            get;
            set;
        }

        [Column(Name = "wares_specifications")]
        public string WaresSpecifications
        {
            get;
            set;
        }

        [Column(Name = "wares_manufacture_date")]
        public DateTime WaresManufactureDate
        {
            get;
            set;
        }

        [Column(Name = "wares_quality_period")]
        public DateTime WaresQualityPeriod
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

        [Column(Name = "pic_id")]
        public string PicId
        {
            get;
            set;
        }

        public string PicUrl
        {
            get;
            set;
        }

        [Column(Name = "wares_type_id")]
        public string WaresTypeId
        {
            get;
            set;
        }

        public string WaresTypeText
        {
            get;
            set;
        }

        [Column(Name = "supplier_id")]
        public string SupplierId
        {
            get;
            set;
        }

        public string Supplier
        {
            get;
            set;
        }

        [Column(Name = "wares_description")]
        public string WaresDescription
        {
            get;
            set;
        }

        [Column(Name = "is_group")]
        public int IsGroup
        {
            get;
            set;
        }


        [Column(Name = "creator")]
        public string Creator
        {
            get;
            set;
        }

        [Column(Name = "update_date")]
        public DateTime UpdateDate
        {
            get;
            set;
        }

        public List<ProductGroupRelationModel> lstProductRelation
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
