using Fycn.Model.Sys;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fycn.Model.Wechat
{
    [Table("table_wechat_member")]
    public class WechatMemberModel
    {
        [Column(Name = "openid")]
        public string OpenId
        {
            get;
            set;
        }

        [Column(Name = "nickname")]
        public string NickName
        {
            get;
            set;
        }

        [Column(Name = "sex")]
        public int Sex
        {
            get;
            set;
        }

        [Column(Name = "province")]
        public string Province
        {
            get;
            set;
        }

        [Column(Name = "city")]
        public string City
        {
            get;
            set;
        }

        [Column(Name = "country")]
        public string Country
        {
            get;
            set;
        }

        [Column(Name = "headimgurl")]
        public string HeadImgUrl
        {
            get;
            set;
        }

        [Column(Name = "privilege")]
        public List<string> Privilege
        {
            get;
            set;
        }

        [Column(Name = "unionid")]
        public string UnionId
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

        public string ClientId
        {
            get;
            set;
        }
    }
}
