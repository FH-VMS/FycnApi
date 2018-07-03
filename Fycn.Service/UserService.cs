using Fycn.Interface;
using Fycn.Model.Machine;
using Fycn.Model.User;
using Fycn.SqlDataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fycn.Utility;

namespace Fycn.Service
{
    public class UserServicee : AbstractService, IBase<UserModel>
    {
        public List<UserModel> GetAll(UserModel userInfo)
        {
            string userClientId = HttpContextHandler.GetHeaderObj("UserClientId").ToString();
            if (string.IsNullOrEmpty(userClientId))
            {
                return null;
            }
           
            /*
            var dics = new Dictionary<string, object>();
            dics.Add("UserAccount", userInfo.UserAccount + "%");
            dics.Add("UserName", userInfo.UserName + "%");
            if (userInfo.PageIndex == 1)
            {
                dics.Add("PageIndex", userInfo.PageIndex-1);
            }
            else
            {
                dics.Add("PageIndex", userInfo.PageIndex);
            }
            dics.Add("PageSize", userInfo.PageSize);
            */

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
                DbColumnName = "a.usr_client_id",
                ParamValue = clientIds,
                Operation = ConditionOperate.INWithNoPara,
                RightBrace = "",
                Logic = ""
            });
            if (!string.IsNullOrEmpty(userInfo.UserAccount))
            {
                conditions.Add(new Condition
                {
                    LeftBrace = " AND ",
                    ParamName = "UserAccount",
                    DbColumnName = "a.usr_account",
                    ParamValue = "%" + userInfo.UserAccount + "%",
                    Operation = ConditionOperate.Like,
                    RightBrace = "",
                    Logic = ""
                });
            }

            if (!string.IsNullOrEmpty(userInfo.UserName))
            {
                conditions.Add(new Condition
                {
                    LeftBrace = " AND ",
                    ParamName = "UserName",
                    DbColumnName = "a.usr_name",
                    ParamValue = "%" + userInfo.UserName + "%",
                    Operation = ConditionOperate.Like,
                    RightBrace = "",
                    Logic = ""
                });
            }
       
           

           

           

         

            conditions.AddRange(CreatePaginConditions(userInfo.PageIndex, userInfo.PageSize));




            return GenerateDal.LoadByConditions<UserModel>(CommonSqlKey.GetUser, conditions);
        }

        private List<UserModel> GetCustomersFinalResult(List<UserModel> result)
        {
            if (result != null && result.Count > 0)
            {
                foreach (UserModel userInfo in result)
                {
                    var conditions = new List<Condition>();
                    conditions.Add(new Condition
                    {
                        LeftBrace = "",
                        ParamName = "ClientFatherId",
                        DbColumnName = "b.client_father_id",
                        ParamValue = userInfo.UserClientId,
                        Operation = ConditionOperate.Equal,
                        RightBrace = "",
                        Logic = ""
                    });
                    userInfo.children = GenerateDal.LoadByConditions<UserModel>(CommonSqlKey.GetUser, conditions);
                    GetCustomersFinalResult(userInfo.children);
                }
            }



            return result;
        }

        public int GetCount(UserModel userInfo)
        {
            var result = 0;


            string userClientId = HttpContextHandler.GetHeaderObj("UserClientId").ToString();
            if (string.IsNullOrEmpty(userClientId))
            {
                return 0;
            }
          
            /*
            var dics = new Dictionary<string, object>();
            dics.Add("UserAccount", userInfo.UserAccount + "%");
            dics.Add("UserName", userInfo.UserName + "%");
           



            if (userStatus == "100" || userStatus == "99")
            {
                dics.Add("ClientFatherId", "self");
            }
            else
            {
                dics.Add("ClientFatherId", userClientId);
            }
          

            result = GenerateDal.CountByDictionary<UserModel>(CommonSqlKey.GetUserCount, dics);
              */
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
                DbColumnName = "a.usr_client_id",
                ParamValue = clientIds,
                Operation = ConditionOperate.INWithNoPara,
                RightBrace = "",
                Logic = ""
            });
            if (!string.IsNullOrEmpty(userInfo.UserAccount))
            {
                conditions.Add(new Condition
                {
                    LeftBrace = " AND ",
                    ParamName = "UserAccount",
                    DbColumnName = "a.usr_account",
                    ParamValue = "%" + userInfo.UserAccount + "%",
                    Operation = ConditionOperate.Like,
                    RightBrace = "",
                    Logic = ""
                });
            }

            if (!string.IsNullOrEmpty(userInfo.UserName))
            {
                conditions.Add(new Condition
                {
                    LeftBrace = " AND ",
                    ParamName = "UserName",
                    DbColumnName = "a.usr_name",
                    ParamValue = "%" + userInfo.UserName + "%",
                    Operation = ConditionOperate.Like,
                    RightBrace = "",
                    Logic = ""
                });
            }
        
            result = GenerateDal.CountByConditions(CommonSqlKey.GetUserCount, conditions);
            return result;
        }


        /// <summary>
        /// 新增/修改会员信息
        /// </summary>
        /// <param name="memberInfo"></param>
        /// <returns></returns>
        public int PostData(UserModel userInfo)
        {
            int result;

            if (CheckUserName(userInfo.UserAccount) > 0)
            {
                return -1;
            } 
            userInfo.Id = Guid.NewGuid().ToString();
            userInfo.Sts = 1;
            userInfo.UserPassword = Md5.md5("888888", 16);
            result = GenerateDal.Create(userInfo);


            //操作日志
            OperationLogService operationService = new OperationLogService();
            operationService.PostData(new OperationLogModel() { Remark = userInfo.Id, OptContent = "新增或修改用户信息" });

            return result;
        }

        private int CheckUserName(string name)
        {
            var conditions = new List<Condition>();
            conditions.Add(new Condition
            {
                LeftBrace = " AND ",
                ParamName = "UserAccount",
                DbColumnName = "usr_account",
                ParamValue = name,
                Operation = ConditionOperate.Equal,
                RightBrace = "",
                Logic = ""
            });
            return GenerateDal.CountByConditions(CommonSqlKey.CheckUserExist, conditions);
        }

        /// <summary>
        /// 删除用户
        /// </summary>
        /// <returns></returns>
        public int DeleteData(string id)
        {
            UserModel userInfo = new UserModel();
            userInfo.Id = id;
            //操作日志
            OperationLogService operationService = new OperationLogService();
            operationService.PostData(new OperationLogModel() { Remark = userInfo.Id, OptContent = "删除用户" });
          return GenerateDal.Delete<UserModel>(CommonSqlKey.DeleteUser, userInfo); 
        }

        public int UpdateData(UserModel userInfo)
        {
            var conditions = new List<Condition>();
            conditions.Add(new Condition
            {
                LeftBrace = " AND ",
                ParamName = "UserAccount",
                DbColumnName = "usr_account",
                ParamValue = userInfo.UserAccount,
                Operation = ConditionOperate.Equal,
                RightBrace = "",
                Logic = ""
            });
            conditions.Add(new Condition
            {
                LeftBrace = " AND ",
                ParamName = "Id",
                DbColumnName = "id",
                ParamValue = userInfo.Id,
                Operation = ConditionOperate.NotEqual,
                RightBrace = "",
                Logic = ""
            });
            int result = GenerateDal.CountByConditions(CommonSqlKey.CheckUserExist, conditions);
            if (result > 0)
            {
                return -1;
            }
            // userInfo.UserPassword = Md5.md5(userInfo.UserPassword, 16);
            //操作日志
            OperationLogService operationService = new OperationLogService();
            operationService.PostData(new OperationLogModel() { Remark = userInfo.Id, OptContent = "更新用户" });
            return GenerateDal.Update(CommonSqlKey.UpdateUser, userInfo);
        }
       

    }
}
