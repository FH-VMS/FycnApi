using Fycn.Model.Ad;
using Fycn.SqlDataAccess;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fycn.Service
{
    public class AdRelationService : AbstractService
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
    }
}
