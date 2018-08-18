using Fycn.Model.Sys;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fycn.Model.Privilege
{
    [Table("table_activity_info")]
    public class ActivityModel
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

        [Column(Name = "activity_type")]
        public string ActivityType
        {
            get;
            set;
        }

        public string ActivityTypeText
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

        [Column(Name = "count_per_person")]
        public int CountPerPerson
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

        [Column(Name = "end_time")]
        public DateTime EndTime
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

        [Column(Name = "creator")]
        public string Creator
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

        public List<ActivityPrivilegeRelationModel> listActivityPrivilege
        {
            get;
            set;
        }
    }
}
