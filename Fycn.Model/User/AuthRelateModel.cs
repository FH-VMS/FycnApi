using Fycn.Model.Sys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fycn.Model.User
{
    [Table("table_corr_dms")]
    public class AuthRelateModel
    {
        [Column(Name = "id")]
        public string Id
        {
            get;
            set;
        }

        [Column(Name = "corr_dms_id")]
        public string CorrDmsId
        {
            get;
            set;
        }

        [Column(Name = "corr_menu_id")]
        public string CorrMenuId
        {
            get;
            set;
        }

        [Column(Name = "corr_add")]
        public int CorrAdd
        {
            get;
            set;
        }

        [Column(Name = "corr_del")]
        public int CorrDel
        {
            get;
            set;
        }

        [Column(Name = "corr_modify")]
        public int CorrModify
        {
            get;
            set;
        }

        [Column(Name = "corr_search")]
        public int CorrSearch
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
    }
}
