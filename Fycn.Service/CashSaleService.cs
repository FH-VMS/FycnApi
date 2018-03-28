using Fycn.Interface;
using Fycn.Model.Common;
using Fycn.Model.Sale;
using Fycn.SqlDataAccess;
using Fycn.Utility;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fycn.Service
{
    public class CashSaleService : AbstractService, IBase<CashSaleModel>
    {
        public List<CashSaleModel> GetAll(CashSaleModel cashSaleInfo)
        {
            string userClientId = HttpContextHandler.GetHeaderObj("UserClientId").ToString();
            if (string.IsNullOrEmpty(userClientId))
            {
                return null;
            }
            var conditions = new List<Condition>();

            conditions.Add(new Condition
            {
                LeftBrace = "",
                ParamName = "ClientId",
                DbColumnName = "",
                ParamValue = userClientId,
                Operation = ConditionOperate.None,
                RightBrace = "",
                Logic = ""
            });

            if (!string.IsNullOrEmpty(cashSaleInfo.MachineId))
            {
                conditions.Add(new Condition
                {
                    LeftBrace = " AND ",
                    ParamName = "MachineId",
                    DbColumnName = "a.machine_id",
                    ParamValue = "%" + cashSaleInfo.MachineId + "%",
                    Operation = ConditionOperate.Like,
                    RightBrace = "",
                    Logic = ""
                });
            }
            

            if (!string.IsNullOrEmpty(cashSaleInfo.SaleDateStart))
            {
                conditions.Add(new Condition
                {
                    LeftBrace = " AND ",
                    ParamName = "SaleDateStart",
                    DbColumnName = "a.sales_date",
                    ParamValue = cashSaleInfo.SaleDateStart,
                    Operation = ConditionOperate.GreaterThan,
                    RightBrace = "",
                    Logic = ""
                });
            }

            if (!string.IsNullOrEmpty(cashSaleInfo.SaleDateEnd))
            {
                conditions.Add(new Condition
                {
                    LeftBrace = " AND ",
                    ParamName = "SaleDateEnd",
                    DbColumnName = "a.sales_date",
                    ParamValue = Convert.ToDateTime(cashSaleInfo.SaleDateEnd).AddDays(1),
                    Operation = ConditionOperate.LessThan,
                    RightBrace = "",
                    Logic = ""
                });
            }

            if (!string.IsNullOrEmpty(cashSaleInfo.TradeNo))
            {
                conditions.Add(new Condition
                {
                    LeftBrace = " AND ",
                    ParamName = "TradeNo",
                    DbColumnName = "a.trade_no",
                    ParamValue = "%" + cashSaleInfo.TradeNo.Trim() + "%",
                    Operation = ConditionOperate.Like,
                    RightBrace = "",
                    Logic = ""
                });
            }

            conditions.AddRange(CreatePaginConditions(cashSaleInfo.PageIndex, cashSaleInfo.PageSize));

            return GenerateDal.LoadByConditions<CashSaleModel>(CommonSqlKey.GetCashSaleList, conditions);
        }


        public int GetCount(CashSaleModel cashSaleInfo)
        {
            var result = 0;

            string userClientId = HttpContextHandler.GetHeaderObj("UserClientId").ToString();
            if (string.IsNullOrEmpty(userClientId))
            {
                return 0;
            }
            var conditions = new List<Condition>();
            conditions.Add(new Condition
            {
                LeftBrace = "",
                ParamName = "ClientId",
                DbColumnName = "",
                ParamValue = userClientId,
                Operation = ConditionOperate.None,
                RightBrace = "",
                Logic = ""
            });

            if (!string.IsNullOrEmpty(cashSaleInfo.MachineId))
            {
                conditions.Add(new Condition
                {
                    LeftBrace = " AND ",
                    ParamName = "MachineId",
                    DbColumnName = "a.machine_id",
                    ParamValue = "%" + cashSaleInfo.MachineId + "%",
                    Operation = ConditionOperate.Like,
                    RightBrace = "",
                    Logic = ""
                });
            }


            if (!string.IsNullOrEmpty(cashSaleInfo.SaleDateStart))
            {
                conditions.Add(new Condition
                {
                    LeftBrace = " AND ",
                    ParamName = "SaleDateStart",
                    DbColumnName = "a.sales_date",
                    ParamValue = cashSaleInfo.SaleDateStart,
                    Operation = ConditionOperate.GreaterThan,
                    RightBrace = "",
                    Logic = ""
                });
            }

            if (!string.IsNullOrEmpty(cashSaleInfo.SaleDateEnd))
            {
                conditions.Add(new Condition
                {
                    LeftBrace = " AND ",
                    ParamName = "SaleDateEnd",
                    DbColumnName = "a.sales_date",
                    ParamValue = Convert.ToDateTime(cashSaleInfo.SaleDateEnd).AddDays(1),
                    Operation = ConditionOperate.LessThan,
                    RightBrace = "",
                    Logic = ""
                });
            }

            if (!string.IsNullOrEmpty(cashSaleInfo.TradeNo))
            {
                conditions.Add(new Condition
                {
                    LeftBrace = " AND ",
                    ParamName = "TradeNo",
                    DbColumnName = "a.trade_no",
                    ParamValue = "%" + cashSaleInfo.TradeNo.Trim() + "%",
                    Operation = ConditionOperate.Like,
                    RightBrace = "",
                    Logic = ""
                });
            }



            result = GenerateDal.CountByConditions(CommonSqlKey.GetCashSaleCount, conditions);

            return result;
        }


        /// <summary>
        /// 新增/修改会员信息
        /// </summary>
        /// <param name="memberInfo"></param>
        /// <returns></returns>
        public int PostData(CashSaleModel cashSaleInfo)
        {
            cashSaleInfo.SalesNo = Guid.NewGuid().ToString();
            if (string.IsNullOrEmpty(cashSaleInfo.TradeNo))
            {
                cashSaleInfo.TradeNo = GeneraterTradeNo();
            }
            cashSaleInfo.SalesNumber = 1;
            cashSaleInfo.SalesDate = DateTime.Now;
            List<CommonDic> wares = GetWaresByTunnel(cashSaleInfo.MachineId, cashSaleInfo.GoodsId);
            if (wares.Count > 0)
            {
                cashSaleInfo.WaresId = wares[0].Id;
                cashSaleInfo.WaresName = wares[0].Name;
            }
            return GenerateDal.Create(cashSaleInfo);
        }

        private string GeneraterTradeNo()
        {
            Random ran = new Random();
            int RandKey = ran.Next(1000, 9999);
            string out_trade_no = DateTime.Now.ToString("yyyyMMddhhmmssffff") + RandKey.ToString();
            return out_trade_no;
        }

        private List<CommonDic> GetWaresByTunnel(string machineId,string tunnelId)
        {
            var conditions = new List<Condition>();
            conditions.Add(new Condition
            {
                LeftBrace = " AND ",
                ParamName = "MachineId",
                DbColumnName = "a.machine_id",
                ParamValue = machineId,
                Operation = ConditionOperate.Equal,
                RightBrace = "",
                Logic = ""
            });

            conditions.Add(new Condition
            {
                LeftBrace = " AND ",
                ParamName = "TunnelId",
                DbColumnName = "a.tunnel_id",
                ParamValue = tunnelId,
                Operation = ConditionOperate.Equal,
                RightBrace = "",
                Logic = ""
            });

            return GenerateDal.LoadByConditions<CommonDic>(CommonSqlKey.GetWaresByTunnel, conditions);

        }

        /// <summary>
        /// 删除用户
        /// </summary>
        /// <returns></returns>
        public int DeleteData(string id)
        {
            return 0;
        }

        public int UpdateData(CashSaleModel cashSaleInfo)
        {
            //操作日志
            return 0;
        }
    }
}
