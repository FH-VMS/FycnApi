using Fycn.Interface;
using Fycn.Model.Ad;
using Fycn.SqlDataAccess;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fycn.Service
{
    public class AdRelationService : AbstractService, IAdRelation
    {
        public int PostAdRelationData(AdRelationModel adRelationInfo)
        {
            try
            {
                GenerateDal.BeginTransaction();

                GenerateDal.Create(adRelationInfo);
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
            AdRelationModel adRelationInfo = new AdRelationModel();
            adRelationInfo.AdId = int.Parse(id);
            return GenerateDal.Delete<AdRelationModel>(CommonSqlKey.DeleteAdRelation, adRelationInfo);
        }

        public List<AdRelationModel> GetRelationByIdAndType(AdRelationModel adRelationInfo)
        {
            var conditions = new List<Condition>();
            conditions.Add(new Condition
            {
                LeftBrace = " AND ",
                ParamName = "AdId",
                DbColumnName = "a.ad_id",
                ParamValue = adRelationInfo.AdId,
                Operation = ConditionOperate.Equal,
                RightBrace = "",
                Logic = ""
            });
            if (adRelationInfo.AdType != 0)
            {
                conditions.Add(new Condition
                {
                    LeftBrace = " AND ",
                    ParamName = "AdType",
                    DbColumnName = "a.ad_type",
                    ParamValue = adRelationInfo.AdType,
                    Operation = ConditionOperate.Equal,
                    RightBrace = "",
                    Logic = ""
                });
            }
            
            return GenerateDal.LoadByConditions<AdRelationModel>(CommonSqlKey.GetRelationByIdAndType, conditions);
        }
    }
}
