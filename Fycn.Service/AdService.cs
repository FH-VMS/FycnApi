using Fycn.Interface;
using Fycn.Model.Ad;
using Fycn.Model.Sys;
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
                string userClientId = HttpContextHandler.GetHeaderObj("UserClientId").ToString();
                if (string.IsNullOrEmpty(userClientId))
                {
                    return 0;
                }
                GenerateDal.BeginTransaction();
                if(string.IsNullOrEmpty(adInfo.Id))
                {
                    adInfo.Id = Guid.NewGuid().ToString();
                } else
                {
                    DeleteData(adInfo.Id);
                }
                adInfo.ClientId = userClientId;
                GenerateDal.Create(adInfo);
                if (adInfo.Relations != null && adInfo.Relations.Count > 0)
                {
                    foreach (var item in adInfo.Relations)
                    {
                        item.AdId = adInfo.Id;
                        new AdRelationService().PostAdRelationData(item);
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
                adInfo.Id = id;
                GenerateDal.Delete<AdModel>(CommonSqlKey.DeleteAd, adInfo);
                new AdRelationService().DeleteData(adInfo.Id.ToString());
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
                if (string.IsNullOrEmpty(userClientId))
                {
                    return 0;
                }
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
