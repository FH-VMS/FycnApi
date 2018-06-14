using Fycn.Interface;
using Fycn.Model.Privilege;
using Fycn.SqlDataAccess;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fycn.Service
{
    public class ActivityRelationService : AbstractService, IBase<ActivityPrivilegeRelationModel>
    {
        public List<ActivityPrivilegeRelationModel> GetAll(ActivityPrivilegeRelationModel relationInfo)
        {

            
            var conditions = new List<Condition>();
            conditions.Add(new Condition
            {
                LeftBrace = " AND ",
                ParamName = "ActivityId",
                DbColumnName = "a.activity_id",
                ParamValue = relationInfo.ActivityId,
                Operation = ConditionOperate.Equal,
                RightBrace = " ",
                Logic = ""
            });

            return GenerateDal.LoadByConditions<ActivityPrivilegeRelationModel>(CommonSqlKey.GetActivityRelation, conditions);
        }


        public int GetCount(ActivityPrivilegeRelationModel relationInfo)
        {

            return 0;
        }


        /// <summary>
        /// 新增/修改会员信息
        /// </summary>
        /// <param name="memberInfo"></param>
        /// <returns></returns>
        public int PostData(ActivityPrivilegeRelationModel relationInfo)
        {
            int result;
            result = GenerateDal.Create(relationInfo);




            return result;
        }

        /// <summary>
        /// 删除用户
        /// </summary>
        /// <returns></returns>
        public int DeleteData(string id)
        {


            ActivityPrivilegeRelationModel relationInfo = new ActivityPrivilegeRelationModel();
            relationInfo.ActivityId = id;
            return GenerateDal.Delete<ActivityPrivilegeRelationModel>(CommonSqlKey.DeleteActivityRelation, relationInfo);


        }

        public int UpdateData(ActivityPrivilegeRelationModel relationInfo)
        {
            return 0;
        }
    }
}
