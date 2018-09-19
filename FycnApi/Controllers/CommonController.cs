using FycnApi.Base;
using Fycn.Interface;
using Fycn.Model.Common;
using Fycn.Model.Resource;
using Fycn.Model.Sys;
using Fycn.Model.User;
using Fycn.Service;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Fycn.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Cors;

namespace FycnApi.Controllers
{
    public class CommonController : ApiBaseController
    {
        public ResultObj<List<MenuModel>> GetMenus()
        {
            ICommon menusService = new CommonService();
            var menuList = menusService.GetMenus("");
            return Content(menuList);
        }

        public ResultObj<int> GetWebAPIAndModel()
        {
            // ServiceInfoHelper.WriteWebAPI();
            ServiceInfoHelper.WriteWebModels();
            return Content(1);
        }
        
        public ResultObj<UserModel> PostLogin([FromBody]UserModel userInfo)
        {
            ICommon common = new CommonService();
            var user = common.PostUser(userInfo);
            if (user == null)
            {
                return Content(new UserModel(), ResultCode.NoAccess, "用户名密码不正确！");
            }
            else
            {
                return Content(user);
            }
            
        }

        public ResultObj<List<DicModel>> GetDic(string id)
        {
            ICommon menusService = new CommonService();
            return Content(menusService.GetDic(id));
        }

        public ResultObj<List<DicModel>> GetRank(string id)
        {
            ICommon menusService = new CommonService();
            return Content(menusService.GetRank(id));
        }

        public ResultObj<List<CommonDic>> GetUserByClientId(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return Content(new List<CommonDic>());
            }
            ICommon menusService = new CommonService();
            return Content(menusService.GetUserByClientId(id));
        }

        // 机器字典
        public ResultObj<List<CommonDic>> GetMachineDic(string name="", int pageIndex=1, int pageSize=15)
        {
            ICommon menusService = new CommonService();
            return Content(menusService.GetMachineDic(name, pageIndex, pageSize));
        }

        // 上传图片
        public ResultObj<List<CommonDic>> PostUpload(string typ)
        {
            string userClientId = HttpContextHandler.GetHeaderObj("UserClientId").ToString();
            if (string.IsNullOrEmpty(userClientId))
            {
                return Content(new List<CommonDic>(), ResultCode.Fail, "上传失败");
            }
            var hfc = Request.Form.Files;
            const string localPath = "Attachment/";
            var path = ConfigHandler.UploadUrl + "/" + localPath+ userClientId+"/";
            List<CommonDic> lstCommonDic = new List<CommonDic>();
            if (hfc.Count == 0)
            {
                return Content(lstCommonDic);
            }
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            IBase<PictureModel> _ibase = new PictureService();

            long size = 0;
            foreach (var file in hfc)
            {
                var readFile = ContentDispositionHeaderValue
                                .Parse(file.ContentDisposition)
                                .FileName
                                .Trim('"');
                var fileName = readFile;
                //这个hostingEnv.WebRootPath就是要存的地址可以改下
                string fileNamePath = path + $@"{fileName}";
                size += file.Length;
                using (FileStream fs = System.IO.File.Create(fileNamePath))
                {
                    file.CopyTo(fs);
                    fs.Flush();
                }
                var pictureInfo = new PictureModel();
                string guild = Guid.NewGuid().ToString();
                pictureInfo.PicId = guild;
                pictureInfo.PicName = fileName;
                pictureInfo.PicPath = "/Attachment/"+ userClientId+"/" + fileName;
                pictureInfo.UploadTime = DateTime.Now;
                pictureInfo.FileType = FileType(readFile.Split('.')[1]);
                pictureInfo.Size = size;
                pictureInfo.Belong = typ;
                pictureInfo.PageIndex = 1;
                pictureInfo.PageSize = 2;
                List<PictureModel> lstPic= _ibase.GetAll(pictureInfo);
                if (lstPic.Count > 0)
                {
                    pictureInfo.PicId = lstPic[0].PicId;
                    _ibase.UpdateData(pictureInfo);
                }
                else
                {
                    _ibase.PostData(pictureInfo);
                }
               
                lstCommonDic.Add(new CommonDic
                {
                    Id = guild,
                    Name = pictureInfo.PicPath
                });
            }
            /*
            for (var i = 0; i < hfc.Count; i++)
            {
                var fileName = hfc[i].FileName.Split('.')[0] + "_" + DateTime.Now.ToString("yyyyMMddHHmmssfffff") + "." + hfc[i].FileName.Split('.')[1];
                try
                {
                    hfc[i].SaveAs(path + fileName);
                    var pictureInfo = new PictureModel();
                    string guild = Guid.NewGuid().ToString();
                    pictureInfo.PicId=guild;
                    pictureInfo.PicName = fileName;
                    pictureInfo.PicPath = "Attachment/" + fileName;
                    _ibase.PostData(pictureInfo);
                    lstCommonDic.Add(new CommonDic
                    {
                        Id = guild,
                        Name = fileName
                    });
                }
                catch (Exception ex)
                {
                    return Content(lstCommonDic);
                }
            }
            */
            return Content(lstCommonDic);
        }

        /// <summary>
        /// 判断文件类型 1：图片  2：视频
        /// </summary>
        /// <param name="fileType"></param>
        /// <returns></returns>
        private string FileType(string fileType)
        {
            string[] images = { "jpg", "jpeg", "png", "bmp", "gif" };
            if (images.Contains(fileType.ToLower()))
            {
                return "1";
            }

            string[] videos = { "mp4","avi","mkv","flv","f4v","rmvb","rm","swf", "wmv" };
            if (videos.Contains(fileType.ToLower()))
            {
                return "2";
            }
            return "";
        }
        

        // 图片字典
        public ResultObj<List<CommonDic>> GetPictureDic(string clientId="", string typ="")
        {
            ICommon menusService = new CommonService();
            return Content(menusService.GetPictureDic(clientId, typ));
        }

        // 取商品作字典
        public ResultObj<List<CommonDic>> GetProductDic(string clientId = "")
        {
            ICommon menusService = new CommonService();
            return Content(menusService.GetProductDic(clientId));
        }

        // 取商品和商品组作字典
        public ResultObj<List<CommonDic>> GetProductAndGroupDic(string clientId = "")
        {
            ICommon menusService = new CommonService();
            return Content(menusService.GetProductAndGroupDic(clientId));
        }

        // 取货柜作字典
        public ResultObj<List<CommonDic>> GetCabinetDic()
        {
            ICommon menusService = new CommonService();
            return Content(menusService.GetCabinetDic());
        }

        //修改密码
        public ResultObj<int> PutPassword([FromBody]UserModel userInfo)
        {
            ICommon menusService = new CommonService();
           
            return Content(menusService.UpdatePassword(userInfo));
        }

        //取支付配置字典
        public ResultObj<List<CommonDic>> GetPayConfigDic(string clientId)
        {
            if (string.IsNullOrEmpty(clientId))
            {
                return Content(new List<CommonDic>());
            }
            ICommon commonService = new CommonService();
            return Content(commonService.GetPayConfigDic(clientId));
        }

        public ResultObj<List<CommonDic>> GetAccountManageDic(string payConfigId, string clientId)
        {
            if (string.IsNullOrEmpty(clientId)||string.IsNullOrEmpty(payConfigId))
            {
                return Content(new List<CommonDic>());
            }
            ICommon commonService = new CommonService();
            return Content(commonService.GetAccountManageDic(payConfigId, clientId));
        }

        //取广告模板字典
        public ResultObj<List<CommonDic>> GetAdDic(string clientId)
        {
            if (string.IsNullOrEmpty(clientId))
            {
                return Content(new List<CommonDic>());
            }
            ICommon commonService = new CommonService();
            return Content(commonService.GetAdDic(clientId));
        }

        //取商品类型做字典
        public ResultObj<List<CommonDic>> GetProductTypeDic(string clientId="")
        {
            ICommon commonService = new CommonService();
            return Content(commonService.GetProductTypeDic(clientId));
        }

        //取未过期优惠券作字典
        public ResultObj<List<CommonDic>> GetNotExpirePrivilegeDic(string clientId = "")
        {
            ICommon commonService = new CommonService();
            return Content(commonService.GetNotExpirePrivilegeDic(clientId));
        }

        public ResultObj<int> ClearLoginCache()
        {
            string userClientId = HttpContextHandler.GetHeaderObj("UserClientId").ToString();
            if(string.IsNullOrEmpty(userClientId))
            {
                return Content(0);
            }
            WebCacheHelper.ClearIds(userClientId);
            return Content(1);
        }

        public ResultObj<string> GetPicPathByWaresId(string waresId)
        {
            
            if (string.IsNullOrEmpty(waresId))
            {
                return Content("");
            }
            ICommon commonService = new CommonService();
            List<PictureModel> lstResult = commonService.GetPicPathByWaresId(waresId);
            if(lstResult.Count==0)
            {
                return Content("");
            }
            else
            {
                return Content(lstResult[0].PicUrl);
            }
        }
    }
}
