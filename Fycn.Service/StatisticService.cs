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
        public DataTable GetSalesAmountByMachine(string salesDateStart, string salesDateEnd, string machineId, bool needPage, int pageIndex, int pageSize)
        {
            var clientId = HttpContextHandler.GetHeaderObj("UserClientId").ToString();
            if (string.IsNullOrEmpty(clientId))
            {
                return null;
            }
            var conditions = new List<Condition>();
            if (string.IsNullOrEmpty(clientId.ToString()))
            {
                return new DataTable();
            }
            string clientIds = new CommonService().GetClientIds(clientId);
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
                RightBrace = "",
                Logic = ""
            });

            if (!string.IsNullOrEmpty(salesDateStart))
            {
                conditions.Add(new Condition
                {
                    LeftBrace = " AND ",
                    ParamName = "SaleDateStart",
                    DbColumnName = "a.pay_date",
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
                    DbColumnName = "a.pay_date",
                    ParamValue =  Convert.ToDateTime(salesDateEnd).AddDays(1),
                    Operation = ConditionOperate.LessThan,
                    RightBrace = "",
                    Logic = ""
                });
            }

            if (!string.IsNullOrEmpty(machineId))
            {
                conditions.Add(new Condition
                {
                    LeftBrace = " AND (",
                    ParamName = "MachineId",
                    DbColumnName = "a.machine_id",
                    ParamValue = "%"+machineId+"%",
                    Operation = ConditionOperate.Like,
                    RightBrace = "",
                    Logic = ""
                });

                conditions.Add(new Condition
                {
                    LeftBrace = " OR ",
                    ParamName = "Remark",
                    DbColumnName = "b.remark",
                    ParamValue = "%" + machineId + "%",
                    Operation = ConditionOperate.Like,
                    RightBrace = ")",
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

            conditions.Add(new Condition
            {
                LeftBrace = "  ",
                ParamName = "Total",
                DbColumnName = "total",
                ParamValue = "desc",
                Operation = ConditionOperate.OrderBy,
                RightBrace = "",
                Logic = ""
            });

            if (needPage)
            {
                conditions.AddRange(CreatePaginConditions(pageIndex, pageSize));
            }
            
            
            return GenerateDal.LoadDataTableByConditions(CommonSqlKey.GetSalesAmountByMachine, conditions);
        }

        public int GetSalesAmountByMachineCount(string salesDateStart, string salesDateEnd, string machineId, bool needPage, int pageIndex, int pageSize)
        {
            var result = 0;

            var clientId = HttpContextHandler.GetHeaderObj("UserClientId").ToString();
            if (string.IsNullOrEmpty(clientId))
            {
                return 0;
            }
            var conditions = new List<Condition>();
            string clientIds = new CommonService().GetClientIds(clientId);
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
                RightBrace = "",
                Logic = ""
            });

            if (!string.IsNullOrEmpty(salesDateStart))
            {
                conditions.Add(new Condition
                {
                    LeftBrace = " AND ",
                    ParamName = "SaleDateStart",
                    DbColumnName = "a.pay_date",
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
                    DbColumnName = "a.pay_date",
                    ParamValue = Convert.ToDateTime(salesDateEnd).AddDays(1),
                    Operation = ConditionOperate.LessThan,
                    RightBrace = "",
                    Logic = ""
                });
            }
            if (!string.IsNullOrEmpty(machineId))
            {
                conditions.Add(new Condition
                {
                    LeftBrace = " AND (",
                    ParamName = "MachineId",
                    DbColumnName = "a.machine_id",
                    ParamValue = "%" + machineId + "%",
                    Operation = ConditionOperate.Like,
                    RightBrace = "",
                    Logic = ""
                });

                conditions.Add(new Condition
                {
                    LeftBrace = " OR ",
                    ParamName = "Remark",
                    DbColumnName = "b.remark",
                    ParamValue = "%" + machineId + "%",
                    Operation = ConditionOperate.Like,
                    RightBrace = ")",
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
            if (string.IsNullOrEmpty(userClientId))
            {
                return null;
            }
          
            DataTable result = new DataTable();
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
                DbColumnName = "b.client_id",
                ParamValue = clientIds,
                Operation = ConditionOperate.INWithNoPara,
                RightBrace = "",
                Logic = ""
            });
            if (!string.IsNullOrEmpty(saleInfo.SaleDateStart))
            {
                conditions.Add(new Condition
                {
                    LeftBrace = " AND ",
                    ParamName = "SaleDateStart",
                    DbColumnName = "a.pay_date",
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
                    DbColumnName = "a.pay_date",
                    ParamValue = saleInfo.SaleDateEnd,
                    Operation = ConditionOperate.LessThan,
                    RightBrace = "",
                    Logic = ""
                });
            }

         

          

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
        public List<ClassModel> GetPayNumbers(string clientId)
        {
            string userClientId = clientId;
            if(string.IsNullOrEmpty(userClientId))
            {
                userClientId=HttpContextHandler.GetHeaderObj("UserClientId").ToString();
            }
            
            if (string.IsNullOrEmpty(userClientId))
            {
                return null;
            }
            var result = new List<ClassModel>();
            var conditions = new List<Condition>();
            string clientIds = new CommonService().GetClientIds(userClientId);
            if(clientIds.Contains("self")){
                clientIds = "'" + clientIds.Replace(",", "','") + "'";
            }
                conditions.Add(new Condition
                {
                    LeftBrace = " AND (",
                    ParamName = "ClientIdA",
                    DbColumnName = "b.client_id",
                    ParamValue = userClientId,
                    Operation = ConditionOperate.Equal,
                    RightBrace = "",
                    Logic = ""
                });

                conditions.Add(new Condition
                {
                    LeftBrace = " OR ",
                    ParamName = "ClientIdB",
                    DbColumnName = "b.client_id",
                    ParamValue = clientIds,
                    Operation = ConditionOperate.INWithNoPara,
                    RightBrace = " ) ",
                    Logic = ""
                });
            conditions.Add(new Condition
                {
                    LeftBrace = " ",
                    ParamName = "",
                    DbColumnName = "",
                    ParamValue = "a.trade_status",
                    Operation = ConditionOperate.GroupBy,
                    RightBrace = "",
                    Logic = ""
                });
            result = GenerateDal.LoadByConditions<ClassModel>(CommonSqlKey.GetPayNumbers, conditions);
            return result;
        }

        /// <summary>
        /// 根据日期取支付笔数
        /// </summary>
        /// <returns></returns>
        public List<ClassModel> GetPayNumbersByDate(string salesDateStart, string salesDateEnd, string type, string clientId)
        {
            string userClientId = clientId;
            if (string.IsNullOrEmpty(userClientId))
            {
                userClientId = HttpContextHandler.GetHeaderObj("UserClientId").ToString();
            }
            if (string.IsNullOrEmpty(userClientId))
            {
                return null;
            }
            var result = new List<ClassModel>();
            var conditions = new List<Condition>();
            string clientIds = new CommonService().GetClientIds(userClientId);
            if (clientIds.Contains("self"))
            {
                clientIds = "'" + clientIds.Replace(",", "','") + "'";
            }
            conditions.Add(new Condition
            {
                LeftBrace = " AND (",
                ParamName = "ClientIdA",
                DbColumnName = "b.client_id",
                ParamValue = userClientId,
                Operation = ConditionOperate.Equal,
                RightBrace = "",
                Logic = ""
            });

            conditions.Add(new Condition
            {
                LeftBrace = " OR ",
                ParamName = "ClientIdB",
                DbColumnName = "b.client_id",
                ParamValue = clientIds,
                Operation = ConditionOperate.INWithNoPara,
                RightBrace = " ) ",
                Logic = ""
            });
            if (!string.IsNullOrEmpty(salesDateStart))
            {
                conditions.Add(new Condition
                {
                    LeftBrace = " AND ",
                    ParamName = "SaleDateStart",
                    DbColumnName = "a.pay_date",
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
                    DbColumnName = "a.pay_date",
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
                    ParamValue = "year(a.pay_date)",
                    Operation = ConditionOperate.GroupBy,
                    RightBrace = "",
                    Logic = ""
                });
            }
            else if (type == "month")
            {
                conditions.Add(new Condition
                {
                    LeftBrace = "",
                    ParamName = "",
                    DbColumnName = "",
                    ParamValue = "year(a.pay_date),month(a.pay_date)",
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
                    ParamValue = "year(a.pay_date),month(a.pay_date),day(pay_date)",
                    Operation = ConditionOperate.GroupBy,
                    RightBrace = "",
                    Logic = ""
                });
            }
            result = GenerateDal.LoadByConditions<ClassModel>(CommonSqlKey.GetPayNumbers, conditions);
            return result;
        }

        /// <summary>
        /// 取销售额
        /// </summary>
        /// <returns></returns>
        public List<ClassModel> GetGroupSalesMoney(string salesDateStart, string salesDateEnd, string type,string clientId)
        {
            //var clientId = HttpContextHandler.GetHeaderObj("UserClientId").ToString();
            string userClientId = clientId;
            if (string.IsNullOrEmpty(userClientId))
            {
                userClientId = HttpContextHandler.GetHeaderObj("UserClientId").ToString();
            }
            if (string.IsNullOrEmpty(userClientId))
            {
                return null;
            }
            var conditions = new List<Condition>();
          
            string clientIds = new CommonService().GetClientIds(userClientId.ToString());
            if(clientIds.Contains("self")){
                clientIds = "'" + clientIds.Replace(",", "','") + "'";
            }
                conditions.Add(new Condition
                {
                    LeftBrace = " AND (",
                    ParamName = "ClientIdA",
                    DbColumnName = "b.client_id",
                    ParamValue = userClientId,
                    Operation = ConditionOperate.Equal,
                    RightBrace = "",
                    Logic = ""
                });

                conditions.Add(new Condition
                {
                    LeftBrace = " OR ",
                    ParamName = "ClientIdB",
                    DbColumnName = "b.client_id",
                    ParamValue = clientIds,
                    Operation = ConditionOperate.INWithNoPara,
                    RightBrace = " ) ",
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
                    DbColumnName = "a.pay_date",
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
                    DbColumnName = "a.pay_date",
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
                    ParamValue = "year(a.pay_date)",
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
                    ParamValue = "year(a.pay_date),month(a.pay_date)",
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
                    ParamValue = "year(a.pay_date),month(a.pay_date),day(pay_date)",
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
                    ParamValue = "week(a.pay_date)",
                    Operation = ConditionOperate.GroupBy,
                    RightBrace = "",
                    Logic = ""
                });
            }


            return GenerateDal.LoadByConditions<ClassModel>(CommonSqlKey.GetGroupSalesMoney, conditions);
        }


        /// <summary>
        /// 根据时间取商品销售数量
        /// </summary>
        /// <returns></returns>
        public List<ClassModel> GetGroupProduct(string salesDateStart, string salesDateEnd, string clientId, bool needPage = false, int pageIndex = 1, int pageSize = 10)
        {
            string userClientId = clientId;
            if (string.IsNullOrEmpty(userClientId))
            {
                userClientId = HttpContextHandler.GetHeaderObj("UserClientId").ToString();
            }
            if (string.IsNullOrEmpty(userClientId))
            {
                return null;
            }
            var conditions = new List<Condition>();

             string clientIds = new CommonService().GetClientIds(userClientId.ToString());
            if(clientIds.Contains("self")){
                clientIds = "'" + clientIds.Replace(",", "','") + "'";
            }
                conditions.Add(new Condition
                {
                    LeftBrace = " AND (",
                    ParamName = "ClientIdA",
                    DbColumnName = "b.client_id",
                    ParamValue = userClientId,
                    Operation = ConditionOperate.Equal,
                    RightBrace = "",
                    Logic = ""
                });

                conditions.Add(new Condition
                {
                    LeftBrace = " OR ",
                    ParamName = "ClientIdB",
                    DbColumnName = "b.client_id",
                    ParamValue = clientIds,
                    Operation = ConditionOperate.INWithNoPara,
                    RightBrace = " ) ",
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
                    DbColumnName = "a.pay_date",
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
                    DbColumnName = "a.pay_date",
                    ParamValue = Convert.ToDateTime(salesDateEnd).AddDays(1),
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
                ParamValue = "a.wares_name",
                Operation = ConditionOperate.GroupBy,
                RightBrace = "",
                Logic = ""
            });

            if (needPage)
            {
                conditions.AddRange(CreatePaginConditions(pageIndex, pageSize));
            }

            conditions.Add(new Condition
            {
                LeftBrace = "",
                ParamName = "Count",
                DbColumnName = "count(1)",
                ParamValue = "desc",
                Operation = ConditionOperate.OrderBy,
                RightBrace = "",
                Logic = ""
            });

            return GenerateDal.LoadByConditions<ClassModel>(CommonSqlKey.GetGroupProduct, conditions);
        }

        /// <summary>
        /// 取销售额根据机器进行分类
        /// </summary>
        /// <returns></returns>
        public List<ClassModel> GetGroupMoneyByMachine(string salesDateStart, string salesDateEnd, string clientId, bool needPage=true, int pageIndex=1, int pageSize=10)
        {
            string userClientId = clientId;
            if (string.IsNullOrEmpty(userClientId))
            {
                userClientId = HttpContextHandler.GetHeaderObj("UserClientId").ToString();
            }
           
            if (string.IsNullOrEmpty(userClientId))
            {
                return null;
            }
            var conditions = new List<Condition>();

            string clientIds = new CommonService().GetClientIds(userClientId.ToString());
            if (clientIds.Contains("self"))
            {
                clientIds = "'" + clientIds.Replace(",", "','") + "'";
            }
            conditions.Add(new Condition
            {
                LeftBrace = " AND (",
                ParamName = "ClientIdA",
                DbColumnName = "b.client_id",
                ParamValue = userClientId,
                Operation = ConditionOperate.Equal,
                RightBrace = "",
                Logic = ""
            });

            conditions.Add(new Condition
            {
                LeftBrace = " OR ",
                ParamName = "ClientIdB",
                DbColumnName = "b.client_id",
                ParamValue = clientIds,
                Operation = ConditionOperate.INWithNoPara,
                RightBrace = " ) ",
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
                    DbColumnName = "a.pay_date",
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
                    DbColumnName = "a.pay_date",
                    ParamValue = Convert.ToDateTime(salesDateEnd).AddDays(1),
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

            conditions.Add(new Condition
            {
                LeftBrace = "  ",
                ParamName = "Data",
                DbColumnName = "SUM(a.trade_amount*a.reality_sale_number)",
                ParamValue = "DESC",
                Operation = ConditionOperate.OrderBy,
                RightBrace = "",
                Logic = ""
            });

            if (needPage)
            {
                conditions.AddRange(CreatePaginConditions(pageIndex, pageSize));
            }

            return GenerateDal.LoadByConditions<ClassModel>(CommonSqlKey.GetGroupMoneyByMachine, conditions);
        }


        // 移动支付统计
        public DataTable GetMobilePayStatistic(string salesDateStart, string salesDateEnd, string clientId, string machineId,string tradeStatus)
        {
            string userClientId = clientId;
            if (string.IsNullOrEmpty(userClientId))
            {
                userClientId = HttpContextHandler.GetHeaderObj("UserClientId").ToString();
            }

            if (string.IsNullOrEmpty(userClientId))
            {
                return null;
            }
            var conditions = new List<Condition>();

            string clientIds = new CommonService().GetClientIds(userClientId.ToString());
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

            string[] statusArr = tradeStatus.Split('^');
            if (statusArr.Length == 1)
            {
                conditions.Add(new Condition
                {
                    LeftBrace = " AND ",
                    ParamName = "TradeStatus",
                    DbColumnName = "a.trade_status",
                    ParamValue = statusArr[0],
                    Operation = ConditionOperate.Equal,
                    RightBrace = "",
                    Logic = ""
                });
            }
            else
            {
                for (int i = 0; i < statusArr.Length; i++)
                {
                    if (i == 0 && !string.IsNullOrEmpty(statusArr[i]))
                    {
                        conditions.Add(new Condition
                        {
                            LeftBrace = " AND (",
                            ParamName = "TradeStatus" + i,
                            DbColumnName = "a.trade_status",
                            ParamValue = statusArr[i],
                            Operation = ConditionOperate.Equal,
                            RightBrace = "",
                            Logic = ""
                        });
                    }
                    else if (i == statusArr.Length - 1 && !string.IsNullOrEmpty(statusArr[i]))
                    {
                        conditions.Add(new Condition
                        {
                            LeftBrace = " OR ",
                            ParamName = "TradeStatus" + i,
                            DbColumnName = "a.trade_status",
                            ParamValue = statusArr[i],
                            Operation = ConditionOperate.Equal,
                            RightBrace = ")",
                            Logic = ""
                        });
                    }
                    else
                    {
                        conditions.Add(new Condition
                        {
                            LeftBrace = " OR ",
                            ParamName = "TradeStatus" + i,
                            DbColumnName = "a.trade_status",
                            ParamValue = statusArr[i],
                            Operation = ConditionOperate.Equal,
                            RightBrace = "",
                            Logic = ""
                        });
                    }
                }
            }

            if (!string.IsNullOrEmpty(salesDateStart))
            {
                conditions.Add(new Condition
                {
                    LeftBrace = " AND ",
                    ParamName = "SaleDateStart",
                    DbColumnName = "a.pay_date",
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
                    DbColumnName = "a.pay_date",
                    ParamValue = Convert.ToDateTime(salesDateEnd).AddDays(1),
                    Operation = ConditionOperate.LessThan,
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
                    DbColumnName = "a.machine_id",
                    ParamValue = machineId.Trim(),
                    Operation = ConditionOperate.Equal,
                    RightBrace = "",
                    Logic = ""
                });
            }
            conditions.Add(new Condition
            {
                LeftBrace = "",
                ParamName = "",
                DbColumnName = "",
                ParamValue = "a.pay_interface",
                Operation = ConditionOperate.GroupBy,
                RightBrace = "",
                Logic = ""
            });

           

            return GenerateDal.LoadDataTableByConditions(CommonSqlKey.GetMobilePayStatistic, conditions);
        }

        /// <summary>
        /// 现金统计
        /// </summary>
        /// <param name="salesDateStart"></param>
        /// <param name="salesDateEnd"></param>
        /// <param name="clientId"></param>
        /// <param name="machineId"></param>
        /// <param name="tradeStatus"></param>
        /// <returns></returns>
        public DataTable GetCashPayStatistic(string salesDateStart, string salesDateEnd, string clientId, string machineId, string tradeStatus)
        {
            string userClientId = clientId;
            if (string.IsNullOrEmpty(userClientId))
            {
                userClientId = HttpContextHandler.GetHeaderObj("UserClientId").ToString();
            }

            if (string.IsNullOrEmpty(userClientId))
            {
                return null;
            }
            var conditions = new List<Condition>();

            string clientIds = new CommonService().GetClientIds(userClientId.ToString());
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

            if (!string.IsNullOrEmpty(machineId))
            {
                conditions.Add(new Condition
                {
                    LeftBrace = " AND ",
                    ParamName = "MachineId",
                    DbColumnName = "a.machine_id",
                    ParamValue = machineId.Trim(),
                    Operation = ConditionOperate.Equal,
                    RightBrace = "",
                    Logic = ""
                });
            }
            

            return GenerateDal.LoadDataTableByConditions(CommonSqlKey.GetCashPayStatistic, conditions);
        }

        public DataTable GetProductStatistic(string salesDateStart, string salesDateEnd,string productName, string clientId, string machineId, string tradeStatus,int pageIndex,int pageSize)
        {
            string userClientId = clientId;
            if (string.IsNullOrEmpty(userClientId))
            {
                userClientId = HttpContextHandler.GetHeaderObj("UserClientId").ToString();
            }

            if (string.IsNullOrEmpty(userClientId))
            {
                return null;
            }
            var conditions = new List<Condition>();

            string clientIds = new CommonService().GetClientIds(userClientId.ToString());
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
            string[] statusArr = tradeStatus.Split('^');
            if(statusArr.Length==1)
            {
                conditions.Add(new Condition
                {
                    LeftBrace = " AND ",
                    ParamName = "TradeStatus",
                    DbColumnName = "a.trade_status",
                    ParamValue = statusArr[0],
                    Operation = ConditionOperate.Equal,
                    RightBrace = "",
                    Logic = ""
                });
            } 
            else
            {
                for (int i = 0; i < statusArr.Length; i++)
                {
                    if (i == 0 && !string.IsNullOrEmpty(statusArr[i]))
                    {
                        conditions.Add(new Condition
                        {
                            LeftBrace = " AND (",
                            ParamName = "TradeStatus"+i,
                            DbColumnName = "a.trade_status",
                            ParamValue = statusArr[i],
                            Operation = ConditionOperate.Equal,
                            RightBrace = "",
                            Logic = ""
                        });
                    }
                    else if(i== statusArr.Length - 1 && !string.IsNullOrEmpty(statusArr[i]))
                    {
                        conditions.Add(new Condition
                        {
                            LeftBrace = " OR ",
                            ParamName = "TradeStatus" + i,
                            DbColumnName = "a.trade_status",
                            ParamValue = statusArr[i],
                            Operation = ConditionOperate.Equal,
                            RightBrace = ")",
                            Logic = ""
                        });
                    } 
                    else
                    {
                        conditions.Add(new Condition
                        {
                            LeftBrace = " OR ",
                            ParamName = "TradeStatus" + i,
                            DbColumnName = "a.trade_status",
                            ParamValue = statusArr[i],
                            Operation = ConditionOperate.Equal,
                            RightBrace = "",
                            Logic = ""
                        });
                    }
                }
            }
           
            

            if (!string.IsNullOrEmpty(salesDateStart))
            {
                conditions.Add(new Condition
                {
                    LeftBrace = " AND ",
                    ParamName = "SaleDateStart",
                    DbColumnName = "a.pay_date",
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
                    DbColumnName = "a.pay_date",
                    ParamValue = Convert.ToDateTime(salesDateEnd).AddDays(1),
                    Operation = ConditionOperate.LessThan,
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
                    DbColumnName = "a.machine_id",
                    ParamValue = machineId.Trim(),
                    Operation = ConditionOperate.Equal,
                    RightBrace = "",
                    Logic = ""
                });
            }

            if (!string.IsNullOrEmpty(productName))
            {
                conditions.Add(new Condition
                {
                    LeftBrace = " AND ",
                    ParamName = "ProductName",
                    DbColumnName = "a.wares_name",
                    ParamValue = "%"+productName.Trim()+"%",
                    Operation = ConditionOperate.Like,
                    RightBrace = "",
                    Logic = ""
                });
            }
            conditions.Add(new Condition
            {
                LeftBrace = "",
                ParamName = "",
                DbColumnName = "",
                ParamValue = "a.wares_name",
                Operation = ConditionOperate.GroupBy,
                RightBrace = "",
                Logic = ""
            });


            conditions.AddRange(CreatePaginConditions(pageIndex, pageSize));
            return GenerateDal.LoadDataTableByConditions(CommonSqlKey.GetProductStatistic, conditions);
        }

        public int GetProductStatisticCount(string salesDateStart, string salesDateEnd,string productName, string clientId, string machineId, string tradeStatus)
        {
            var result = 0;

            string userClientId = clientId;
            if (string.IsNullOrEmpty(userClientId))
            {
                userClientId = HttpContextHandler.GetHeaderObj("UserClientId").ToString();
            }

            if (string.IsNullOrEmpty(userClientId))
            {
                return 0;
            }
            var conditions = new List<Condition>();

            string clientIds = new CommonService().GetClientIds(userClientId.ToString());
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

            string[] statusArr = tradeStatus.Split('^');
            if (statusArr.Length == 1)
            {
                conditions.Add(new Condition
                {
                    LeftBrace = " AND ",
                    ParamName = "TradeStatus",
                    DbColumnName = "a.trade_status",
                    ParamValue = statusArr[0],
                    Operation = ConditionOperate.Equal,
                    RightBrace = "",
                    Logic = ""
                });
            }
            else
            {
                for (int i = 0; i < statusArr.Length; i++)
                {
                    if (i == 0 && !string.IsNullOrEmpty(statusArr[i]))
                    {
                        conditions.Add(new Condition
                        {
                            LeftBrace = " AND (",
                            ParamName = "TradeStatus" + i,
                            DbColumnName = "a.trade_status",
                            ParamValue = statusArr[i],
                            Operation = ConditionOperate.Equal,
                            RightBrace = "",
                            Logic = ""
                        });
                    }
                    else if (i == statusArr.Length - 1 && !string.IsNullOrEmpty(statusArr[i]))
                    {
                        conditions.Add(new Condition
                        {
                            LeftBrace = " OR ",
                            ParamName = "TradeStatus" + i,
                            DbColumnName = "a.trade_status",
                            ParamValue = statusArr[i],
                            Operation = ConditionOperate.Equal,
                            RightBrace = ")",
                            Logic = ""
                        });
                    }
                    else
                    {
                        conditions.Add(new Condition
                        {
                            LeftBrace = " OR ",
                            ParamName = "TradeStatus" + i,
                            DbColumnName = "a.trade_status",
                            ParamValue = statusArr[i],
                            Operation = ConditionOperate.Equal,
                            RightBrace = "",
                            Logic = ""
                        });
                    }
                }
            }

            if (!string.IsNullOrEmpty(salesDateStart))
            {
                conditions.Add(new Condition
                {
                    LeftBrace = " AND ",
                    ParamName = "SaleDateStart",
                    DbColumnName = "a.pay_date",
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
                    DbColumnName = "a.pay_date",
                    ParamValue = Convert.ToDateTime(salesDateEnd).AddDays(1),
                    Operation = ConditionOperate.LessThan,
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
                    DbColumnName = "a.machine_id",
                    ParamValue = machineId.Trim(),
                    Operation = ConditionOperate.Equal,
                    RightBrace = "",
                    Logic = ""
                });
            }

            if (!string.IsNullOrEmpty(productName))
            {
                conditions.Add(new Condition
                {
                    LeftBrace = " AND ",
                    ParamName = "ProductName",
                    DbColumnName = "a.wares_name",
                    ParamValue = "%"+productName.Trim()+"%",
                    Operation = ConditionOperate.Like,
                    RightBrace = "",
                    Logic = ""
                });
            }



            result = GenerateDal.CountByConditions(CommonSqlKey.GetProductStatisticCount, conditions);

            return result;
        }
    }
}
