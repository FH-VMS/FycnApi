using Fycn.Model.Sys;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fycn.Model.Privilege
{
    [Table("table_privilege_member_relation")]
    public class PrivilegeMemberRelationModel
    {
        [Column(Name = "id")]
        public string Id
        {
            get;
            set;
        }

        [Column(Name = "member_id")]
        public string MemberId
        {
            get;
            set;
        }

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

        [Column(Name = "expire_time")]
        public DateTime ExpireTime
        {
            get;
            set;
        }

        [Column(Name = "privilege_status")]
        public int PrivilegeStatus
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

        [Column(Name = "use_money_limit")]
        public decimal UseMoneyLimit
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

        [Column(Name = "client_id")]
        public string ClientId
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

        [Column(Name = "source_id")]
        public string SourceId
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

        [Column(Name = "come_from")]
        public string ComeFrom
        {
            get;
            set;
        }

        [Column(Name = "trade_no")]
        public string TradeNo
        {
            get;
            set;
        }

        [Column(Name = "happen_date")]
        public DateTime HappenDate
        {
            get;
            set;
        }

        [Column(Name = "get_date")]
        public DateTime GetDate
        {
            get;
            set;
        }

        public bool Chosen
        {
            get;
            set;
        }

        public string TimeRule
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
