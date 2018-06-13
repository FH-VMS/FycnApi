using Fycn.Model.Sys;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fycn.Model.Machine
{
    [Table("table_machine_location")]
    public class MachineLocationModel
    {
        [Column(Name = "id")]
        public string Id
        {
            get;
            set;
        }

        [Column(Name = "machine_id")]
        public string MachineId
        {
            get;
            set;
        }

        [Column(Name = "address")]
        public string Address
        {
            get;
            set;
        }

        [Column(Name = "longitude")]
        public string Longitude
        {
            get;
            set;
        }

        [Column(Name = "latitude")]
        public string Latitude
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

        public string StartLong
        {
            get;
            set;
        }

        public string EndLong
        {
            get;
            set;
        }

        public string StartLati
        {
            get;
            set;
        }

        public string EndLati
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
