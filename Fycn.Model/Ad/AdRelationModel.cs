using Fycn.Model.Sys;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fycn.Model.Ad
{
    public class AdRelationModel
    {
        [Column(Name = "ad_id")]
        public int AdId
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

        public string ResourcePath
        {
            get;
            set;
        }

        public string ResourceName
        {
            get;
            set;
        }
    }
}
