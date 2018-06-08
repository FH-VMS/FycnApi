using Fycn.Model.Sys;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fycn.Model.Privilege
{
    [Table("table_privilege_Info")]
    public class PrivilegeModel
    {
        [Column(Name = "privilege_id")]
        public string PrivilegeId
        {
            get;
            set;
        }

        [Column(Name = "privilege_name")]
        public string PrivilegeName
        {
            get;
            set;
        }

        [Column(Name = "create_date")]
        public DateTime CreateDate
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

        [Column(Name = "principle_type")]
        public string PrincipleType
        {
            get;
            set;
        }


        [Column(Name = "principle_group")]
        public string PrincipleGroup
        {
            get;
            set;
        }

        public string PrincipleGroupText
        {
            get;
            set;
        }

        public string PrincipleTypeText
        {
            get;
            set;
        }

        [Column(Name = "use_money_limit")]
        public decimal UseMoneyLimit
        {
            get;
            set;
        }

        [Column(Name = "expire_time")]
        public DateTime ExpireTime
        {
            get;
            set;
        }

        [Column(Name = "start_time")]
        public DateTime StartTime
        {
            get;
            set;
        }

        [Column(Name = "time_rule")]
        public string TimeRule
        {
            get;
            set;
        }

        public string TimeRuleText
        {
            get;
            set;
        }

        [Column(Name = "is_bind")]
        public int IsBind
        {
            get;
            set;
        }

        [Column(Name = "is_overlay")]
        public int IsOverlay
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

        [Column(Name = "privilege_instru")]
        public string PrivilegeInstru
        {
            get;
            set;
        }

        [Column(Name = "bind_product_ids")]
        public string BindProductIds
        {
            get;
            set;
        }

        [Column(Name = "source_id")]
        public string SourceId
        {
            get;
            set;
        }

        [Column(Name = "money")]
        public decimal Money
        {
            get;
            set;
        }

        [Column(Name = "discount")]
        public decimal Discount
        {
            get;
            set;
        }

        public int PageIndex
        {
            get;
            set;
        }

        [Column(Name = "display_text")]
        public string DisplayText
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
