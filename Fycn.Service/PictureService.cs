﻿using Fycn.Interface;
using Fycn.Model.Resource;
using Fycn.SqlDataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fycn.Utility;
using System.IO;

namespace Fycn.Service
{
    public class PictureService : AbstractService, IBase<PictureModel>
    {

        public List<PictureModel> GetAll(PictureModel pictureInfo)
        {
            string userClientId = HttpContextHandler.GetHeaderObj("UserClientId").ToString();
            if (string.IsNullOrEmpty(userClientId))
            {
                return null;
            }
            var conditions = new List<Condition>();
            string clientIds = new CommonService().GetClientIds(userClientId);
            if (clientIds.Contains("self"))
            {
                clientIds = "'" + clientIds.Replace(",", "','") + "'";
            }

            conditions.Add(new Condition
            {
                LeftBrace = " AND ",
                ParamName = "ClientId",
                DbColumnName = "a.client_id",
                ParamValue = clientIds,
                Operation = ConditionOperate.INWithNoPara,
                RightBrace = "",
                Logic = ""
            });

            if (!string.IsNullOrEmpty(pictureInfo.FileType))
            {
                conditions.Add(new Condition
                {
                    LeftBrace = " AND ",
                    ParamName = "FileType",
                    DbColumnName = "a.file_type",
                    ParamValue = pictureInfo.FileType,
                    Operation = ConditionOperate.Equal,
                    RightBrace = "",
                    Logic = ""
                });
            }


            if (!string.IsNullOrEmpty(pictureInfo.Belong))
            {
                conditions.Add(new Condition
                {
                    LeftBrace = " AND ",
                    ParamName = "Belong",
                    DbColumnName = "a.belong",
                    ParamValue = pictureInfo.Belong,
                    Operation = ConditionOperate.Equal,
                    RightBrace = "",
                    Logic = ""
                });
            }

            if (!string.IsNullOrEmpty(pictureInfo.PicName))
            {
                conditions.Add(new Condition
                {
                    LeftBrace = " AND ",
                    ParamName = "PicName",
                    DbColumnName = "a.pic_name",
                    ParamValue = pictureInfo.PicName,
                    Operation = ConditionOperate.Equal,
                    RightBrace = "",
                    Logic = ""
                });
            }

            conditions.Add(new Condition
            {
                LeftBrace = "  ",
                ParamName = "ResourceUrl",
                DbColumnName = "",
                ParamValue = ConfigHandler.ResourceUrl,
                Operation = ConditionOperate.None,
                RightBrace = "",
                Logic = ""
            });

            conditions.Add(new Condition
            {
                LeftBrace = "  ",
                ParamName = "UploadTime",
                DbColumnName = "upload_time",
                ParamValue = "desc",
                Operation = ConditionOperate.OrderBy,
                RightBrace = "",
                Logic = ""
            });

           

            conditions.AddRange(CreatePaginConditions(pictureInfo.PageIndex, pictureInfo.PageSize));

            return GenerateDal.LoadByConditions<PictureModel>(CommonSqlKey.GetPictureList, conditions);
        }


        public int GetCount(PictureModel pictureInfo)
        {
            var result = 0;

            string userClientId = HttpContextHandler.GetHeaderObj("UserClientId").ToString();
            if (string.IsNullOrEmpty(userClientId))
            {
                return 0;
            }
            var conditions = new List<Condition>();

            string clientIds = new CommonService().GetClientIds(userClientId);
            if (clientIds.Contains("self"))
            {
                clientIds = "'" + clientIds.Replace(",", "','") + "'";
            }

            conditions.Add(new Condition
            {
                LeftBrace = " AND ",
                ParamName = "ClientId",
                DbColumnName = "a.client_id",
                ParamValue = clientIds,
                Operation = ConditionOperate.INWithNoPara,
                RightBrace = "",
                Logic = ""
            });

            if (!string.IsNullOrEmpty(pictureInfo.FileType))
            {
                conditions.Add(new Condition
                {
                    LeftBrace = " AND ",
                    ParamName = "FileType",
                    DbColumnName = "a.file_type",
                    ParamValue = pictureInfo.FileType,
                    Operation = ConditionOperate.Equal,
                    RightBrace = "",
                    Logic = ""
                });
            }

            if (!string.IsNullOrEmpty(pictureInfo.Belong))
            {
                conditions.Add(new Condition
                {
                    LeftBrace = " AND ",
                    ParamName = "Belong",
                    DbColumnName = "a.belong",
                    ParamValue = pictureInfo.Belong,
                    Operation = ConditionOperate.Equal,
                    RightBrace = "",
                    Logic = ""
                });
            }


            result = GenerateDal.CountByConditions(CommonSqlKey.GetPictureListCount, conditions);

            return result;
        }


        /// <summary>
        /// 新增/修改会员信息
        /// </summary>
        /// <param name="memberInfo"></param>
        /// <returns></returns>
        public int PostData(PictureModel pictureInfo)
        {
            int result;
            string userClientId = HttpContextHandler.GetHeaderObj("UserClientId").ToString();
            if (string.IsNullOrEmpty(userClientId))
            {
                return 0;
            }
            pictureInfo.ClientId = userClientId;
            result = GenerateDal.Create(pictureInfo);




            return result;
        }

        /// <summary>
        /// 删除用户
        /// </summary>
        /// <returns></returns>
        public int DeleteData(string id)
        {

            try
            {
                List<PictureModel> lstPic = GetResourceById(id);
                if(lstPic.Count>0)
                {
                    GenerateDal.BeginTransaction();

                    PictureModel pictureInfo = lstPic[0];
                    pictureInfo.PicId = id;
                    GenerateDal.Delete<PictureModel>(CommonSqlKey.DeletePictureList, pictureInfo);
                    string path = ConfigHandler.UploadUrl + pictureInfo.PicPath;
                    if(File.Exists(path))
                    {
                        File.Delete(path);
                    }
                    GenerateDal.CommitTransaction();
                    return 1;
                }

                return 0;
                
            }
            catch (Exception e)
            {
                GenerateDal.RollBack();
                return 0;
            }
        }

        // 根据id取资源路径
        private List<PictureModel> GetResourceById(string id)
        {
            var conditions = new List<Condition>();
            conditions.Add(new Condition
            {
                LeftBrace = " AND ",
                ParamName = "PicId",
                DbColumnName = "pic_id",
                ParamValue = id,
                Operation = ConditionOperate.Equal,
                RightBrace = "",
                Logic = ""
            });
            return GenerateDal.LoadByConditions<PictureModel>(CommonSqlKey.GetResourceById, conditions);
        }

        public int UpdateData(PictureModel pictureInfo)
        {
            string userClientId = HttpContextHandler.GetHeaderObj("UserClientId").ToString();
            if (string.IsNullOrEmpty(userClientId))
            {
                return 0;
            }
            pictureInfo.ClientId = userClientId;
            return GenerateDal.Update(CommonSqlKey.UpdatePictureList, pictureInfo);
        }
    }
}
