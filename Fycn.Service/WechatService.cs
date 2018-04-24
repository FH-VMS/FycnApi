using Fycn.Interface;
using Fycn.Model.Wechat;
using Fycn.SqlDataAccess;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fycn.Service
{
    public class WechatService : AbstractService, IWechat
    {
        public int CreateMember(WechatMemberModel memberInfo, ClientMemberRelationModel clientMemberInfo)
        {
            try
            {
                GenerateDal.BeginTransaction();
                memberInfo.CreateDate = DateTime.Now;
                memberInfo.Privilege = null;
                GenerateDal.Create(memberInfo);
                clientMemberInfo.CreateTime= DateTime.Now;
                GenerateDal.Create(clientMemberInfo);
                GenerateDal.CommitTransaction();
                return 1;
            }
            catch(Exception ex)
            {
                GenerateDal.RollBack();
                return 0;
            }
        }

        public int CreateClientAndMemberRelation(ClientMemberRelationModel clientMemberInfo)
        {
            clientMemberInfo.CreateTime = DateTime.Now;
            return GenerateDal.Create(clientMemberInfo);
        }

        public List<WechatMemberModel> IsExistMember(WechatMemberModel memberInfo)
        {
            var conditions = new List<Condition>();
            if(string.IsNullOrEmpty(memberInfo.OpenId))
            {
                return null;
            }
            conditions.Add(new Condition
            {
                LeftBrace = " AND ",
                ParamName = "OpenId",
                DbColumnName = "a.openid",
                ParamValue = memberInfo.OpenId,
                Operation = ConditionOperate.Equal,
                RightBrace = " ",
                Logic = ""
            });
            return GenerateDal.LoadByConditions<WechatMemberModel>(CommonSqlKey.IsExistMember,conditions);
        }
    }
}
