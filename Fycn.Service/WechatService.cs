using Fycn.Interface;
using Fycn.Model.Pay;
using Fycn.Model.Product;
using Fycn.Model.Sale;
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

        public List<ProductListModel> GetWechatProductInfo(string waresIds)
        {
            var conditions = new List<Condition>();
            conditions.Add(new Condition
            {
                LeftBrace = " AND ",
                ParamName = "WaresId",
                DbColumnName = "a.wares_id",
                ParamValue = waresIds,
                Operation = ConditionOperate.INWithNoPara,
                RightBrace = " ",
                Logic = ""
            });
            return GenerateDal.LoadByConditions<ProductListModel>(CommonSqlKey.GetWechatProductInfo, conditions);
        }

        public List<ProductListModel> GetProdcutAndGroupList(string waresIds, string waresGroupIds)
        {
            var conditions = new List<Condition>();
            if(!string.IsNullOrEmpty(waresIds))
            {
                conditions.Add(new Condition
                {
                    LeftBrace = "  ",
                    ParamName = "WaresId",
                    DbColumnName = "",
                    ParamValue = waresIds,
                    Operation = ConditionOperate.None,
                    RightBrace = "",
                    Logic = ""
                });
            }
            
            if(!string.IsNullOrEmpty(waresGroupIds))
            {
                conditions.Add(new Condition
                {
                    LeftBrace = "  ",
                    ParamName = "WaresGroupId",
                    DbColumnName = "",
                    ParamValue = waresGroupIds,
                    Operation = ConditionOperate.None,
                    RightBrace = "",
                    Logic = ""
                });
            }
           
            return GenerateDal.LoadByConditions<ProductListModel>(CommonSqlKey.GetProdcutAndGroupList, conditions);
        }

        /// <summary>
        /// 7:待取货 8：已取货
        /// </summary>
        public List<SaleModel> GetHistorySalesList(string openId)
        {
            var conditions = new List<Condition>();
            
            conditions.Add(new Condition
            {
                LeftBrace = " AND ",
                ParamName = "OpenId",
                DbColumnName = "buyer_id",
                ParamValue = openId,
                Operation = ConditionOperate.Equal,
                RightBrace = "",
                Logic = ""
            });

            conditions.Add(new Condition
            {
                LeftBrace = " AND ",
                ParamName = "TradeStatus",
                DbColumnName = "trade_status",
                ParamValue = 7,
                Operation = ConditionOperate.NotEqual,
                RightBrace = "",
                Logic = ""
            });


            return GenerateDal.LoadByConditions<SaleModel>(CommonSqlKey.GetHistorySalesList, conditions);
        }

        public List<SaleModel> GetWaitingSalesList(string openId)
        {
            var conditions = new List<Condition>();

            conditions.Add(new Condition
            {
                LeftBrace = " AND ",
                ParamName = "OpenId",
                DbColumnName = "buyer_id",
                ParamValue = openId,
                Operation = ConditionOperate.Equal,
                RightBrace = "",
                Logic = ""
            });

            conditions.Add(new Condition
            {
                LeftBrace = " AND ",
                ParamName = "TradeStatus",
                DbColumnName = "trade_status",
                ParamValue = 7,
                Operation = ConditionOperate.Equal,
                RightBrace = "",
                Logic = ""
            });


            return GenerateDal.LoadByConditions<SaleModel>(CommonSqlKey.GetHistorySalesList, conditions);
        }
    }
}
