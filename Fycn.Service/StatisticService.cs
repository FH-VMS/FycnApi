using Fycn.Interface;
using Fycn.Model.Sale;
using Fycn.SqlDataAccess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using Fycn.Utility;
using Fycn.Model.Sys;
using Fycn.Model.Statistic;

namespace Fycn.Service
{
    public class StatisticService : AbstractService, IStatistic
    {
        /// <summary>
        /// 取机器的销售额
        /// </summary>
        /// <returns></returns>
        public DataTable GetSalesAmountByMachine(string salesDateStart, string salesDateEnd, bool needPage, int pageIndex, int pageSize)
        {
            var clientId = HttpContextHandler.GetHeaderObj("UserClientId");
            var conditions = new List<Condition>();
            if (string.IsNullOrEmpty(clientId.ToString()))
            {
                return new DataTable();
            }
            conditions.Add(new Condition
            {
                LeftBrace = "",
                ParamName = "ClientId",
                DbColumnName = "",
                ParamValue = clientId,
                Operation = ConditionOperate.None,
                RightBrace = "",
                Logic = ""
            });

            if (!string.IsNullOrEmpty(salesDateStart))
            {
                conditions.Add(new Condition
                {
                    LeftBrace = " AND ",
                    ParamName = "SaleDateStart",
                    DbColumnName = "sales_date",
                    ParamValue = salesDateStart,
                    Operation = ConditionOperate.GreaterThan,
                    RightBrace = "",
                    Logic = ""
                });
            }

            if (!string.IsNullOrEmpty(salesDateEnd))
            {
                conditions.Add(new Condition
                {
                    LeftBrace = " AND ",
                    ParamName = "SaleDateEnd",
                    DbColumnName = "sales_date",
                    ParamValue =  Convert.ToDateTime(salesDateEnd).AddDays(1),
                    Operation = ConditionOperate.LessThan,
                    RightBrace = "",
                    Logic = ""
                });
            }

            conditions.Add(new Condition
            {
                LeftBrace = "",
                ParamName = "",
                DbColumnName = "",
                ParamValue = "a.machine_id",
                Operation = ConditionOperate.GroupBy,
                RightBrace = "",
                Logic = ""
            });

            if (needPage)
            {
                conditions.AddRange(CreatePaginConditions(pageIndex, pageSize));
            }
            
            
            return GenerateDal.LoadDataTableByConditions(CommonSqlKey.GetSalesAmountByMachine, conditions);
        }

        public int GetSalesAmountByMachineCount(string salesDateStart, string salesDateEnd, bool needPage, int pageIndex, int pageSize)
        {
            var result = 0;

            var clientId = HttpContextHandler.GetHeaderObj("UserClientId");
            var conditions = new List<Condition>();
            conditions.Add(new Condition
            {
                LeftBrace = "",
                ParamName = "ClientId",
                DbColumnName = "",
                ParamValue = clientId,
                Operation = ConditionOperate.None,
                RightBrace = "",
                Logic = ""
            });

            if (!string.IsNullOrEmpty(salesDateStart))
            {
                conditions.Add(new Condition
                {
                    LeftBrace = " AND ",
                    ParamName = "SaleDateStart",
                    DbColumnName = "sales_date",
                    ParamValue = salesDateStart,
                    Operation = ConditionOperate.GreaterThan,
                    RightBrace = "",
                    Logic = ""
                });
            }

            if (!string.IsNullOrEmpty(salesDateEnd))
            {
                conditions.Add(new Condition
                {
                    LeftBrace = " AND ",
                    ParamName = "SaleDateEnd",
                    DbColumnName = "sales_date",
                    ParamValue = Convert.ToDateTime(salesDateEnd).AddDays(1),
                    Operation = ConditionOperate.LessThan,
                    RightBrace = "",
                    Logic = ""
                });
            }



            result = GenerateDal.CountByConditions(CommonSqlKey.GetSalesAmountByMachineCount, conditions);

            return result;
        }

        /// <summary>
        /// 统计金额
        /// </summary>
        /// <param name="saleInfo"></param>
        /// <returns></returns>
        public DataTable GetStatisticSalesMoneyByDate(SaleModel saleInfo)
        {
            string userClientId = HttpContextHandler.GetHeaderObj("UserClientId").ToString();
            string userStatus = HttpContextHandler.GetHeaderObj("Sts").ToString();
            DataTable result = new DataTable();
            var conditions = new List<Condition>();

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
                    ParamValue = saleInfo.SaleDateEnd,
                    Operation = ConditionOperate.LessThan,
                    RightBrace = "",
                    Logic = ""
                });
            }

         

          

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

            conditions.Add(new Condition
            {
                LeftBrace = "  ",
                ParamName = "",
                DbColumnName = "a.pay_interface",
                ParamValue = "",
                Operation = ConditionOperate.GroupBy,
                RightBrace = "",
                Logic = ""
            });

            result = GenerateDal.LoadDataTableByConditions(CommonSqlKey.GetStatisticSalesMoneyByDate, conditions);

            return result;
        }

        /// <summary>
        /// 取支付笔数
        /// </summary>
        /// <returns></returns>
        public List<ClassModel> GetPayNumbers()
        {
            string userClientId = HttpContextHandler.GetHeaderObj("UserClientId").ToString();
            var result = new List<ClassModel>();
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
            result = GenerateDal.LoadByConditions<ClassModel>(CommonSqlKey.GetPayNumbers, conditions);
            return result;
        }

        /// <summary>
        /// 取销售额
        /// </summary>
        /// <returns></returns>
        public List<ClassModel> GetGroupSalesMoney(string salesDateStart, string salesDateEnd, string type)
        {
            var clientId = HttpContextHandler.GetHeaderObj("UserClientId");
            var conditions = new List<Condition>();
          
            conditions.Add(new Condition
            {
                LeftBrace = "",
                ParamName = "ClientId",
                DbColumnName = "",
                ParamValue = clientId,
                Operation = ConditionOperate.None,
                RightBrace = "",
                Logic = ""
            });

            conditions.Add(new Condition
            {
                LeftBrace = " AND ",
                ParamName = "TradeStatus",
                DbColumnName = "a.trade_status",
                ParamValue = 2,
                Operation = ConditionOperate.Equal,
                RightBrace = "",
                Logic = ""
            });

            if (!string.IsNullOrEmpty(salesDateStart))
            {
                conditions.Add(new Condition
                {
                    LeftBrace = " AND ",
                    ParamName = "SaleDateStart",
                    DbColumnName = "a.sales_date",
                    ParamValue = salesDateStart,
                    Operation = ConditionOperate.GreaterThan,
                    RightBrace = "",
                    Logic = ""
                });
            }

            if (!string.IsNullOrEmpty(salesDateEnd))
            {
                conditions.Add(new Condition
                {
                    LeftBrace = " AND ",
                    ParamName = "SaleDateEnd",
                    DbColumnName = "a.sales_date",
                    ParamValue = Convert.ToDateTime(salesDateEnd).AddDays(1),
                    Operation = ConditionOperate.LessThan,
                    RightBrace = "",
                    Logic = ""
                });
            }
            if (type == "year")
            {
                conditions.Add(new Condition
                {
                    LeftBrace = "",
                    ParamName = "",
                    DbColumnName = "",
                    ParamValue = "year(a.sales_date)",
                    Operation = ConditionOperate.GroupBy,
                    RightBrace = "",
                    Logic = ""
                });
            }
            else if(type=="month")
            {
                conditions.Add(new Condition
                {
                    LeftBrace = "",
                    ParamName = "",
                    DbColumnName = "",
                    ParamValue = "year(a.sales_date),month(a.sales_date)",
                    Operation = ConditionOperate.GroupBy,
                    RightBrace = "",
                    Logic = ""
                });
            }
            else if (type == "day")
            {
                conditions.Add(new Condition
                {
                    LeftBrace = "",
                    ParamName = "",
                    DbColumnName = "",
                    ParamValue = "year(a.sales_date),month(a.sales_date),day(sales_date)",
                    Operation = ConditionOperate.GroupBy,
                    RightBrace = "",
                    Logic = ""
                });
            }
            else if(type == "week")
            {
                conditions.Add(new Condition
                {
                    LeftBrace = "",
                    ParamName = "",
                    DbColumnName = "",
                    ParamValue = "week(a.sales_date)",
                    Operation = ConditionOperate.GroupBy,
                    RightBrace = "",
                    Logic = ""
                });
            }


            return GenerateDal.LoadByConditions<ClassModel>(CommonSqlKey.GetGroupSalesMoney, conditions);
        }
    }
}
