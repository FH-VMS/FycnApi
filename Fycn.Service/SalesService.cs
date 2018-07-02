using Fycn.Interface;
using Fycn.Model.Pay;
using Fycn.Model.Refund;
using Fycn.Model.Sale;
using Fycn.SqlDataAccess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Fycn.Utility;
using Fycn.Model.Sys;

namespace Fycn.Service
{
    public class SalesService : AbstractService, ISale, IBase<SaleModel>
    {
        public int PostData(SaleModel saleInfo)
        {
            int result = 0;
            saleInfo.TradeStatus = 0;
            saleInfo.SalesDate = DateTime.Now;
            result = GenerateDal.Create(saleInfo);

            return result;
        }

        public int UpdateData(SaleModel saleInfo)
        {
            //saleInfo.PayDate = DateTime.Now;
            return GenerateDal.Update(CommonSqlKey.UpdateSale, saleInfo);

        }

        //返回支付结果
        public List<KeyTunnelModel> GetPayResult(string randomId, string tradeStatus, string machineId)
        {
            var conditions = new List<Condition>();

            if (!string.IsNullOrEmpty(randomId))
            {
                conditions.Add(new Condition
                {
                    LeftBrace = " AND ",
                    ParamName = "RandomId",
                    DbColumnName = "random_id",
                    ParamValue = randomId,
                    Operation = ConditionOperate.Equal,
                    RightBrace = "",
                    Logic = ""
                });
            }

            if (!string.IsNullOrEmpty(tradeStatus))
            {
                conditions.Add(new Condition
                {
                    LeftBrace = " AND ",
                    ParamName = "TradeStatus",
                    DbColumnName = "trade_status",
                    ParamValue = tradeStatus,
                    Operation = ConditionOperate.Equal,
                    RightBrace = "",
                    Logic = ""
                });
            }

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

            conditions.Add(new Condition
            {
                LeftBrace = "  ",
                ParamName = "PayDate",
                DbColumnName = "pay_date",
                ParamValue = "asc",
                Operation = ConditionOperate.OrderBy,
                RightBrace = "",
                Logic = ""
            });

            conditions.Add(new Condition
            {
                LeftBrace = "",
                ParamName = "Index",
                DbColumnName = "",
                ParamValue = 0,
                Operation = ConditionOperate.LimitIndex,
                RightBrace = "",
                Logic = ""

            });

            conditions.Add(new Condition
            {
                LeftBrace = "",
                ParamName = "Length",
                DbColumnName = "",
                ParamValue = 5,
                Operation = ConditionOperate.LimitLength,
                RightBrace = "",
                Logic = ""

            });


            return GenerateDal.LoadByConditions<KeyTunnelModel>(CommonSqlKey.GetPayResultById, conditions);
        }

        public List<SaleModel> GetAll(SaleModel saleInfo)
        {
            string userClientId = HttpContextHandler.GetHeaderObj("UserClientId").ToString();
            if (string.IsNullOrEmpty(userClientId))
            {
                return null;
            }
            var result = new List<SaleModel>();
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
                DbColumnName = "c.client_id",
                ParamValue = clientIds,
                Operation = ConditionOperate.INWithNoPara,
                RightBrace = "",
                Logic = ""
            });
            if (!string.IsNullOrEmpty(saleInfo.DeviceId))
            {
                conditions.Add(new Condition
                {
                    LeftBrace = " AND ",
                    ParamName = "DeviceId",
                    DbColumnName = "b.device_id",
                    ParamValue = "%" + saleInfo.DeviceId + "%",
                    Operation = ConditionOperate.Like,
                    RightBrace = "",
                    Logic = ""
                });
            }

            if (!string.IsNullOrEmpty(saleInfo.PayType))
            {
                conditions.Add(new Condition
                {
                    LeftBrace = " AND ",
                    ParamName = "PayType",
                    DbColumnName = "pay_type",
                    ParamValue = saleInfo.PayType,
                    Operation = ConditionOperate.Equal,
                    RightBrace = "",
                    Logic = ""
                });
            }

            if (saleInfo.TradeStatus!=0)
            {
                conditions.Add(new Condition
                {
                    LeftBrace = " AND ",
                    ParamName = "TradeStatus",
                    DbColumnName = "trade_status",
                    ParamValue = saleInfo.TradeStatus,
                    Operation = ConditionOperate.Equal,
                    RightBrace = "",
                    Logic = ""
                });
            }

            if (!string.IsNullOrEmpty(saleInfo.SaleDateStart))
            {
                conditions.Add(new Condition
                {
                    LeftBrace = " AND ",
                    ParamName = "SaleDateStart",
                    DbColumnName = "sales_date",
                    ParamValue = saleInfo.SaleDateStart,
                    Operation = ConditionOperate.GreaterThan,
                    RightBrace = "",
                    Logic = ""
                });
            }

            if (!string.IsNullOrEmpty(saleInfo.SaleDateEnd))
            {
                conditions.Add(new Condition
                {
                    LeftBrace = " AND ",
                    ParamName = "SaleDateEnd",
                    DbColumnName = "sales_date",
                    ParamValue = Convert.ToDateTime(saleInfo.SaleDateEnd).AddDays(1),
                    Operation = ConditionOperate.LessThan,
                    RightBrace = "",
                    Logic = ""
                });
            }

            if (!string.IsNullOrEmpty(saleInfo.TradeNo))
            {
                conditions.Add(new Condition
                {
                    LeftBrace = " AND ",
                    ParamName = "TradeNo",
                    DbColumnName = "trade_no",
                    ParamValue = "%" + saleInfo.TradeNo.Trim() + "%",
                    Operation = ConditionOperate.Like,
                    RightBrace = "",
                    Logic = ""
                });
            }

            

            
                conditions.Add(new Condition
                {
                    LeftBrace = "  ",
                    ParamName = "SalesDate",
                    DbColumnName = "sales_date",
                    ParamValue = "desc",
                    Operation = ConditionOperate.OrderBy,
                    RightBrace = "",
                    Logic = ""
                });
                conditions.AddRange(CreatePaginConditions(saleInfo.PageIndex, saleInfo.PageSize));
                result = GenerateDal.LoadByConditions<SaleModel>(CommonSqlKey.GetSaleAllList, conditions);





            return result;
        }


        public int GetCount(SaleModel saleInfo)
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
                DbColumnName = "c.client_id",
                ParamValue = clientIds,
                Operation = ConditionOperate.INWithNoPara,
                RightBrace = "",
                Logic = ""
            });
            if (!string.IsNullOrEmpty(saleInfo.DeviceId))
            {
                conditions.Add(new Condition
                {
                    LeftBrace = " AND ",
                    ParamName = "DeviceId",
                    DbColumnName = "b.device_id",
                    ParamValue = "%" + saleInfo.DeviceId + "%",
                    Operation = ConditionOperate.Like,
                    RightBrace = "",
                    Logic = ""
                });
            }

            if (!string.IsNullOrEmpty(saleInfo.PayType))
            {
                conditions.Add(new Condition
                {
                    LeftBrace = " AND ",
                    ParamName = "PayType",
                    DbColumnName = "pay_type",
                    ParamValue = saleInfo.PayType,
                    Operation = ConditionOperate.Equal,
                    RightBrace = "",
                    Logic = ""
                });
            }

            if (saleInfo.TradeStatus != 0)
            {
                conditions.Add(new Condition
                {
                    LeftBrace = " AND ",
                    ParamName = "TradeStatus",
                    DbColumnName = "trade_status",
                    ParamValue = saleInfo.TradeStatus,
                    Operation = ConditionOperate.Equal,
                    RightBrace = "",
                    Logic = ""
                });
            }

            if (!string.IsNullOrEmpty(saleInfo.SaleDateStart))
            {
                conditions.Add(new Condition
                {
                    LeftBrace = " AND ",
                    ParamName = "SaleDateStart",
                    DbColumnName = "sales_date",
                    ParamValue = saleInfo.SaleDateStart,
                    Operation = ConditionOperate.GreaterThan,
                    RightBrace = "",
                    Logic = ""
                });
            }

            if (!string.IsNullOrEmpty(saleInfo.SaleDateEnd))
            {
                conditions.Add(new Condition
                {
                    LeftBrace = " AND ",
                    ParamName = "SaleDateEnd",
                    DbColumnName = "sales_date",
                    ParamValue = Convert.ToDateTime(saleInfo.SaleDateEnd).AddDays(1),
                    Operation = ConditionOperate.LessThan,
                    RightBrace = "",
                    Logic = ""
                });
            }

            if (!string.IsNullOrEmpty(saleInfo.TradeNo))
            {
                conditions.Add(new Condition
                {
                    LeftBrace = " AND ",
                    ParamName = "TradeNo",
                    DbColumnName = "trade_no",
                    ParamValue = "%" + saleInfo.TradeNo.Trim() + "%",
                    Operation = ConditionOperate.Like,
                    RightBrace = "",
                    Logic = ""
                });
            }


           
                result = GenerateDal.CountByConditions(CommonSqlKey.GetSaleListAllCount, conditions);
           




            return result;
        }

     

        public int DeleteData(string id)
        {
            SaleModel salesInfo = new SaleModel();
            salesInfo.SalesIcId = id;
            return GenerateDal.Delete<SaleModel>(CommonSqlKey.DeleteSaleList, salesInfo);
        }

        //取退款详情
        public RefundModel GetRefundDetail(string orderNo, string typ)
        {
            var conditions = new List<Condition>();
            if (typ == "w")
            {
                conditions.Add(new Condition
                {
                    LeftBrace = " AND ",
                    ParamName = "OutTradeNo",
                    DbColumnName = "out_trade_no",
                    ParamValue = orderNo,
                    Operation = ConditionOperate.Equal,
                    RightBrace = "",
                    Logic = ""
                });
            }

            else if (typ == "a")
            {
                conditions.Add(new Condition
                {
                    LeftBrace = " AND ",
                    ParamName = "TradeNo",
                    DbColumnName = "trade_no",
                    ParamValue = orderNo,
                    Operation = ConditionOperate.Equal,
                    RightBrace = "",
                    Logic = ""
                });
            }
           



            try
            {
                return GenerateDal.LoadByConditions<RefundModel>(CommonSqlKey.GetRefundDetail, conditions)[0];
            }
            catch (Exception e)
            {
                return new RefundModel();
            }
           
        }

    }
}
