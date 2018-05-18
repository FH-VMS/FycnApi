using Fycn.Interface;
using Fycn.Model.Privilege;
using Fycn.Model.Wechat;
using Fycn.SqlDataAccess;
using Fycn.Utility;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fycn.Service
{
    public class MemberService : AbstractService, IBase<WechatMemberModel>, IMember
    {
        public List<WechatMemberModel> GetAll(WechatMemberModel wechatMemberInfo)
        {
            string userClientId = HttpContextHandler.GetHeaderObj("UserClientId").ToString();
            if (string.IsNullOrEmpty(userClientId))
            {
                return null;
            }
            var conditions = new List<Condition>();
            if(userClientId!="self")
            {
                conditions.Add(new Condition
                {
                    LeftBrace = " AND ",
                    ParamName = "ClientId",
                    DbColumnName = "a.client_id",
                    ParamValue = userClientId,
                    Operation = ConditionOperate.Equal,
                    RightBrace = " ",
                    Logic = ""
                });
            }
          
            
           
            if (!string.IsNullOrEmpty(wechatMemberInfo.NickName))
            {
                conditions.Add(new Condition
                {
                    LeftBrace = " AND ",
                    ParamName = "NickName",
                    DbColumnName = "a.nickname",
                    ParamValue = "%" + wechatMemberInfo.NickName + "%",
                    Operation = ConditionOperate.Like,
                    RightBrace = "",
                    Logic = ""
                });
            }
            

            conditions.AddRange(CreatePaginConditions(wechatMemberInfo.PageIndex, wechatMemberInfo.PageSize));

            return GenerateDal.LoadByConditions<WechatMemberModel>(CommonSqlKey.GetWechatMemberList, conditions);
        }


        public int GetCount(WechatMemberModel wechatMemberInfo)
        {
            var result = 0;

            string userClientId = HttpContextHandler.GetHeaderObj("UserClientId").ToString();
            if (string.IsNullOrEmpty(userClientId))
            {
                return 0;
            }
            var conditions = new List<Condition>();
            if (userClientId != "self")
            {
                conditions.Add(new Condition
                {
                    LeftBrace = " AND ",
                    ParamName = "ClientId",
                    DbColumnName = "a.client_id",
                    ParamValue = userClientId,
                    Operation = ConditionOperate.Equal,
                    RightBrace = " ",
                    Logic = ""
                });
            }

            if (!string.IsNullOrEmpty(wechatMemberInfo.NickName))
            {
                conditions.Add(new Condition
                {
                    LeftBrace = " AND ",
                    ParamName = "NickName",
                    DbColumnName = "a.nickname",
                    ParamValue = "%" + wechatMemberInfo.NickName + "%",
                    Operation = ConditionOperate.Like,
                    RightBrace = "",
                    Logic = ""
                });
            }

            result = GenerateDal.CountByConditions(CommonSqlKey.GetWechatMemberCount, conditions);

            return result;
        }


        /// <summary>
        /// 新增/修改会员信息
        /// </summary>
        /// <param name="memberInfo"></param>
        /// <returns></returns>
        public int PostData(WechatMemberModel wechatMemberInfo)
        {
            return 0;
        }
        
        public int GivePrivilegeTicket(PrivilegeMemberRelationModel privilegeMemberInfo)
        {
            privilegeMemberInfo.HappenDate = DateTime.Now;
            return GenerateDal.Create(privilegeMemberInfo);
        }

        /// <summary>
        /// 删除用户
        /// </summary>
        /// <returns></returns>
        public int DeleteData(string id)
        {
            return 0;
        }

        public int UpdateData(WechatMemberModel wechatMemberInfo)
        {
            //操作日志
            return 0;
        }
    }
}
