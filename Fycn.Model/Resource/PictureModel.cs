﻿using Fycn.Model.Sys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fycn.Model.Resource
{
     [Table("table_pic")]
    public class PictureModel
    {
         [Column(Name = "pic_id")]
         public string PicId
         {
             get;
             set;
         }

         [Column(Name = "pic_name")]
         public string PicName
         {
             get;
             set;
         }

         [Column(Name = "pic_path")]
         public string PicPath
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

        [Column(Name = "file_type")]
        public string FileType
        {
            get;
            set;
        }

        [Column(Name = "belong")]
        public string Belong
        {
            get;
            set;
        }

        [Column(Name = "upload_time")]
        public DateTime UploadTime
        {
            get;
            set;
        }

        [Column(Name = "size")]
        public long Size
        {
            get;
            set;
        }

        public string PicUrl
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
