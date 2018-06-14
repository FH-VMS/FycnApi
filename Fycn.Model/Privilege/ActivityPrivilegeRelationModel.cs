using Fycn.Model.Sys;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fycn.Model.Privilege
{
    [Table("table_activity_privilege_relation")]
    public class ActivityPrivilegeRelationModel
    {
        [Column(Name = "activity_id")]
        public string ActivityId
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

        [Column(Name = "rate")]
        public decimal Rate
        {
            get;
            set;
        }
    }
}
