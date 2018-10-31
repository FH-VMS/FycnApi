using Fycn.Model.Sys;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fycn.Model.Member
{
    [Table("table_member_account")]
    public class MemberAccountModel
    {
        [Column(Name = "id")]
        public string Id
        {
            get;
            set;
        }

        [Column(Name = "account_name")]
        public string AccountName
        {
            get;
            set;
        }

        [Column(Name = "account_data")]
        public int AccountData
        {
            get;
            set;
        }

        [Column(Name = "account_type")]
        public string AccountType
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

        [Column(Name = "member_id")]
        public string MemberId
        {
            get;
            set;
        }

        [Column(Name = "display_unit")]
        public string DisplayUnit
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

        [Column(Name = "wares_type_id")]
        public string WaresTypeId
        {
            get;
            set;
        }

        [Column(Name = "transfer_with_money")]
        public string TransferWithMoney
        {
            get;
            set;
        }
    }
}
