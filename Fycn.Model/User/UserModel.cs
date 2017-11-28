using Fycn.Model.Sys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fycn.Model.User
{
    [Table("table_user")]
    public class UserModel:BaseModel
    {
        [Column(Name = "id")]
        public string Id
        {
            get;
            set;
        }

        [Column(Name = "usr_account")]
        public string UserAccount
        {
            get;
            set;
        }
        [Column(Name = "usr_access_id")]
        public string UserAccessId
        {
            get;
            set;
        }


        [Column(Name = "usr_name")]
        public string UserName
        {
            get;
            set;
        }
        [Column(Name = "usr_pwd")]
        public string UserPassword
        {
            get;
            set;
        }
        [Column(Name = "usr_client_id")]
        public string UserClientId
        {
            get;
            set;
        }

        public string UserClientName
        {
            get;
            set;
        }

        [Column(Name = "enddate")]
        public DateTime EndDate
        {
            get;
            set;
        }

        [Column(Name = "sts")]
        public int Sts
        {
            get;
            set;
        }


        public List<UserModel> children
        {
            get;
            set;
        }
      
    }
}
