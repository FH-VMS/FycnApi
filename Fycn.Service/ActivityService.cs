using Fycn.Interface;
using Fycn.Model.Privilege;
using Fycn.SqlDataAccess;
using Fycn.Utility;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fycn.Service
{
    public class ActivityService : AbstractService, IBase<ActivityModel>
    {
        public List<ActivityModel> GetAll(ActivityModel activityInfo)
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
            var result = new List<ActivityModel>();
            conditions.Add(new Condition
            {
                LeftBrace = " AND ",
                ParamName = "ClientId",
                DbColumnName = "a.client_id",
                ParamValue = clientIds,
                Operation = ConditionOperate.INWithNoPara,
                RightBrace = " ",
                Logic = ""
            });
            if (!string.IsNullOrEmpty(activityInfo.Name))
            {
                conditions.Add(new Condition
                {
                    LeftBrace = " AND ",
                    ParamName = "Name",
                    DbColumnName = "a.name",
                    ParamValue = "%" + activityInfo.Name + "%",
                    Operation = ConditionOperate.Like,
                    RightBrace = "",
                    Logic = ""
                });
            }

            if (!string.IsNullOrEmpty(activityInfo.ActivityType))
            {
                conditions.Add(new Condition
                {
                    LeftBrace = " AND ",
                    ParamName = "ActivityType",
                    DbColumnName = "a.activity_type",
                    ParamValue = activityInfo.ActivityType,
                    Operation = ConditionOperate.Equal,
                    RightBrace = "",
                    Logic = ""
                });
            }
            conditions.AddRange(CreatePaginConditions(activityInfo.PageIndex, activityInfo.PageSize));
            result = GenerateDal.LoadByConditions<ActivityModel>(CommonSqlKey.GetActivityList, conditions);






            return result;
        }


        public int GetCount(ActivityModel activityInfo)
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
                DbColumnName = "client_id",
                ParamValue = clientIds,
                Operation = ConditionOperate.INWithNoPara,
                RightBrace = " ",
                Logic = ""
            });
            if (!string.IsNullOrEmpty(activityInfo.Name))
            {
                conditions.Add(new Condition
                {
                    LeftBrace = " AND ",
                    ParamName = "Name",
                    DbColumnName = "name",
                    ParamValue = "%" + activityInfo.Name + "%",
                    Operation = ConditionOperate.Like,
                    RightBrace = "",
                    Logic = ""
                });
            }

            if (!string.IsNullOrEmpty(activityInfo.ActivityType))
            {
                conditions.Add(new Condition
                {
                    LeftBrace = " AND ",
                    ParamName = "ActivityType",
                    DbColumnName = "activity_type",
                    ParamValue = activityInfo.ActivityType,
                    Operation = ConditionOperate.Equal,
                    RightBrace = "",
                    Logic = ""
                });
            }
            result = GenerateDal.CountByConditions(CommonSqlKey.GetActivityCount, conditions);

            return result;
        }

        
        public int PostData(ActivityModel activityInfo)
        {
            try
            {
                GenerateDal.BeginTransaction();
                string userClientId = activityInfo.ClientId;

                if (string.IsNullOrEmpty(userClientId))
                {
                    userClientId = HttpContextHandler.GetHeaderObj("UserClientId").ToString();
                }
                string userAccount = HttpContextHandler.GetHeaderObj("UserAccount").ToString();
                activityInfo.Id = Guid.NewGuid().ToString();
                activityInfo.Creator = userAccount;
                activityInfo.CreateDate = DateTime.Now;
                activityInfo.ClientId = userClientId;
                if (activityInfo.listActivityPrivilege != null && activityInfo.listActivityPrivilege.Count > 0)
                {
                    foreach (ActivityPrivilegeRelationModel relationInfo in activityInfo.listActivityPrivilege)
                    {
                        relationInfo.ActivityId = activityInfo.Id;
                        new ActivityRelationService().PostData(relationInfo);
                    }
                }
                GenerateDal.Create(activityInfo);
                GenerateDal.CommitTransaction();
                return 1;
            }
            catch (Exception ex)
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
                ActivityModel activityInfo = new ActivityModel();
                activityInfo.Id = id;
                GenerateDal.Delete<ActivityModel>(CommonSqlKey.DeleteActivity, activityInfo);
                new ActivityRelationService().DeleteData(id);
                GenerateDal.CommitTransaction();
                return 1;
            }
            catch (Exception ex)
            {
                GenerateDal.RollBack();
                return 0;
            }



        }

        public int UpdateData(ActivityModel activityInfo)
        {
            try
            {
                GenerateDal.BeginTransaction();
                activityInfo.CreateDate = DateTime.Now;
                string userClientId = activityInfo.ClientId;

                if (string.IsNullOrEmpty(userClientId))
                {
                    userClientId = HttpContextHandler.GetHeaderObj("UserClientId").ToString();
                }
                string userAccount = HttpContextHandler.GetHeaderObj("UserAccount").ToString();
                activityInfo.ClientId = userClientId;
                activityInfo.Creator = userAccount;

                new ActivityRelationService().DeleteData(activityInfo.Id);
                if (activityInfo.listActivityPrivilege != null && activityInfo.listActivityPrivilege.Count > 0)
                {
                    foreach (ActivityPrivilegeRelationModel relationInfo in activityInfo.listActivityPrivilege)
                    {
                        relationInfo.ActivityId = activityInfo.Id;
                        new ActivityRelationService().PostData(relationInfo);
                    }
                }
                GenerateDal.Update(CommonSqlKey.UpdateActivity, activityInfo);
                GenerateDal.CommitTransaction();
                return 1;
            }
            catch (Exception ex)
            {
                GenerateDal.RollBack();
                return 0;
            }

        }
    }
}
