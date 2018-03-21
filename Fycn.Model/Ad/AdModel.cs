using Fycn.Model.Resource;
using Fycn.Model.Sys;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fycn.Model.Ad
{
    public class AdModel
    {
        [Column(Name = "id",IsPrimaryKey =true)]
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

        [Column(Name = "client_id")]
        public string ClientId
        {
            get;
            set;
        }

        public List<PictureModel> Reources
        {
            get;
            set;
        }

        public List<AdRelationModel> Relations
        {
            get;
            set;
        }
    }
}
