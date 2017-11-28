using Fycn.Model.Common;
using Fycn.Model.Sys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fycn.Model.User
{
    [Table("table_dms")]
    public class AuthModel
    {
        [Column(Name = "id")]
        public string Id
        {
            get;
            set;
        }

        [Column(Name = "dms_name")]
        public string DmsName
        {
            get;
            set;
        }

        [Column(Name = "rank")]
        public int Rank
        {
            get;
            set;
        }

        public string RankName
        {
            get;
            set;
        }

        [Column(Name = "remark")]
        public string Remark
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

        public List<MenuModel> lstAuthRelate
        {
            get;
            set;
        }
    }
}
