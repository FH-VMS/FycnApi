using Fycn.Model.Sys;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fycn.Model.Ad
{
    [Table("table_ad_relation")]
    public class AdRelationModel
    {

        [Column(Name = "ad_id")]
        public string AdId
        {
            get;
            set;
        }

        [Column(Name = "source_id")]
        public string SourceId
        {
            get;
            set;
        }

        [Column(Name = "sequence")]
        public int Sequence
        {
            get;
            set;
        }

        [Column(Name = "ad_type")]
        public int AdType
        {
            get;
            set;
        }

        public string PicId
        {
            get;
            set;
        }

        public string PicUrl
        {
            get;
            set;
        }

        public string PicName
        {
            get;
            set;
        }
    }
}
