﻿using Fycn.Interface;
using Fycn.Model.Pay;
using Fycn.Model.Privilege;
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
        public int CreateMember(WechatMemberModel memberInfo)
        {
            try
            {
                GenerateDal.BeginTransaction();
                memberInfo.CreateDate = DateTime.Now;
                memberInfo.Privilege = null;
                GenerateDal.Create(memberInfo);
                GenerateDal.CommitTransaction();
                return 1;
            }
            catch(Exception ex)
            {
                GenerateDal.RollBack();
                return 0;
            }
        }

      

        public List<WechatMemberModel> IsExistMember(WechatMemberModel memberInfo)
        {
            var conditions = new List<Condition>();
            if(string.IsNullOrEmpty(memberInfo.OpenId))
            {
                return null;
            }
            if (string.IsNullOrEmpty(memberInfo.ClientId))
            {
                return null;
            }
            conditions.Add(new Condition
            {
                LeftBrace = " AND ",
                ParamName = "OpenId",
                DbColumnName = "openid",
                ParamValue = memberInfo.OpenId,
                Operation = ConditionOperate.Equal,
                RightBrace = " ",
                Logic = ""
            });
            conditions.Add(new Condition
            {
                LeftBrace = " AND ",
                ParamName = "ClientId",
                DbColumnName = "client_id",
                ParamValue = memberInfo.ClientId,
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
           
                conditions.Add(new Condition
                {
                    LeftBrace = "",
                    ParamName = "WaresId",
                    DbColumnName = "",
                    ParamValue = waresIds,
                    Operation = ConditionOperate.None,
                    RightBrace = "",
                    Logic = ""
                });
            
            
          
                conditions.Add(new Condition
                {
                    LeftBrace = "",
                    ParamName = "WaresGroupIds",
                    DbColumnName = "",
                    ParamValue = waresGroupIds,
                    Operation = ConditionOperate.None,
                    RightBrace = "",
                    Logic = ""
                });
            
          
            return GenerateDal.LoadByConditions<ProductListModel>(CommonSqlKey.GetProdcutAndGroupList, conditions);
        }

        /// <summary>
        /// 7:待取货 8：已取货
        /// </summary>
        public List<SaleModel> GetHistorySalesList(string openId, int pageIndex, int pageSize)
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

            conditions.AddRange(CreatePaginConditions(pageIndex, pageSize));
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

        //微信支付结果插入数据库
        public int PostPayResultW(List<ProductPayModel> lstProductPay, string sellerId, string buyerId, string isConcern, string payDate)
        {
            try
            {
                GenerateDal.BeginTransaction();

                foreach (ProductPayModel payInfo in lstProductPay)
                {
                    SaleModel saleInfo = new SaleModel();
                    saleInfo.SalesIcId = Guid.NewGuid().ToString();
                    saleInfo.MachineId = "";
                    saleInfo.SalesDate = DateTime.Now;
                    saleInfo.SalesNumber = (payInfo.Number==0 ? 1 : payInfo.Number);
                    saleInfo.PayDate = TransStrToDateTime(payDate, "w");
                    saleInfo.PayInterface = "微信";
                    saleInfo.PayType = "微信";
                    saleInfo.TradeNo = payInfo.TradeNo;
                    saleInfo.GoodsId = "";
                    saleInfo.TradeStatus = 7;
                    saleInfo.MerchantId = sellerId;
                    saleInfo.BuyerId = buyerId;
                    saleInfo.IsWeixinConcern = isConcern;
                    saleInfo.TradeAmount = Convert.ToDouble(payInfo.TradeAmount);
                    saleInfo.ServiceCharge = Math.Round(Convert.ToDouble(payInfo.TradeAmount) * ConfigHandler.WeixinRate, 2, MidpointRounding.AwayFromZero);
                    saleInfo.WaresId = payInfo.WaresId;
                    saleInfo.WaresName = payInfo.WaresName;
                    GenerateDal.Create(saleInfo);
                    //更新存存
                    // UpdateCurrStock(keyJsonModel.m, keyTunnelInfo.tid, saleInfo.SalesNumber);
                }

                GenerateDal.CommitTransaction();
                return 1;
            }
            catch (Exception e)
            {
                GenerateDal.RollBack();
                return 0;
            }


        }

        private DateTime TransStrToDateTime(string strDate, string wOrA)
        {
            try
            {
                if (string.IsNullOrEmpty(strDate))
                {
                    return DateTime.Now;
                }
                if (wOrA == "w")
                {
                    if (strDate.Length == 14)
                    {
                        string year = strDate.Substring(0, 4);
                        string month = strDate.Substring(4, 2);
                        string day = strDate.Substring(6, 2);
                        string hour = strDate.Substring(8, 2);
                        string minute = strDate.Substring(10, 2);
                        string second = strDate.Substring(12, 2);
                        return Convert.ToDateTime(string.Format("{0}-{1}-{2} {3}:{4}:{5}", year, month, day, hour, minute, second));
                    }
                }
                else if (wOrA == "a")
                {
                    return Convert.ToDateTime(strDate);
                }
                return DateTime.Now;
            }
            catch (Exception e)
            {
                return DateTime.Now;
            }


        }
         
        // 取活动优惠券列表
        public List<PrivilegeModel> GetActivityPrivilegeList(PrivilegeModel privilegeInfo)
        {
            var conditions = new List<Condition>();

            conditions.Add(new Condition
            {
                LeftBrace = " AND ",
                ParamName = "ClientId",
                DbColumnName = "client_id",
                ParamValue = privilegeInfo.ClientId,
                Operation = ConditionOperate.Equal,
                RightBrace = "",
                Logic = ""
            });

            conditions.Add(new Condition
            {
                LeftBrace = " AND ",
                ParamName = "Numbers",
                DbColumnName = "numbers",
                ParamValue = 0,
                Operation = ConditionOperate.GreaterThan,
                RightBrace = "",
                Logic = ""
            });

            conditions.Add(new Condition
            {
                LeftBrace = " AND (",
                ParamName = "StartTime",
                DbColumnName = "start_time",
                ParamValue = DateTime.Now,
                Operation = ConditionOperate.LessThan,
                RightBrace = "",
                Logic = " OR "
            });
            conditions.Add(new Condition
            {
                LeftBrace = "",
                ParamName = "StartTime1",
                DbColumnName = "start_time",
                ParamValue = "",
                Operation = ConditionOperate.Null,
                RightBrace = ")",
                Logic = ""
            });

            if(!string.IsNullOrEmpty(privilegeInfo.PrincipleType))
            {
                    conditions.Add(new Condition
                    {
                        LeftBrace = " AND ",
                        ParamName = "PrincipleType",
                        DbColumnName = "principle_type",
                        ParamValue = privilegeInfo.PrincipleType,
                        Operation = ConditionOperate.Equal,
                        RightBrace = "",
                        Logic = ""
                    });
            }


            return GenerateDal.LoadByConditions<PrivilegeModel>(CommonSqlKey.GetActivityPrivilegeList, conditions);
        }


        //会员领取优惠券
        public int PostTicket(PrivilegeMemberRelationModel privilegeMemberInfo)
        {
            privilegeMemberInfo.GetDate=DateTime.Now;
            return GenerateDal.Create(privilegeMemberInfo);
        }

        public int IsExistTicket(PrivilegeMemberRelationModel privilegeMemberInfo)
        {
            var conditions=new List<Condition>();
            conditions.Add(new Condition
            {
                LeftBrace = " AND ",
                ParamName = "MemberId",
                DbColumnName = "member_id",
                ParamValue = privilegeMemberInfo.MemberId,
                Operation = ConditionOperate.Equal,
                RightBrace = "",
                Logic = ""
            });
            conditions.Add(new Condition
            {
                LeftBrace = " AND ",
                ParamName = "PrivilegeId",
                DbColumnName = "privilege_id",
                ParamValue = privilegeMemberInfo.PrivilegeId,
                Operation = ConditionOperate.Equal,
                RightBrace = "",
                Logic = ""
            });

            conditions.Add(new Condition
            {
                LeftBrace = " AND ",
                ParamName = "ClientId",
                DbColumnName = "client_id",
                ParamValue = privilegeMemberInfo.ClientId,
                Operation = ConditionOperate.Equal,
                RightBrace = "",
                Logic = ""
            });

            conditions.Add(new Condition
            {
                LeftBrace = " AND ",
                ParamName = "ExpireTime",
                DbColumnName = "expire_time",
                ParamValue = DateTime.Now,
                Operation = ConditionOperate.GreaterThan,
                RightBrace = "",
                Logic = ""
            });

            return GenerateDal.CountByConditions(CommonSqlKey.IsExistTicket, conditions);
        }

        //取得会员的优惠券
        public List<PrivilegeMemberRelationModel> GetPrivilegeByMemberId(PrivilegeMemberRelationModel privilegeMemberInfo)
        {
            var conditions=new List<Condition>();
            conditions.Add(new Condition
            {
                LeftBrace = " AND ",
                ParamName = "MemberId",
                DbColumnName = "member_id",
                ParamValue = privilegeMemberInfo.MemberId,
                Operation = ConditionOperate.Equal,
                RightBrace = "",
                Logic = ""
            });

            conditions.Add(new Condition
            {
                LeftBrace = " ",
                ParamName = "",
                DbColumnName = "get_data desc",
                ParamValue = "",
                Operation = ConditionOperate.OrderBy,
                RightBrace = "",
                Logic = ""
            });

            conditions.AddRange(CreatePaginConditions(privilegeMemberInfo.PageIndex, privilegeMemberInfo.PageSize));
            return GenerateDal.LoadByConditions<PrivilegeMemberRelationModel>(CommonSqlKey.GetPrivilegeByMemberId, conditions);
        }
    }
}
