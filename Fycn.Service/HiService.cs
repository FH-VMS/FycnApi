using Fycn.Interface;
using Fycn.Model.Ad;
using Fycn.Model.Machine;
using Fycn.Model.Member;
using Fycn.Model.Pay;
using Fycn.Model.Privilege;
using Fycn.Model.Sale;
using Fycn.Model.Wechat;
using Fycn.SqlDataAccess;
using Fycn.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Fycn.Service
{
    public class HiService : AbstractService, IHi
    {
        public List<MachineLocationModel> GetMachineLocationById(string machineId)
        {
            var conditions = new List<Condition>();
          
            conditions.Add(new Condition
            {
                LeftBrace = " AND ",
                ParamName = "MachineId",
                DbColumnName = "a.machine_id",
                ParamValue = machineId,
                Operation = ConditionOperate.Equal,
                RightBrace = " ",
                Logic = ""
            });

            return GenerateDal.LoadByConditions<MachineLocationModel>(CommonSqlKey.GetMachineLocationById, conditions);
        }

        //微信支付结果插入数据库
        public int PostPayResultW(KeyJsonModel keyJsonModel, string tradeNo, string sellerId, string buyerId, string isConcern, string payDate)
        {
            int existResult = new MachineService().GetCountByTradeNo(tradeNo);
            if(existResult>0)
            {
                return 1;
            }
            try
            {
                GenerateDal.BeginTransaction();

                foreach (KeyTunnelModel keyTunnelInfo in keyJsonModel.t)
                {
                    SaleModel saleInfo = new SaleModel();
                    saleInfo.SalesIcId = Guid.NewGuid().ToString();
                    saleInfo.MachineId = keyJsonModel.m;
                    saleInfo.SalesDate = DateTime.Now;
                    saleInfo.SalesNumber = string.IsNullOrEmpty(keyTunnelInfo.n) ? 1 : Convert.ToInt32(keyTunnelInfo.n);
                    saleInfo.PayDate = new PayHelper().TransStrToDateTime(payDate, "w");
                    saleInfo.PayInterface = "微信";
                    saleInfo.PayType = "微信";
                    saleInfo.TradeNo = tradeNo;
                    saleInfo.GoodsId = keyTunnelInfo.tid;
                    saleInfo.TradeStatus = 9;
                    saleInfo.MerchantId = sellerId;
                    saleInfo.BuyerId = buyerId;
                    saleInfo.IsWeixinConcern = isConcern;
                    saleInfo.TradeAmount = Convert.ToDouble(keyTunnelInfo.p);
                    saleInfo.ServiceCharge = Math.Round(Convert.ToDouble(keyTunnelInfo.p) * ConfigHandler.WeixinRate, 2, MidpointRounding.AwayFromZero);
                    saleInfo.WaresId = keyTunnelInfo.wid;
                   
                    saleInfo.WaresName = GetProductNameByWaresId(keyTunnelInfo.wid) + "一元嗨(" + keyTunnelInfo.p + "倍)";
                    
                    GenerateDal.Create(saleInfo);
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

        //根据商品id取商品名称
        private string GetProductNameByWaresId(string waresId)
        {
            var conditions = new List<Condition>();

            if (!string.IsNullOrEmpty(waresId))
            {
                conditions.Add(new Condition
                {
                    LeftBrace = " AND ",
                    ParamName = "WaresId",
                    DbColumnName = "wares_id",
                    ParamValue = waresId,
                    Operation = ConditionOperate.Equal,
                    RightBrace = "",
                    Logic = ""
                });
            }
            DataTable result = GenerateDal.LoadDataTableByConditions(CommonSqlKey.GetProductNameByWaresId, conditions);
            if (result.Rows.Count > 0)
            {
                return result.Rows[0][0].ToString();
            }
            else
            {
                return "";
            }
        }

        public List<ActivityPrivilegeRelationModel> IsSupportActivity(string machineId)
        {

            var conditions = new List<Condition>();
            conditions.Add(new Condition
            {
                LeftBrace = " AND ",
                ParamName = "MachineId",
                DbColumnName = "c.machine_id",
                ParamValue = machineId,
                Operation = ConditionOperate.Equal,
                RightBrace = " ",
                Logic = ""
            });

            conditions.Add(new Condition
            {
                LeftBrace = " AND ",
                ParamName = "ActivityType",
                DbColumnName = "b.activity_type",
                ParamValue = 2,
                Operation = ConditionOperate.Equal,
                RightBrace = " ",
                Logic = ""
            });
            

            return GenerateDal.LoadByConditions<ActivityPrivilegeRelationModel>(CommonSqlKey.IsSupportActivity, conditions);
            
        }
        public int DoReward(KeyJsonModel keyJsonModel, string tradeNo,  string memberId, string waresName, bool isGoal)
        {
            try
            {

                string clientId = string.Empty;
                if (!string.IsNullOrEmpty(keyJsonModel.m))
                {
                    List<MachineListModel> lstMachine = GetMachineByMachineId(keyJsonModel.m);

                    if (lstMachine.Count > 0)
                    {
                        clientId = lstMachine[0].ClientId;
                    }
                }
                GenerateDal.BeginTransaction();
               
                if (isGoal) //一元嗨 嗨中
                {
                    SaleModel saleInfo = new SaleModel();
                    saleInfo.TradeStatus = 7;
                    saleInfo.TradeNo = tradeNo;
                    GenerateDal.Update(CommonSqlKey.ChangeTradeStatus, saleInfo);
                    ClientSalesRelationModel clientSalesInfo = new ClientSalesRelationModel();
                   
                    clientSalesInfo.ClientId = clientId;
                    clientSalesInfo.TradeNo = tradeNo;
                    clientSalesInfo.PickupNo = "取货卡";
                    clientSalesInfo.WaresId = keyJsonModel.t[0].wid;
                    clientSalesInfo.WaresName = waresName;
                    clientSalesInfo.CodeStatus = 1;
                    clientSalesInfo.TotalNum = 1;
                    clientSalesInfo.CreateDate = DateTime.Now;
                    clientSalesInfo.MemberId = memberId;
                    GenerateDal.Create(clientSalesInfo);
                } 
                else
                {
                    SaleModel saleInfo = new SaleModel();
                    saleInfo.TradeStatus = 10;
                    saleInfo.TradeNo = tradeNo;
                    MemberAccountModel memberAccount = new MemberAccountModel();
                    if(!string.IsNullOrEmpty(clientId))
                    {
                        int existResult = GetMemberAccountCount(memberId, clientId);
                        if (existResult == 0)
                        {
                            memberAccount.Id = Guid.NewGuid().ToString();
                            memberAccount.AccountData = 1;
                            memberAccount.ClientId = clientId;
                            memberAccount.MemberId = memberId;
                            memberAccount.TransferWithMoney = "100";
                            GenerateDal.Create(memberAccount);
                        }
                        else
                        {
                            memberAccount.ClientId = clientId;
                            memberAccount.MemberId = memberId;
                            memberAccount.AccountData = 1;
                            GenerateDal.Update(CommonSqlKey.AddMemberAccount, memberAccount);
                        }
                    }
                   
                    GenerateDal.Update(CommonSqlKey.ChangeTradeStatus, saleInfo);
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


        //取机器情况根据machine_id
        public List<MachineListModel> GetMachineByMachineId(string machineId)
        {
            var conditions = new List<Condition>();

            if (!string.IsNullOrEmpty(machineId))
            {
                conditions.Add(new Condition
                {
                    LeftBrace = " AND ",
                    ParamName = "MachineId",
                    DbColumnName = "machine_id",
                    ParamValue = machineId,
                    Operation = ConditionOperate.Equal,
                    RightBrace = "",
                    Logic = ""
                });
            }

            return GenerateDal.LoadByConditions<MachineListModel>(CommonSqlKey.GetCopyMachineById, conditions);
        }

        //根据单号取订单
        public List<SaleModel> GetTradeStatusByTradeNo(string tradeNo)
        {
            if(string.IsNullOrEmpty(tradeNo))
            {
                return null;
            }
            var conditions = new List<Condition>();

            
            conditions.Add(new Condition
            {
                LeftBrace = " AND ",
                ParamName = "TradeNo",
                DbColumnName = "trade_no",
                ParamValue = tradeNo,
                Operation = ConditionOperate.Equal,
                RightBrace = "",
                Logic = ""
            });
            

            return GenerateDal.LoadByConditions<SaleModel>(CommonSqlKey.GetSalesByNo, conditions);
        }

        //取广告资源
        public List<SourceToMachineModel> GetAdSource(string machineId,string adType)
        {
            if (string.IsNullOrEmpty(machineId))
            {
                return null;
            }
            var conditions = new List<Condition>();
            conditions.Add(new Condition
            {
                LeftBrace = " AND ",
                ParamName = "MachineId",
                DbColumnName = "c.machine_id",
                ParamValue = machineId,
                Operation = ConditionOperate.Equal,
                RightBrace = "",
                Logic = ""
            });

            if (!string.IsNullOrEmpty(adType))
            {
                conditions.Add(new Condition
                {
                    LeftBrace = " AND ",
                    ParamName = "AdType1",
                    DbColumnName = "a.ad_type",
                    ParamValue = adType,
                    Operation = ConditionOperate.Equal,
                    RightBrace = "",
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

            

            conditions.Add(new Condition
            {
                LeftBrace = "  ",
                ParamName = "Sequence",
                DbColumnName = "a.ad_type",
                ParamValue = "asc",
                Operation = ConditionOperate.OrderBy,
                RightBrace = "",
                Logic = ""
            });

            return GenerateDal.LoadByConditions<SourceToMachineModel>(CommonSqlKey.GetAdSource, conditions);
        }


        //取取货卡列表
        public List<ClientSalesRelationModel> GetWaitingPickupByMachine(string machineId, string openId)
        {
            if (string.IsNullOrEmpty(machineId)|| string.IsNullOrEmpty(openId))
            {
                return null;
            }
            var conditions = new List<Condition>();
            conditions.Add(new Condition
            {
                LeftBrace = " AND ",
                ParamName = "MachineId",
                DbColumnName = "b.machine_id",
                ParamValue = machineId,
                Operation = ConditionOperate.Equal,
                RightBrace = "",
                Logic = ""
            });

            conditions.Add(new Condition
            {
                LeftBrace = " AND ",
                ParamName = "MemberId",
                DbColumnName = "a.member_id",
                ParamValue = openId,
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

            conditions.Add(new Condition
            {
                LeftBrace = " AND ",
                ParamName = "TradeStatus",
                DbColumnName = "b.trade_status",
                ParamValue = 7,
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
            return GenerateDal.LoadByConditions<ClientSalesRelationModel>(CommonSqlKey.GetWaitingPickupByMachine, conditions);
        }

        //验证取货码
        public List<ClientSalesRelationModel> VerifyPickupByTradeNo(string tradeNo)
        {
            try
            {
                if (string.IsNullOrEmpty(tradeNo))
                {
                    return null;
                }
                
                var conditions = new List<Condition>();
                conditions.Add(new Condition
                {
                    LeftBrace = " AND ",
                    ParamName = "TradeNo",
                    DbColumnName = "a.trade_no",
                    ParamValue = tradeNo,
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

                return GenerateDal.LoadByConditions<ClientSalesRelationModel>(CommonSqlKey.VerifyPickupByTradeNo, conditions);
            }
            catch (Exception e)
            {
                return new List<ClientSalesRelationModel>();
            }


        }

        //根据商品id取商品信息
        public List<ProductModel> GetProducInfoByWaresId(string machineId, string waresId)
        {
            var conditions = new List<Condition>();

            
            conditions.Add(new Condition
            {
                LeftBrace = " AND ",
                ParamName = "MachineId",
                DbColumnName = "a.machine_id",
                ParamValue = machineId,
                Operation = ConditionOperate.Equal,
                RightBrace = "  ",
                Logic = ""
            });
            conditions.Add(new Condition
            {
                LeftBrace = " AND ",
                ParamName = "WaresId1",
                DbColumnName = "a.wares_id",
                ParamValue = waresId,
                Operation = ConditionOperate.Equal,
                RightBrace = "  ",
                Logic = ""
            });
            conditions.Add(new Condition
            {
                LeftBrace = "  ",
                ParamName = "",
                DbColumnName = "a.wares_id",
                ParamValue = "",
                Operation = ConditionOperate.GroupBy,
                RightBrace = "",
                Logic = ""
            });

            return GenerateDal.LoadByConditions<ProductModel>(CommonSqlKey.GetProductInfoByWaresId, conditions);


        }

        //是否存在会员账户
        private int GetMemberAccountCount(string memberId, string clientId)
        {
            var conditions = new List<Condition>();


            conditions.Add(new Condition
            {
                LeftBrace = " AND ",
                ParamName = "MachineId",
                DbColumnName = "member_id",
                ParamValue = memberId,
                Operation = ConditionOperate.Equal,
                RightBrace = "  ",
                Logic = ""
            });

            conditions.Add(new Condition
            {
                LeftBrace = " AND ",
                ParamName = "ClientId",
                DbColumnName = "client_id",
                ParamValue = clientId,
                Operation = ConditionOperate.Equal,
                RightBrace = "  ",
                Logic = ""
            });


            return GenerateDal.CountByConditions(CommonSqlKey.IsExistMemberAccount, conditions);
        }

        //根据会员id取对应客户的账户信息
        public MemberAccountModel GetMemberAccountByMember(string memberId, string clientId)
        {
            var conditions = new List<Condition>();


            conditions.Add(new Condition
            {
                LeftBrace = " AND ",
                ParamName = "MachineId",
                DbColumnName = "member_id",
                ParamValue = memberId,
                Operation = ConditionOperate.Equal,
                RightBrace = "  ",
                Logic = ""
            });

            conditions.Add(new Condition
            {
                LeftBrace = " AND ",
                ParamName = "ClientId",
                DbColumnName = "client_id",
                ParamValue = clientId,
                Operation = ConditionOperate.Equal,
                RightBrace = "  ",
                Logic = ""
            });


            List<MemberAccountModel> lstMemberAccount = GenerateDal.LoadByConditions<MemberAccountModel>(CommonSqlKey.GetMemberAccountByMember, conditions);
            if(lstMemberAccount.Count==0)
            {
                return new MemberAccountModel();
            }
            else
            {
                return lstMemberAccount[0];
            }
        }
        
    }
}
