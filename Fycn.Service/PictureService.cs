using Fycn.Interface;
using Fycn.Model.Resource;
using Fycn.SqlDataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fycn.Utility;

namespace Fycn.Service
{
    public class PictureService : AbstractService, IBase<PictureModel>
    {

        public List<PictureModel> GetAll(PictureModel pictureInfo)
        {
            string userClientId = HttpContextHandler.GetHeaderObj("UserClientId").ToString();

            var conditions = new List<Condition>();
          

            conditions.Add(new Condition
            {
                LeftBrace = "",
                ParamName = "ClientId",
                DbColumnName = "",
                ParamValue = userClientId,
                Operation = ConditionOperate.None,
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

            var conditions = new List<Condition>();
          
            conditions.Add(new Condition
            {
                LeftBrace = "",
                ParamName = "ClientId",
                DbColumnName = "",
                ParamValue = userClientId,
                Operation = ConditionOperate.None,
                RightBrace = "",
                Logic = ""
            });



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
                GenerateDal.BeginTransaction();

                PictureModel pictureInfo = new PictureModel();
                pictureInfo.PicId = id;
                GenerateDal.Delete<PictureModel>(CommonSqlKey.DeletePictureList, pictureInfo);
                GenerateDal.CommitTransaction();

                return 1;
            }
            catch (Exception e)
            {
                GenerateDal.RollBack();
                return 0;
            }
        }

        public int UpdateData(PictureModel pictureInfo)
        {
            string userClientId = HttpContextHandler.GetHeaderObj("UserClientId").ToString();
            pictureInfo.ClientId = userClientId;
            return GenerateDal.Update(CommonSqlKey.UpdatePictureList, pictureInfo);
        }
    }
}
