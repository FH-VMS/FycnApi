using Fycn.Interface;
using Fycn.Model.Ad;
using Fycn.SqlDataAccess;
using Fycn.Utility;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fycn.Service
{
    public class AdService : AbstractService, IBase<AdModel>
    {

        public List<AdModel> GetAll(AdModel adInfo)
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
            return GenerateDal.LoadByConditions<AdModel>(CommonSqlKey.GetAd, conditions);
        }


        public int GetCount(AdModel adInfo)
        {
            var result = 0;
            

            return result;
        }


        /// <summary>
        /// 新增/修改会员信息
        /// </summary>
        /// <param name="memberInfo"></param>
        /// <returns></returns>
        public int PostData(AdModel adInfo)
        {
            try
            {
                GenerateDal.BeginTransaction();
                GenerateDal.Create(adInfo);
                if (adInfo.Reources != null && adInfo.Reources.Count > 0)
                {
                    foreach (var item in adInfo.Reources)
                    {
                        var tmpInfo = new AdRelationModel();
                        tmpInfo.AdId = adInfo.Id;
                        tmpInfo.SourceId = item.PicId;
                        new AdRelationService().PostAdRelationData(tmpInfo);
                    }
                }
                GenerateDal.CommitTransaction();

                return 1;
            }
            catch (Exception e)
            {
                GenerateDal.RollBack();
                return 0;
            }


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
                AdModel adInfo = new AdModel();
                adInfo.Id = int.Parse(id);
                GenerateDal.Delete<AdModel>(CommonSqlKey.DeleteAd, adInfo);
                new CabinetService().DeleteData(adInfo.Id.ToString());
                GenerateDal.CommitTransaction();

                return 1;
            }
            catch (Exception e)
            {
                GenerateDal.RollBack();
                return 0;
            }

        }

        public int UpdateData(AdModel adInfo)
        {
            try
            {
                GenerateDal.BeginTransaction();
                string userClientId = HttpContextHandler.GetHeaderObj("UserClientId").ToString();
                adInfo.ClientId = userClientId;
                GenerateDal.Update(CommonSqlKey.UpdateAd, adInfo);
                new AdRelationService().DeleteData(adInfo.Id.ToString());
                if (adInfo.Reources != null && adInfo.Reources.Count > 0)
                {
                    foreach (var item in adInfo.Reources)
                    {
                        var tmpInfo = new AdRelationModel();
                        tmpInfo.AdId = adInfo.Id;
                        tmpInfo.SourceId = item.PicId;
                        new AdRelationService().PostAdRelationData(tmpInfo);
                    }
                }
                GenerateDal.CommitTransaction();

                return 1;
            }
            catch (Exception e)
            {
                GenerateDal.RollBack();
                return 0;
            }
        }
    }
}
