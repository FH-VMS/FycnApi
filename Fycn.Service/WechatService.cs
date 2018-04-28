using Fycn.Interface;
using Fycn.Model.Pay;
using Fycn.Model.Product;
using Fycn.Model.Wechat;
using Fycn.SqlDataAccess;
using Fycn.Utility;
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

        //根据客户取商品类型
        public List<ProductTypeModel> GetProdcutTypeByClientId(string clientId)
        {
            var conditions = new List<Condition>();
            conditions.Add(new Condition
            {
                LeftBrace = " AND ",
                ParamName = "Client",
                DbColumnName = "client_id",
                ParamValue = clientId,
                Operation = ConditionOperate.Equal,
                RightBrace = " ",
                Logic = ""
            });

            conditions.Add(new Condition
            {
                LeftBrace = "  ",
                ParamName = "Sequence",
                DbColumnName = "sequence",
                ParamValue = "asc",
                Operation = ConditionOperate.OrderBy,
                RightBrace = "",
                Logic = ""
            });

            return GenerateDal.LoadByConditions<ProductTypeModel>(CommonSqlKey.GetProductTypeByClientId, conditions);
        }

        //根据商品类型取商品
        public List<ProductListModel> GetProdcutByTypeAndClient(string typeId,string clientId)
        {
            var conditions = new List<Condition>();
            conditions.Add(new Condition
            {
                LeftBrace = " AND ",
                ParamName = "WaresTypeId",
                DbColumnName = "a.wares_type_id",
                ParamValue = typeId,
                Operation = ConditionOperate.Equal,
                RightBrace = " ",
                Logic = ""
            });
            if(!string.IsNullOrEmpty(clientId))
            {
                conditions.Add(new Condition
                {
                    LeftBrace = " AND ",
                    ParamName = "ClientId",
                    DbColumnName = "a.client_id",
                    ParamValue = clientId,
                    Operation = ConditionOperate.Equal,
                    RightBrace = " ",
                    Logic = ""
                });
            }

            conditions.Add(new Condition
            {
                LeftBrace = "  ",
                ParamName = "ResourceUrl",
                DbColumnName = "",
                ParamValue = ConfigHandler.ResourceUrl,
                Operation = ConditionOperate.None,
                RightBrace = "",
                Logic = ""
            });

            return GenerateDal.LoadByConditions<ProductListModel>(CommonSqlKey.GetProductByTypeAndClientId, conditions);
        }
    }
}
