using Fycn.Interface;
using Fycn.Model.Machine;
using Fycn.Model.Pay;
using Fycn.Model.Privilege;
using Fycn.Model.Product;
using Fycn.Model.Sale;
using Fycn.Model.Wechat;
using Fycn.SqlDataAccess;
using Fycn.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
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
            if (!string.IsNullOrEmpty(typeId))
            {
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
            }
            
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
                LeftBrace = " AND ",
                ParamName = "WaresTypeId1",
                DbColumnName = "a.wares_type_id",
                ParamValue = "",
                Operation = ConditionOperate.NotEqual,
                RightBrace = "",
                Logic = ""
            });

            conditions.Add(new Condition
            {
                LeftBrace = " AND ",
                ParamName = "WaresStatus",
                DbColumnName = "a.wares_status",
                ParamValue = 1,
                Operation = ConditionOperate.Equal,
                RightBrace = "",
                Logic = ""
            });

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

            conditions.Add(new Condition
            {
                LeftBrace = "  ",
                ParamName = "Sequence",
                DbColumnName = "b.wares_type_id,b.sequence",
                ParamValue = "asc",
                Operation = ConditionOperate.OrderBy,
                RightBrace = "",
                Logic = ""
            });

            return GenerateDal.LoadByConditions<ProductListModel>(CommonSqlKey.GetProdcutAndGroupList, conditions);
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
            string waresPara = string.Empty;
            string groupPara = string.Empty;
            if(!string.IsNullOrEmpty(waresIds))
            {
                waresPara = "'" + waresIds.Replace(",", "','") + "'";
            }
            //dics.Add("")
            
            conditions.Add(new Condition
                {
                    LeftBrace = " AND ",
                    ParamName = "WaresId",
                    DbColumnName = "a.wares_id",
                    ParamValue = waresPara,
                    Operation = ConditionOperate.INWithNoPara,
                    RightBrace = "",
                    Logic = ""
                });


            /*
                  conditions.Add(new Condition
                  {
                      LeftBrace = "",
                      ParamName = "WaresGroupIds",
                      DbColumnName = "",
                      ParamValue = groupPara,
                      Operation = ConditionOperate.None,
                      RightBrace = "",
                      Logic = ""
                  });
              */

            return GenerateDal.LoadByConditions<ProductListModel>(CommonSqlKey.GetPayProdcutAndGroupList, conditions);
        }

        // 根据waresId 取对应的商品或商品组
        public List<ProductListModel> GetProdcutAndGroupByWaresId(string waresId)
        {
            var conditions = new List<Condition>();
            //dics.Add("")
            
            conditions.Add(new Condition
                {
                    LeftBrace = " AND ",
                    ParamName = "WaresId",
                    DbColumnName = "a.wares_id",
                    ParamValue = waresId,
                    Operation = ConditionOperate.Equal,
                    RightBrace = "",
                    Logic = ""
                });


            /*
                  conditions.Add(new Condition
                  {
                      LeftBrace = "",
                      ParamName = "WaresGroupIds",
                      DbColumnName = "",
                      ParamValue = groupPara,
                      Operation = ConditionOperate.None,
                      RightBrace = "",
                      Logic = ""
                  });
              */

            return GenerateDal.LoadByConditions<ProductListModel>(CommonSqlKey.GetPayProdcutAndGroupList, conditions);
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

        public List<ClientSalesRelationModel> GetWaitingSalesList(string openId)
        {
            var conditions = new List<Condition>();

            conditions.Add(new Condition
            {
                LeftBrace = " AND ",
                ParamName = "MemberId",
                DbColumnName = "member_id",
                ParamValue = openId,
                Operation = ConditionOperate.Equal,
                RightBrace = "",
                Logic = ""
            });

            conditions.Add(new Condition
            {
                LeftBrace = " AND ",
                ParamName = "CodeStatus",
                DbColumnName = "code_status",
                ParamValue = 1,
                Operation = ConditionOperate.Equal,
                RightBrace = "",
                Logic = ""
            });


            return GenerateDal.LoadByConditions<ClientSalesRelationModel>(CommonSqlKey.GetWaitingSalesList, conditions);
        }

        //微信支付结果插入数据库
        public int PostPayResultW(List<ProductPayModel> lstProductPay, string sellerId, string buyerId, string isConcern, string payDate,string clientIdAndPrivilege)
        {
            try
            {
                GenerateDal.BeginTransaction();
                string[] clientAndPrivilegeArr = clientIdAndPrivilege.Split('~');
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
                    saleInfo.TradeAmount = Convert.ToDouble(payInfo.TradeAmount) * saleInfo.SalesNumber;
                    saleInfo.ServiceCharge = Math.Round(Convert.ToDouble(payInfo.TradeAmount) * saleInfo.SalesNumber * ConfigHandler.WeixinRate, 2, MidpointRounding.AwayFromZero);
                    saleInfo.WaresId = payInfo.WaresId;
                    saleInfo.WaresName = payInfo.WaresName;
                    GenerateDal.Create(saleInfo);

                    //更新存存
                    // UpdateCurrStock(keyJsonModel.m, keyTunnelInfo.tid, saleInfo.SalesNumber);
                    //生成取货码
                    
                    if (payInfo.IsGroup != 1)
                    {
                        for (int i = 0; i < payInfo.Number; i++)
                        {
                            ClientSalesRelationModel clientSalesInfo = new ClientSalesRelationModel();
                            clientSalesInfo.ClientId = clientAndPrivilegeArr[0];
                            clientSalesInfo.TradeNo = payInfo.TradeNo;
                            clientSalesInfo.PickupNo = GeneratePickupCode();
                            clientSalesInfo.WaresId = payInfo.WaresId;
                            clientSalesInfo.WaresName = payInfo.WaresName;
                            clientSalesInfo.CodeStatus = 1;
                            clientSalesInfo.CreateDate = DateTime.Now;
                            clientSalesInfo.MemberId = buyerId;
                            GenerateDal.Create(clientSalesInfo);
                        }
                    }
                    else
                    {
                        for (int i = 0; i < payInfo.Number; i++)
                        {
                            var listProduct = GetProductByGroupId(payInfo.WaresId);
                            foreach(ProductListModel lInfo in listProduct)
                            {
                                if(lInfo.Numbers==0)
                                {
                                    lInfo.Numbers = 1;
                                }
                                for(int j = 0; j < lInfo.Numbers; j++)
                                {
                                    ClientSalesRelationModel clientSalesInfo = new ClientSalesRelationModel();
                                    clientSalesInfo.ClientId = clientAndPrivilegeArr[0];
                                    clientSalesInfo.TradeNo = payInfo.TradeNo;
                                    clientSalesInfo.PickupNo = GeneratePickupCode();
                                    clientSalesInfo.WaresId = lInfo.WaresId;
                                    clientSalesInfo.WaresName = lInfo.WaresName;
                                    clientSalesInfo.CodeStatus = 1;
                                    clientSalesInfo.CreateDate = DateTime.Now;
                                    clientSalesInfo.MemberId = buyerId;
                                    GenerateDal.Create(clientSalesInfo);
                                }
                                
                            }
                        }
                    }
                }
                if (clientAndPrivilegeArr.Length > 1)
                {
                    string[] ids = clientAndPrivilegeArr[1].Split(',');
                    foreach(string id in ids)
                    {
                       if(!string.IsNullOrEmpty(id))
                       {
                            PrivilegeMemberRelationModel privilegeMemberInfo = new PrivilegeMemberRelationModel();
                            privilegeMemberInfo.PrivilegeStatus = 2;
                            privilegeMemberInfo.TradeNo = lstProductPay[0].TradeNo;
                            privilegeMemberInfo.HappenDate = DateTime.Now;
                            privilegeMemberInfo.Id = id;
                            GenerateDal.Update(CommonSqlKey.UseTicket, privilegeMemberInfo);
                       }
                    }
                   
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

        private string GeneratePickupCode()
        {
            try
            {
                string tmpStr = Guid.NewGuid().GetHashCode().ToString().Substring(1, 8);
                RedisHelper redisHelper3 = new RedisHelper(3);
                if (redisHelper3.KeyExists(tmpStr))
                {
                    return GeneratePickupCode();
                    // return "";
                }
                else
                {
                    redisHelper3.StringSet(tmpStr, "");
                    return tmpStr;
                }
            }
            catch
            {
                return Guid.NewGuid().GetHashCode().ToString();
                // return "";
            }
            
        }

        private List<ProductListModel> GetProductByGroupId(string groupId)
        {
            var conditions = new List<Condition>();
            conditions.Add(new Condition
            {
                LeftBrace = " AND ",
                ParamName = "WaresGroupId",
                DbColumnName = "a.wares_group_id",
                ParamValue = groupId,
                Operation = ConditionOperate.Equal,
                RightBrace = "",
                Logic = ""
            });

           return GenerateDal.LoadByConditions<ProductListModel>(CommonSqlKey.GetProductByGroupId, conditions);
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

        //取活动列表
        public List<ActivityModel> GetActivityList(ActivityModel activityeInfo)
        {
            var conditions = new List<Condition>();

            conditions.Add(new Condition
            {
                LeftBrace = " AND ",
                ParamName = "ClientId",
                DbColumnName = "client_id",
                ParamValue = activityeInfo.ClientId,
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

            conditions.Add(new Condition
            {
                LeftBrace = " AND ",
                ParamName = "EndTime",
                DbColumnName = "end_time",
                ParamValue = DateTime.Now,
                Operation = ConditionOperate.GreaterThan,
                RightBrace = "",
                Logic = ""
            });

            if (!string.IsNullOrEmpty(activityeInfo.ActivityType))
            {
                conditions.Add(new Condition
                {
                    LeftBrace = " AND ",
                    ParamName = "ActivityType",
                    DbColumnName = "activity_type",
                    ParamValue = activityeInfo.ActivityType,
                    Operation = ConditionOperate.Equal,
                    RightBrace = "",
                    Logic = ""
                });
            }

            return GenerateDal.LoadByConditions<ActivityModel>(CommonSqlKey.GetWechatActivityList, conditions);
        }
         
        // 取活动优惠券列表
        public List<PrivilegeModel> GetActivityPrivilegeListById(ActivityPrivilegeRelationModel privilegeRelationInfo)
        {
            var conditions = new List<Condition>();
            
            if (!string.IsNullOrEmpty(privilegeRelationInfo.ActivityId))
            {
                conditions.Add(new Condition
                {
                    LeftBrace = " AND ",
                    ParamName = "ActivityId",
                    DbColumnName = "a.activity_id",
                    ParamValue = privilegeRelationInfo.ActivityId,
                    Operation = ConditionOperate.Equal,
                    RightBrace = "",
                    Logic = ""
                });
            }
            


            return GenerateDal.LoadByConditions<PrivilegeModel>(CommonSqlKey.GetActivityPrivilegeListById, conditions);
        }


        //会员领取优惠券
        public int PostTicket(PrivilegeMemberRelationModel privilegeMemberInfo)
        {
            try
            {
                GenerateDal.BeginTransaction();
                privilegeMemberInfo.Id = Guid.NewGuid().ToString();
                privilegeMemberInfo.GetDate=DateTime.Now;
                GenerateDal.Create(privilegeMemberInfo);
                if(!string.IsNullOrEmpty(privilegeMemberInfo.ActivityId))
                {
                    UpdateCurrTicket(privilegeMemberInfo.ActivityId);
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

        //领券后更新减券的数量
        private void UpdateCurrTicket(string activityId)
        {
            ActivityModel activityInfo = new ActivityModel();
            activityInfo.Id = activityId;
            GenerateDal.Execute(CommonSqlKey.UpdateActivityNumberById, activityInfo);
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
                LeftBrace = " AND ",
                ParamName = "PrincipleType",
                DbColumnName = "principle_type",
                ParamValue = 4,
                Operation = ConditionOperate.NotEqual,
                RightBrace = "",
                Logic = ""
            });

            conditions.Add(new Condition
            {
                LeftBrace = " ",
                ParamName = "",
                DbColumnName = "get_date desc",
                ParamValue = "",
                Operation = ConditionOperate.OrderBy,
                RightBrace = "",
                Logic = ""
            });

            conditions.AddRange(CreatePaginConditions(privilegeMemberInfo.PageIndex, privilegeMemberInfo.PageSize));
            return GenerateDal.LoadByConditions<PrivilegeMemberRelationModel>(CommonSqlKey.GetPrivilegeByMemberId, conditions);
        }

        //取得会员的优惠券总数
        public int GetPrivilegeCountByMemberId(PrivilegeMemberRelationModel privilegeMemberInfo)
        {
            var conditions = new List<Condition>();
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
                DbColumnName = "get_date desc",
                ParamValue = "",
                Operation = ConditionOperate.OrderBy,
                RightBrace = "",
                Logic = ""
            });
            
            return GenerateDal.CountByConditions(CommonSqlKey.GetPrivilegeCountByMemberId, conditions);
        }

        //取指定时间内用户优惠券数量
        public int GetTicketCountByTime(PrivilegeMemberRelationModel privilegeMemberInfo)
        {
            var conditions = new List<Condition>();
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
                    ParamName = "ActivityType",
                    DbColumnName = "activity_type",
                    ParamValue = privilegeMemberInfo.ActivityType,
                    Operation = ConditionOperate.Equal,
                    RightBrace = "",
                    Logic = ""
                });

                conditions.Add(new Condition
                {
                    LeftBrace = " AND ",
                    ParamName = "StartDateDate",
                    DbColumnName = "get_date",
                    ParamValue = DateTime.Now.ToString("yyyy/MM/dd") + " 00:00:00",
                    Operation = ConditionOperate.GreaterThan,
                    RightBrace = "",
                    Logic = ""
                });
                conditions.Add(new Condition
                {
                    LeftBrace = " AND ",
                    ParamName = "EndDateDate",
                    DbColumnName = "get_date",
                    ParamValue = DateTime.Now.AddDays(1).ToString("yyyy/MM/dd") + " 00:00:00",
                    Operation = ConditionOperate.Less,
                    RightBrace = "",
                    Logic = ""
                });
               

                //取该用户当天的券数量
                int result = GenerateDal.CountByConditions(CommonSqlKey.IsExistTicket, conditions);
                return result;

            

            return 0;
            
            
        }

        // 取可以使用的优惠券
        public List<PrivilegeMemberRelationModel> GetCanUsePrivilege(PrivilegeMemberRelationModel privilegeMemberInfo, string privilegeIds,ref decimal totalFee, List<ProductPayModel> lstPayInfo)
        {
            var conditions = new List<Condition>();
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
                ParamName = "PrincipleType",
                DbColumnName = "principle_type",
                ParamValue = 4,
                Operation = ConditionOperate.NotEqual,
                RightBrace = "",
                Logic = ""
            });

            conditions.Add(new Condition
            {
                LeftBrace = " AND ",
                ParamName = "PrivilegeStatus",
                DbColumnName = "privilege_status",
                ParamValue = 1,
                Operation = ConditionOperate.Equal,
                RightBrace = "",
                Logic = ""
            });
            
            conditions.Add(new Condition
            {
                LeftBrace = " AND (",
                ParamName = "UseMoneyLimit",
                DbColumnName = "use_money_limit",
                ParamValue = 0,
                Operation = ConditionOperate.Equal,
                RightBrace = "",
                Logic = "OR"
            });

            conditions.Add(new Condition
            {
                LeftBrace = "",
                ParamName = "UseMoneyLimit1",
                DbColumnName = "use_money_limit",
                ParamValue = totalFee,
                Operation = ConditionOperate.LessThan,
                RightBrace = ")",
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
            /* 
            if(!string.IsNullOrEmpty(privilegeIds))
            {
                conditions.Add(new Condition
                {
                    LeftBrace = " AND ",
                    ParamName = "Id",
                    DbColumnName = "id",
                    ParamValue = privilegeIds,
                    Operation = ConditionOperate.Equal,
                    RightBrace = "",
                    Logic = ""
                });
            }
           */

            conditions.Add(new Condition
            {
                LeftBrace = " ",
                ParamName = "",
                DbColumnName = "money desc",
                ParamValue = "",
                Operation = ConditionOperate.OrderBy,
                RightBrace = "",
                Logic = ""
            });

            List<PrivilegeMemberRelationModel> lstPrivilege = GenerateDal.LoadByConditions<PrivilegeMemberRelationModel>(CommonSqlKey.GetPrivilegeByMemberId, conditions);
            if(lstPrivilege.Count==0)
            {
                return lstPrivilege;
            }
            decimal canOverLayReducerMoney = totalFee;
            decimal cannotOverlayReducerMoney=totalFee;
            List<decimal> lstDeci=new List<decimal>();
            var canOverLay = (from m in lstPrivilege
                                where m.IsOverlay==1
                                select m).ToList();
                var cannotOverlay = (from m in lstPrivilege
                                where m.IsOverlay!=1
                                select m).ToList();
            List<PrivilegeMemberRelationModel> lstReturnCanOverlay = new List<PrivilegeMemberRelationModel>();
                foreach (PrivilegeMemberRelationModel privilegeRelationInfo in canOverLay)
                {
                    switch(privilegeRelationInfo.PrincipleType)
                    {
                        case "1":  //满减券
                        canOverLayReducerMoney=canOverLayReducerMoney-privilegeRelationInfo.Money;
                        lstReturnCanOverlay.Add(privilegeRelationInfo);
                        break;
                        case "2":  //折扣券
                        canOverLayReducerMoney=canOverLayReducerMoney*(privilegeRelationInfo.Discount/10);
                        lstReturnCanOverlay.Add(privilegeRelationInfo);
                        break;
                        case "3":  //赠品券
                        var tmpWares = (from n in lstPayInfo
                                       where n.WaresId==privilegeRelationInfo.BindProductIds
                                       select n).ToList();
                        if(tmpWares.Count>0){
                            lstReturnCanOverlay.Add(privilegeRelationInfo);
                            canOverLayReducerMoney = canOverLayReducerMoney-Convert.ToDecimal(tmpWares[0].TradeAmount);
                        }
                        break;
                        /* 
                        case "4":  //满减券绑定商品
                        break;
                        case "5":  //折扣券券绑定商品
                        break;
                        */
                    }
                }
            List<PrivilegeMemberRelationModel> lstReturnCannotOverlay = new List<PrivilegeMemberRelationModel>();
            if (cannotOverlay.Count>0)
                {

                    foreach(PrivilegeMemberRelationModel privilegeRelationInfo in cannotOverlay) 
                    {
                        switch(privilegeRelationInfo.PrincipleType)
                        {
                            case "1":  //满减券
                            lstDeci.Add(totalFee-privilegeRelationInfo.Money);
                            lstReturnCannotOverlay.Add(privilegeRelationInfo);
                            break;
                            case "2":  //折扣券
                            lstDeci.Add(totalFee*(privilegeRelationInfo.Discount/10));
                            lstReturnCannotOverlay.Add(privilegeRelationInfo);
                            break;
                            case "3":  //赠品券
                            var tmpWares = (from n in lstPayInfo
                                        where n.WaresId==privilegeRelationInfo.BindProductIds
                                        select n).ToList();
                            if(tmpWares.Count>0){
                                lstReturnCannotOverlay.Add(privilegeRelationInfo);
                                lstDeci.Add(totalFee-Convert.ToDecimal(tmpWares[0].TradeAmount));
                            }
                            break;
                            /* 
                            case "4":  //满减券绑定商品
                            break;
                            case "5":  //折扣券券绑定商品
                            break;
                            */
                        }
                            

                    }
                }
                if(lstDeci.Count==0)
                {
                  totalFee=canOverLayReducerMoney;
                  return lstReturnCanOverlay;
                }
                
                var minMoney = lstDeci.Select(w => w).Min();
                if(minMoney<=canOverLayReducerMoney)
                {
                    int index =lstDeci.FindIndex(x => x == minMoney);
                    List<PrivilegeMemberRelationModel> ret = new List<PrivilegeMemberRelationModel>();
                    ret.Add(lstReturnCannotOverlay[index]);
                    totalFee=minMoney;
                    return ret;
                } 
                else 
                {
                  totalFee=canOverLayReducerMoney;
                  return lstReturnCanOverlay;
                }
                
                
            
        }

        public List<PrivilegeMemberRelationModel> GetChosenPrivilege(PrivilegeMemberRelationModel privilegeMemberInfo, string ids, ref decimal totalFee, List<ProductPayModel> lstPayInfo)
        {
            var conditions = new List<Condition>();
            conditions.Add(new Condition
            {
                LeftBrace = " AND ",
                ParamName = "MemberId",
                DbColumnName = "a.member_id",
                ParamValue = privilegeMemberInfo.MemberId,
                Operation = ConditionOperate.Equal,
                RightBrace = "",
                Logic = ""
            });


            ids = "'" + ids.Replace(",", "','") + "'";
            
            conditions.Add(new Condition
            {
                LeftBrace = " AND ",
                ParamName = "Id",
                DbColumnName = "a.id",
                ParamValue = ids,
                Operation = ConditionOperate.INWithNoPara,
                RightBrace = " ",
                Logic = ""
            });


            List<PrivilegeMemberRelationModel> lstPrivilege = GenerateDal.LoadByConditions<PrivilegeMemberRelationModel>(CommonSqlKey.GetPrivilegeByMemberId, conditions);
            if (lstPrivilege.Count == 0)
            {
                return lstPrivilege;
            }
            decimal canOverLayReducerMoney = totalFee;
            decimal cannotOverlayReducerMoney = totalFee;
            List<decimal> lstDeci = new List<decimal>();
            var canOverLay = (from m in lstPrivilege
                              where m.IsOverlay == 1
                              select m).ToList();
            var cannotOverlay = (from m in lstPrivilege
                                 where m.IsOverlay != 1
                                 select m).ToList();
            List<PrivilegeMemberRelationModel> lstReturnCanOverlay = new List<PrivilegeMemberRelationModel>();
            foreach (PrivilegeMemberRelationModel privilegeRelationInfo in canOverLay)
            {
                switch (privilegeRelationInfo.PrincipleType)
                {
                    case "1":  //满减券
                        canOverLayReducerMoney = canOverLayReducerMoney - privilegeRelationInfo.Money;
                        lstReturnCanOverlay.Add(privilegeRelationInfo);
                        break;
                    case "2":  //折扣券
                        canOverLayReducerMoney = canOverLayReducerMoney * (privilegeRelationInfo.Discount / 10);
                        lstReturnCanOverlay.Add(privilegeRelationInfo);
                        break;
                    case "3":  //赠品券
                        var tmpWares = (from n in lstPayInfo
                                        where n.WaresId == privilegeRelationInfo.BindProductIds
                                        select n).ToList();
                        if (tmpWares.Count > 0)
                        {
                            lstReturnCanOverlay.Add(privilegeRelationInfo);
                            canOverLayReducerMoney = canOverLayReducerMoney - Convert.ToDecimal(tmpWares[0].TradeAmount);
                        }
                        break;
                        /* 
                        case "4":  //满减券绑定商品
                        break;
                        case "5":  //折扣券券绑定商品
                        break;
                        */
                }
            }
            List<PrivilegeMemberRelationModel> lstReturnCannotOverlay = new List<PrivilegeMemberRelationModel>();
            if (cannotOverlay.Count > 0)
            {

                foreach (PrivilegeMemberRelationModel privilegeRelationInfo in cannotOverlay)
                {
                    switch (privilegeRelationInfo.PrincipleType)
                    {
                        case "1":  //满减券
                            lstDeci.Add(totalFee - privilegeRelationInfo.Money);
                            lstReturnCannotOverlay.Add(privilegeRelationInfo);
                            break;
                        case "2":  //折扣券
                            lstDeci.Add(totalFee * (privilegeRelationInfo.Discount / 10));
                            lstReturnCannotOverlay.Add(privilegeRelationInfo);
                            break;
                        case "3":  //赠品券
                            var tmpWares = (from n in lstPayInfo
                                            where n.WaresId == privilegeRelationInfo.BindProductIds
                                            select n).ToList();
                            if (tmpWares.Count > 0)
                            {
                                lstReturnCannotOverlay.Add(privilegeRelationInfo);
                                lstDeci.Add(totalFee - Convert.ToDecimal(tmpWares[0].TradeAmount));
                            }
                            break;
                            /* 
                            case "4":  //满减券绑定商品
                            break;
                            case "5":  //折扣券券绑定商品
                            break;
                            */
                    }


                }
            }
            if (lstDeci.Count == 0)
            {
                totalFee = canOverLayReducerMoney;
                return lstReturnCanOverlay;
            }

            var minMoney = lstDeci.Select(w => w).Min();
            if (minMoney <= canOverLayReducerMoney)
            {
                int index = lstDeci.FindIndex(x => x == minMoney);
                List<PrivilegeMemberRelationModel> ret = new List<PrivilegeMemberRelationModel>();
                ret.Add(lstReturnCannotOverlay[index]);
                totalFee = minMoney;
                return ret;
            }
            else
            {
                totalFee = canOverLayReducerMoney;
                return lstReturnCanOverlay;
            }
        }

        public List<MachineLocationModel> GetMachineLocations(MachineLocationModel machineLocationInfo)
        {
            var conditions = new List<Condition>();
            if(string.IsNullOrEmpty(machineLocationInfo.Longitude))
            {
                machineLocationInfo.Longitude = "120.1648200000";
            }
            if(string.IsNullOrEmpty(machineLocationInfo.Latitude))
            {
                machineLocationInfo.Latitude = "30.2435500000";
            }

            string userClientId = machineLocationInfo.ClientId;
            if(string.IsNullOrEmpty(userClientId))
            {
                return null;
            }
            string clientIds = new CommonService().GetClientIds(userClientId);
            if (clientIds.Contains("self"))
            {
                clientIds = "'" + clientIds.Replace(",", "','") + "'";
            }
            conditions.Add(new Condition
            {
                LeftBrace = " AND ",
                ParamName = "ClientId",
                DbColumnName = "b.client_id",
                ParamValue = clientIds,
                Operation = ConditionOperate.INWithNoPara,
                RightBrace = " ",
                Logic = ""
            });

            conditions.Add(new Condition
            {
                LeftBrace = "  ",
                ParamName = "Longitude",
                DbColumnName = "",
                ParamValue = machineLocationInfo.Longitude,
                Operation = ConditionOperate.None,
                RightBrace = "",
                Logic = ""
            });
            conditions.Add(new Condition
            {
                LeftBrace = "  ",
                ParamName = "Latitude",
                DbColumnName = "",
                ParamValue = machineLocationInfo.Latitude,
                Operation = ConditionOperate.None,
                RightBrace = "",
                Logic = ""
            });

            


            conditions.Add(new Condition
            {
                LeftBrace = "  ",
                ParamName = "Sequence",
                DbColumnName = "Distance",
                ParamValue = "asc",
                Operation = ConditionOperate.OrderBy,
                RightBrace = "",
                Logic = ""
            });
            conditions.AddRange(CreatePaginConditions(machineLocationInfo.PageIndex, machineLocationInfo.PageSize));

            return GenerateDal.LoadByConditions<MachineLocationModel>(CommonSqlKey.GetMachineLocations, conditions);
        }

        public List<PrivilegeMemberRelationModel> GetNoneExpirePrivilegeByMemberId(PrivilegeMemberRelationModel privilegeMemberInfo)
        {
            var conditions = new List<Condition>();
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
                ParamName = "PrincipleType",
                DbColumnName = "principle_type",
                ParamValue = 4,
                Operation = ConditionOperate.NotEqual,
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

            conditions.Add(new Condition
            {
                LeftBrace = " ",
                ParamName = "",
                DbColumnName = "get_date desc",
                ParamValue = "",
                Operation = ConditionOperate.OrderBy,
                RightBrace = "",
                Logic = ""
            });

            return GenerateDal.LoadByConditions<PrivilegeMemberRelationModel>(CommonSqlKey.GetPrivilegeByMemberId, conditions);
        }

        //验证取货码
        public List<ClientSalesRelationModel> VerifyPickupCode(ClientSalesRelationModel clientSalesInfo)
        {
            try
            {
                if (string.IsNullOrEmpty(clientSalesInfo.MachineId))
                {
                    return null;
                }

                if (string.IsNullOrEmpty(clientSalesInfo.PickupNo))
                {
                    return null;
                }
                var conditions = new List<Condition>();
                conditions.Add(new Condition
                {
                    LeftBrace = " AND ",
                    ParamName = "PickupCode",
                    DbColumnName = "a.pickup_code",
                    ParamValue = clientSalesInfo.PickupNo,
                    Operation = ConditionOperate.Equal,
                    RightBrace = "",
                    Logic = ""
                });

                conditions.Add(new Condition
                {
                    LeftBrace = " AND ",
                    ParamName = "MachineId",
                    DbColumnName = "c.machine_id",
                    ParamValue = clientSalesInfo.MachineId,
                    Operation = ConditionOperate.Equal,
                    RightBrace = "",
                    Logic = ""
                });

                conditions.Add(new Condition
                {
                    LeftBrace = " AND ",
                    ParamName = "CodeStatus",
                    DbColumnName = "a.code_status",
                    ParamValue = 1,
                    Operation = ConditionOperate.Equal,
                    RightBrace = "",
                    Logic = ""
                });

                return GenerateDal.LoadByConditions<ClientSalesRelationModel>(CommonSqlKey.VerifyPickupCode, conditions);
            }
            catch (Exception e)
            {
                return new List<ClientSalesRelationModel>();
            }
            

        }


        // 上报出货码取货结果
        public int PutPayResultByPickupCode(ClientSalesRelationModel clinetSalesInfo)
        {
            if(clinetSalesInfo.TotalNum==0)
            {
                return 2;
            }
            try
            {
                var conditions = new List<Condition>();

                conditions.Add(new Condition
                {
                    LeftBrace = " AND ",
                    ParamName = "PickupCode",
                    DbColumnName = "pickup_code",
                    ParamValue = clinetSalesInfo.PickupNo,
                    Operation = ConditionOperate.Equal,
                    RightBrace = "",
                    Logic = ""
                });
                conditions.Add(new Condition
                {
                    LeftBrace = " AND ",
                    ParamName = "CodeStatus",
                    DbColumnName = "code_status",
                    ParamValue = 1,
                    Operation = ConditionOperate.Equal,
                    RightBrace = "",
                    Logic = ""
                });
                List<ClientSalesRelationModel> lstSaleInfo = GenerateDal.LoadByConditions<ClientSalesRelationModel>(CommonSqlKey.GetWaitingSalesList, conditions);
                if (lstSaleInfo.Count == 0)
                {
                    return 3;
                }
                GenerateDal.BeginTransaction();

                ClientSalesRelationModel saleRelationModel = lstSaleInfo[0];
                if (saleRelationModel != null && saleRelationModel.CodeStatus == 1)
                {
                    if(clinetSalesInfo.Remark=="成功")
                    {
                        SaleModel saleInfo = new SaleModel();
                        saleInfo.SalesDate = DateTime.Now;
                        saleInfo.RealitySaleNumber = clinetSalesInfo.TotalNum;
                        saleInfo.TradeStatus = 8;
                        saleInfo.MachineId = clinetSalesInfo.MachineId;
                        saleInfo.GoodsId = clinetSalesInfo.TunnelId;

                        saleInfo.TradeNo = saleRelationModel.TradeNo;
                        saleInfo.WaresId = saleRelationModel.WaresId;

                        GenerateDal.Update(CommonSqlKey.UpdateSalesCashlessByPickup, saleInfo);
                        UpdateCurrStock(clinetSalesInfo.MachineId, clinetSalesInfo.TunnelId, clinetSalesInfo.TotalNum);
                        saleRelationModel.TotalNum = clinetSalesInfo.TotalNum;
                        saleRelationModel.CodeStatus = 2;
                        saleRelationModel.EndDate = DateTime.Now;
                        saleRelationModel.Remark = clinetSalesInfo.Remark;
                        GenerateDal.Update(CommonSqlKey.UpdatePickupCodeStatus, saleRelationModel);
                        RedisHelper redisHelper3 = new RedisHelper(3);
                        redisHelper3.KeyDelete(clinetSalesInfo.PickupNo);
                    }
                    else
                    {
                        saleRelationModel.Remark = clinetSalesInfo.Remark;
                        GenerateDal.Update(CommonSqlKey.UpdatePickupCodeStatus, saleRelationModel);
                    }
                   
                }
                GenerateDal.CommitTransaction();
               
            }
            catch (Exception e)
            {
                GenerateDal.RollBack();
                return 0;
            }

            return 1;
        }

        private void UpdateCurrStock(string machineId, string tunnelId, int saleNumber)
        {
            TunnelInfoModel tunnelInfo = new TunnelInfoModel();
            tunnelInfo.MachineId = machineId;
            tunnelInfo.GoodsStuId = tunnelId;
            tunnelInfo.CurrStock = saleNumber;
            GenerateDal.Execute(CommonSqlKey.UpdateCurrStock, tunnelInfo);
        }

        //分享给朋友，朋友领取分享的商品
        public int ExchangeFromFriend(ClientSalesRelationModel clientSalesInfo)
        {
            if(string.IsNullOrEmpty(clientSalesInfo.PickupNo))
            {
                return 0;
            }
            if (string.IsNullOrEmpty(clientSalesInfo.MemberId))
            {
                return 0;
            }
            clientSalesInfo.CodeStatus = 1;
            return GenerateDal.Update(CommonSqlKey.ExchangeFromFriend, clientSalesInfo);
        }

        //根据取货码和会员id取对应数据
        public List<ClientSalesRelationModel> GetClientSalesByPickNo(string otherOpendId, ClientSalesRelationModel clientSalesInfo)
        {
            var conditions = new List<Condition>();

            conditions.Add(new Condition
            {
                LeftBrace = " AND ",
                ParamName = "PickupCode",
                DbColumnName = "pickup_code",
                ParamValue = clientSalesInfo.PickupNo,
                Operation = ConditionOperate.Equal,
                RightBrace = "",
                Logic = ""
            });
            conditions.Add(new Condition
            {
                LeftBrace = " AND ",
                ParamName = "CodeStatus",
                DbColumnName = "code_status",
                ParamValue = 1,
                Operation = ConditionOperate.Equal,
                RightBrace = "",
                Logic = ""
            });
            conditions.Add(new Condition
            {
                LeftBrace = " AND ",
                ParamName = "MemberId",
                DbColumnName = "member_id",
                ParamValue = otherOpendId,
                Operation = ConditionOperate.Equal,
                RightBrace = "",
                Logic = ""
            });
            List<ClientSalesRelationModel> lstSaleInfo = GenerateDal.LoadByConditions<ClientSalesRelationModel>(CommonSqlKey.GetWaitingSalesList, conditions);
            return lstSaleInfo;
        }
    }
}
