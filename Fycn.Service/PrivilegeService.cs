using Fycn.Interface;
using Fycn.Model.Privilege;
using Fycn.SqlDataAccess;
using Fycn.Utility;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fycn.Service
{
    public class PrivilegeService : AbstractService, IBase<PrivilegeModel>
    {
        public List<PrivilegeModel> GetAll(PrivilegeModel privilegeInfo)
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
                RightBrace = " ",
                Logic = ""
            });
            if (!string.IsNullOrEmpty(privilegeInfo.PrivilegeName))
            {
                conditions.Add(new Condition
                {
                    LeftBrace = " AND ",
                    ParamName = "PrivilegeName",
                    DbColumnName = "a.privilege_name",
                    ParamValue = "%" + privilegeInfo.PrivilegeName + "%",
                    Operation = ConditionOperate.Like,
                    RightBrace = "",
                    Logic = ""
                });
            }

            if (!string.IsNullOrEmpty(privilegeInfo.PrincipleType))
            {
                conditions.Add(new Condition
                {
                    LeftBrace = " AND ",
                    ParamName = "PrincipleType",
                    DbColumnName = "a.principle_type",
                    ParamValue = privilegeInfo.PrincipleType,
                    Operation = ConditionOperate.Equal,
                    RightBrace = "",
                    Logic = ""
                });
            }

            conditions.AddRange(CreatePaginConditions(privilegeInfo.PageIndex, privilegeInfo.PageSize));

            return GenerateDal.LoadByConditions<PrivilegeModel>(CommonSqlKey.GetPrivilegeList, conditions);
        }


        public int GetCount(PrivilegeModel privilegeInfo)
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
            if (!string.IsNullOrEmpty(privilegeInfo.PrivilegeName))
            {
                conditions.Add(new Condition
                {
                    LeftBrace = " AND ",
                    ParamName = "PrivilegeName",
                    DbColumnName = "a.privilege_name",
                    ParamValue = "%" + privilegeInfo.PrivilegeName + "%",
                    Operation = ConditionOperate.Like,
                    RightBrace = "",
                    Logic = ""
                });
            }

            if (!string.IsNullOrEmpty(privilegeInfo.PrincipleType))
            {
                conditions.Add(new Condition
                {
                    LeftBrace = " AND ",
                    ParamName = "PrincipleType",
                    DbColumnName = "a.principle_type",
                    ParamValue = privilegeInfo.PrincipleType,
                    Operation = ConditionOperate.Equal,
                    RightBrace = "",
                    Logic = ""
                });
            }

            result = GenerateDal.CountByConditions(CommonSqlKey.GetPrivilegeListCount, conditions);

            return result;
        }


        /// <summary>
        /// 新增/修改会员信息
        /// </summary>
        /// <param name="memberInfo"></param>
        /// <returns></returns>
        public int PostData(PrivilegeModel privilegeInfo)
        {
            int result;

            string userClientId = HttpContextHandler.GetHeaderObj("UserClientId").ToString();
            privilegeInfo.ClientId = userClientId;
            privilegeInfo.CreateDate = DateTime.Now;
            
            result = GenerateDal.Create(privilegeInfo);

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

                PrivilegeModel privilegeInfo = new PrivilegeModel();
                privilegeInfo.PrivilegeId = id;
                GenerateDal.Delete<PrivilegeModel>(CommonSqlKey.DeletePrivilege, privilegeInfo);
              
                GenerateDal.CommitTransaction();
                return 1;
            }
            catch (Exception e)
            {
                GenerateDal.RollBack();
                return 0;
            }
        }

        public int UpdateData(PrivilegeModel privilegeInfo)
        {
            //操作日志
            return GenerateDal.Update(CommonSqlKey.UpdatePrivilege, privilegeInfo);
        }
    }
}
