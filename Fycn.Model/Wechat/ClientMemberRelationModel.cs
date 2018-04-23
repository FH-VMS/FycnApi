using Fycn.Model.Sys;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fycn.Model.Wechat
{
    [Table("table_client_member_relation")]
    public class ClientMemberRelationModel
    {
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

        [Column(Name = "create_time")]
        public DateTime CreateTime
        {
            get;
            set;
        }
    }
}
