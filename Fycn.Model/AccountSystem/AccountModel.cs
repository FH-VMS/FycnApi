using Fycn.Model.Sys;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fycn.Model.AccountSystem
{
    [Table("table_transfer_account_system")]
    public class AccountModel
    {
        [Column(Name = "id")]
        public string Id
        {
            get;
            set;
        }

        [Column(Name = "name")]
        public string Name
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

        [Column(Name = "pay_config_id")]
        public string PayConfigId
        {
            get;
            set;
        }

        [Column(Name = "partner_id")]
        public string PartnerId
        {
            get;
            set;
        }

        [Column(Name = "mch_id")]
        public string MchId
        {
            get;
            set;
        }

        [Column(Name = "user_openid")]
        public string UserOpenid
        {
            get;
            set;
        }

        [Column(Name = "wx_user_name")]
        public string WxUserName
        {
            get;
            set;
        }

        [Column(Name = "ali_account")]
        public string AliAccount
        {
            get;
            set;
        }

        [Column(Name = "ali_user_name")]
        public string AliUserName
        {
            get;
            set;
        }

        [Column(Name = "wx_rate")]
        public float WxRate
        {
            get;
            set;
        }

        [Column(Name = "ali_rate")]
        public float AliRate
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

        public string PayConfigName
        {
            get;
            set;
        }
    }
}
