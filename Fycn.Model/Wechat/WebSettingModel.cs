using Fycn.Model.Sys;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fycn.Model.Wechat
{
    [Table("table_wechat_info")]
    public class WebSettingModel
    {
        [Column(Name = "id")]
        public string Id
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

        [Column(Name = "name")]
        public string Name
        {
            get;
            set;
        }

        [Column(Name = "service_json")]
        public string ServiceJson
        {
            get;
            set;
        }

        [Column(Name = "carousel_json")]
        public string CarouselJson
        {
            get;
            set;
        }
    }
}
