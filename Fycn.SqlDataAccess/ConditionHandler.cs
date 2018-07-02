using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fycn.Utility;

namespace Fycn.SqlDataAccess
{
    public class ConditionHandler
    {
        public static string GetWhereSql(IList conditions, out List<DbParameter> parameter, CommonSqlKey sqlKey,string orderField = "", string orderType = "")
        {
            var text = "?";
            var preParaName = "p_";
            var list = new List<DbParameter>();
            var stringBuilder = new StringBuilder();

            var conditionsCount = ConditionsCounter(conditions);
            if (conditions.Count > 0 && conditionsCount > 0)
            {
                // stringBuilder.Append(" AND");
            }
            var num = 0;
            foreach (Condition condition in conditions)
            {
                if (condition.Operation == ConditionOperate.None)
                {
                    continue;
                }
                if (condition.ParamValue != null)
                {
                    string text2;
                    if (string.IsNullOrEmpty(condition.Logic))
                    {
                        text2 = "";
                    }
                    else
                    {
                        text2 = ((condition.Logic == "AND") ? "AND" : "OR");
                    }
                    if (conditions.Count - 1 == num)
                    {
                        text2 = "";
                    }
                    var paramName = condition.DbColumnName;
                    var text3 = preParaName + condition.ParamName;
                    switch (condition.Operation)
                    {
                        case ConditionOperate.Equal:
                            stringBuilder.Append(string.Concat(new string[]
                        {
                            " ",
                            condition.LeftBrace,
                            paramName,
                            " = ",
                            text,
                            text3,
                            condition.RightBrace,
                            " ",
                            text2
                        }));
                            list.Add(CreateDbParameter(text + text3, condition.ParamValue));
                            break;
                        case ConditionOperate.IN:
                            stringBuilder.Append(string.Concat(new string[]
                        {
                            " ",
                            condition.LeftBrace,
                            paramName,
                            " in ",
                            "(",
                            text,
                            text3,
                            ")",
                            condition.RightBrace,
                            " ",
                            text2
                        }));
                            list.Add(CreateDbParameter(text + text3, condition.ParamValue));
                            break;
                        case ConditionOperate.INWithNoPara:
                            stringBuilder.Append(string.Concat(new string[]
                        {
                            " ",
                            condition.LeftBrace,
                            paramName,
                            " in ",
                            "(",
                          condition.ParamValue.ToString(),
                            ")",
                            condition.RightBrace,
                            " ",
                            text2
                        }));
                            
                            break;
                        case ConditionOperate.NotEqual:
                            stringBuilder.Append(string.Concat(new string[]
                        {
                            " ",
                            condition.LeftBrace,
                            paramName,
                            " <> ",
                            text,
                            text3,
                            condition.RightBrace,
                            " ",
                            text2
                        }));
                            list.Add(CreateDbParameter(text + text3, condition.ParamValue));
                            break;
                        case ConditionOperate.Greater:
                            stringBuilder.Append(string.Concat(new string[]
                        {
                            " ",
                            condition.LeftBrace,
                            paramName,
                            " > ",
                            text,
                            text3,
                            condition.RightBrace,
                            " ",
                            text2
                        }));
                            list.Add(CreateDbParameter(text + text3, condition.ParamValue));
                            break;
                        case ConditionOperate.GreaterThan:
                            stringBuilder.Append(string.Concat(new string[]
                        {
                            " ",
                            condition.LeftBrace,
                            paramName,
                            " >= ",
                            text,
                            text3,
                            condition.RightBrace,
                            " ",
                            text2
                        }));
                            list.Add(CreateDbParameter(text + text3, condition.ParamValue));
                            break;
                        case ConditionOperate.Less:
                            stringBuilder.Append(string.Concat(new string[]
                        {
                            " ",
                            condition.LeftBrace,
                            paramName,
                            " < ",
                            text,
                            text3,
                            condition.RightBrace,
                            " ",
                            text2
                        }));
                            list.Add(CreateDbParameter(text + text3, condition.ParamValue));
                            break;
                        case ConditionOperate.LessThan:
                            stringBuilder.Append(string.Concat(new string[]
                        {
                            " ",
                            condition.LeftBrace,
                            paramName,
                            " <= ",
                            text,
                            text3,
                            condition.RightBrace,
                            " ",
                            text2
                        }));
                            list.Add(CreateDbParameter(text + text3, condition.ParamValue));
                            break;
                        case ConditionOperate.Null:
                            stringBuilder.Append(string.Format(" " + condition.LeftBrace + "{0} is null ", paramName) + condition.RightBrace + " " + text2);
                            break;
                        case ConditionOperate.NotNull:
                            stringBuilder.Append(string.Format(" " + condition.LeftBrace + "{0} is not null ", paramName) + condition.RightBrace + " " + text2);
                            break;
                        case ConditionOperate.OrderBy:
                            stringBuilder.Append(string.Format(" " + condition.LeftBrace + "order by {0} {1} ", paramName, condition.ParamValue) + condition.RightBrace + " " + text2);
                            break;
                        case ConditionOperate.GroupBy:
                            stringBuilder.Append(string.Format(" " + condition.LeftBrace + "group by {0} {1} ", paramName, condition.ParamValue) + condition.RightBrace + " " + text2);
                            break;
                        case ConditionOperate.LimitIndex:
                            //stringBuilder.Append($"  limit {text + "p_" + condition.ParamName}");
                            stringBuilder.Append(string.Format(" limit {0} ", text + preParaName + condition.ParamName));
                            break;
                        case ConditionOperate.LimitLength:
                            //stringBuilder.Append($",{text + "p_" + condition.ParamName} ");
                            stringBuilder.Append(string.Format(" ,{0} ", text + preParaName + condition.ParamName));
                            break;
                        case ConditionOperate.Like:
                            stringBuilder.Append(string.Concat(new string[]
                        {
                            " ",
                            condition.LeftBrace,
                            paramName,
                            " like ",
                            text,
                            text3,
                            condition.RightBrace,
                            " ",
                            text2
                        }));
                            list.Add(CreateDbParameter(text + text3, "%" + condition.ParamValue + "%"));
                            break;
                        case ConditionOperate.NotLike:
                            stringBuilder.Append(string.Concat(new string[]
                        {
                            " ",
                            condition.LeftBrace,
                            paramName,
                            " not like ",
                            text,
                            text3,
                            condition.RightBrace,
                            " ",
                            text2
                        }));
                            list.Add(CreateDbParameter(text + text3, "%" + condition.ParamValue + "%"));
                            break;
                        case ConditionOperate.LeftLike:
                            stringBuilder.Append(string.Concat(new string[]
                        {
                            " ",
                            condition.LeftBrace,
                            paramName,
                            " like ",
                            text,
                            text3,
                            condition.RightBrace,
                            " ",
                            text2
                        }));
                            list.Add(CreateDbParameter(text + text3, "%" + condition.ParamValue));
                            break;
                        case ConditionOperate.RightLike:
                            stringBuilder.Append(string.Concat(new string[]
                        {
                            " ",
                            condition.LeftBrace,
                            paramName,
                            " like ",
                            text,
                            text3,
                            condition.RightBrace,
                            " ",
                            text2
                        }));
                            list.Add(CreateDbParameter(text + text3, condition.ParamValue + "%"));
                            break;

                        case ConditionOperate.Yesterday:
                            {
                                var dateTime = DateTimeHandler.GetDateTime(DateTime.Now.AddDays(-1.0).ToString("yyyy-MM-dd") + " 00:00");
                                DateTime dateTime2 = DateTimeHandler.GetDateTime(DateTime.Now.AddDays(-1.0).ToString("yyyy-MM-dd") + " 23:59");
                                stringBuilder.Append(string.Format(string.Concat(new string[]
                        {
                            " ",
                            condition.LeftBrace,
                            "{0} between ",
                            text,
                            "start{1}  AND ",
                            text,
                            "end_{2}",
                            condition.RightBrace
                        }), paramName, text3, text3) + " " + text2);
                                list.Add(CreateDbParameter(text + string.Format("start{0}", text3), dateTime));
                                list.Add(CreateDbParameter(text + string.Format("end_{0}", text3), dateTime2));
                                break;
                            }
                        case ConditionOperate.Today:
                            {
                                DateTime dateTime = DateTimeHandler.GetDateTime(DateTimeHandler.GetToday() + " 00:00");
                                DateTime dateTime2 = DateTimeHandler.GetDateTime(DateTimeHandler.GetToday() + " 23:59");
                                stringBuilder.Append(string.Format(string.Concat(new string[]
                        {
                            " ",
                            condition.LeftBrace,
                            "{0} between ",
                            text,
                            "start{1}  AND ",
                            text,
                            "end_{2}",
                            condition.RightBrace
                        }), paramName, text3, text3) + " " + text2);
                                list.Add(CreateDbParameter(text + string.Format("start{0}", text3), dateTime));
                                list.Add(CreateDbParameter(text + string.Format("end_{0}", text3), dateTime2));
                                break;
                            }
                        case ConditionOperate.Tomorrow:
                            {
                                DateTime dateTime = DateTimeHandler.GetDateTime(DateTime.Now.AddDays(1.0).ToString("yyyy-MM-dd") + " 00:00");
                                DateTime dateTime2 = DateTimeHandler.GetDateTime(DateTime.Now.AddDays(1.0).ToString("yyyy-MM-dd") + " 23:59");
                                stringBuilder.Append(string.Format(string.Concat(new string[]
                        {
                            " ",
                            condition.LeftBrace,
                            "{0} between ",
                            text,
                            "start{1}  AND ",
                            text,
                            "end_{2}",
                            condition.RightBrace
                        }), paramName, text3, text3) + " " + text2);
                                list.Add(CreateDbParameter(text + string.Format("start{0}", text3), dateTime));
                                list.Add(CreateDbParameter(text + string.Format("end_{0}", text3), dateTime2));
                                break;
                            }
                        case ConditionOperate.LastWeek:
                            {
                                DateTime dateTime = DateTimeHandler.GetDateTime(DateTime.Now.AddDays((double)(Convert.ToInt32(1 - Convert.ToInt32(DateTime.Now.DayOfWeek)) - 7)).ToString("yyyy-MM-dd") + " 00:00");
                                DateTime dateTime2 = DateTimeHandler.GetDateTime(dateTime.AddDays(6.0).ToString("yyyy-MM-dd") + " 23:59");
                                stringBuilder.Append(string.Format(string.Concat(new string[]
                        {
                            " ",
                            condition.LeftBrace,
                            "{0} between ",
                            text,
                            "start{1}  AND ",
                            text,
                            "end_{2}",
                            condition.RightBrace
                        }), paramName, text3, text3) + " " + text2);
                                list.Add(CreateDbParameter(text + string.Format("start{0}", text3), dateTime));
                                list.Add(CreateDbParameter(text + string.Format("end_{0}", text3), dateTime2));
                                break;
                            }
                        case ConditionOperate.ThisWeek:
                            {
                                DateTime dateTime = DateTimeHandler.GetDateTime(DateTime.Now.AddDays((double)(1 - Convert.ToInt32(DateTime.Now.DayOfWeek))).ToString("yyyy-MM-dd") + " 00:00");
                                DateTime dateTime2 = DateTimeHandler.GetDateTime(dateTime.AddDays(6.0).ToString("yyyy-MM-dd") + " 23:59");
                                stringBuilder.Append(string.Format(string.Concat(new string[]
                        {
                            " ",
                            condition.LeftBrace,
                            "{0} between ",
                            text,
                            "start{1}  AND ",
                            text,
                            "end_{2}",
                            condition.RightBrace
                        }), paramName, text3, text3) + " " + text2);
                                list.Add(CreateDbParameter(text + string.Format("start{0}", text3), dateTime));
                                list.Add(CreateDbParameter(text + string.Format("end_{0}", text3), dateTime2));
                                break;
                            }
                        case ConditionOperate.NextWeek:
                            {
                                DateTime dateTime = DateTimeHandler.GetDateTime(DateTime.Now.AddDays((double)(Convert.ToInt32(1 - Convert.ToInt32(DateTime.Now.DayOfWeek)) + 7)).ToString("yyyy-MM-dd") + " 00:00");
                                DateTime dateTime2 = DateTimeHandler.GetDateTime(dateTime.AddDays(6.0).ToString("yyyy-MM-dd") + " 23:59");
                                stringBuilder.Append(string.Format(string.Concat(new string[]
                        {
                            " ",
                            condition.LeftBrace,
                            "{0} between ",
                            text,
                            "start{1}  AND ",
                            text,
                            "end_{2}",
                            condition.RightBrace
                        }), paramName, text3, text3) + " " + text2);
                                list.Add(CreateDbParameter(text + string.Format("start{0}", text3), dateTime));
                                list.Add(CreateDbParameter(text + string.Format("end_{0}", text3), dateTime2));
                                break;
                            }
                        case ConditionOperate.LastMonth:
                            {
                                DateTime dateTime = DateTimeHandler.GetDateTime(DateTime.Now.AddMonths(-1).ToString("yyyy-MM-01") + " 00:00");
                                DateTime dateTime2 = DateTimeHandler.GetDateTime(DateTime.Parse(DateTime.Now.ToString("yyyy-MM-01")).AddDays(-1.0).ToString("yyyy-MM-dd") + " 23:59");
                                stringBuilder.Append(string.Format(string.Concat(new string[]
                        {
                            " ",
                            condition.LeftBrace,
                            "{0} between ",
                            text,
                            "start{1}  AND ",
                            text,
                            "end_{2}",
                            condition.RightBrace
                        }), paramName, text3, text3) + " " + text2);
                                list.Add(CreateDbParameter(text + string.Format("start{0}", text3), dateTime));
                                list.Add(CreateDbParameter(text + string.Format("end_{0}", text3), dateTime2));
                                break;
                            }
                        case ConditionOperate.ThisMonth:
                            {
                                DateTime dateTime = DateTimeHandler.GetDateTime(DateTime.Now.ToString("yyyy-MM-01") + " 00:00");
                                DateTime dateTime2 = DateTimeHandler.GetDateTime(DateTime.Now.AddMonths(1).AddDays(-1.0).ToString("yyyy-MM-dd") + " 23:59");
                                stringBuilder.Append(string.Format(string.Concat(new string[]
                        {
                            " ",
                            condition.LeftBrace,
                            "{0} between ",
                            text,
                            "start{1}  AND ",
                            text,
                            "end_{2}",
                            condition.RightBrace
                        }), paramName, text3, text3) + " " + text2);
                                list.Add(CreateDbParameter(text + string.Format("start{0}", text3), dateTime));
                                list.Add(CreateDbParameter(text + string.Format("end_{0}", text3), dateTime2));
                                break;
                            }
                        case ConditionOperate.NextMonth:
                            {
                                DateTime dateTime = DateTimeHandler.GetDateTime(DateTime.Now.AddMonths(1).ToString("yyyy-MM-01") + " 00:00");
                                DateTime dateTime2 = DateTimeHandler.GetDateTime(DateTime.Parse(DateTime.Now.ToString("yyyy-MM-01")).AddMonths(2).AddDays(-1.0).ToString("yyyy-MM-dd") + " 23:59");
                                stringBuilder.Append(string.Format(string.Concat(new string[]
                        {
                            " ",
                            condition.LeftBrace,
                            "{0} between ",
                            text,
                            "start{1}  AND ",
                            text,
                            "end_{2}",
                            condition.RightBrace
                        }), paramName, text3, text3) + " " + text2);
                                list.Add(CreateDbParameter(text + string.Format("start{0}", text3), dateTime));
                                list.Add(CreateDbParameter(text + string.Format("end_{0}", text3), dateTime2));
                                break;
                            }
                        case ConditionOperate.BeforeDay:
                            {
                                DateTime dateTime = DateTimeHandler.GetDateTime(DateTime.Now.AddDays(double.Parse("-" + condition.ParamValue.ToString())) + " 00:00");
                                DateTime dateTime2 = DateTimeHandler.GetDateTime(DateTimeHandler.GetToday() + " 23:59");
                                stringBuilder.Append(string.Format(string.Concat(new string[]
                        {
                            " ",
                            condition.LeftBrace,
                            "{0} between ",
                            text,
                            "start{1}  AND ",
                            text,
                            "end_{2}",
                            condition.RightBrace
                        }), paramName, text3, text3) + " " + text2);
                                list.Add(CreateDbParameter(text + string.Format("start{0}", text3), dateTime));
                                list.Add(CreateDbParameter(text + string.Format("end_{0}", text3), dateTime2));
                                break;
                            }
                        case ConditionOperate.AfterDay:
                            {
                                DateTime dateTime = DateTimeHandler.GetDateTime(DateTime.Now.AddDays(double.Parse(condition.ParamValue.ToString())) + " 00:00");
                                DateTime dateTime2 = DateTimeHandler.GetDateTime(DateTimeHandler.GetToday() + " 23:59");
                                stringBuilder.Append(string.Format(string.Concat(new string[]
                        {
                            " ",
                            condition.LeftBrace,
                            "{0} between ",
                            text,
                            "start{1}  AND ",
                            text,
                            "end_{2}",
                            condition.RightBrace
                        }), paramName, text3, text3) + " " + text2);
                                list.Add(CreateDbParameter(text + string.Format("start{0}", text3), dateTime));
                                list.Add(CreateDbParameter(text + string.Format("end_{0}", text3), dateTime2));
                                break;
                            }
                    }
                    num++;
                }
            }
            if (!string.IsNullOrEmpty(orderField))
            {
                orderType = ((orderType.ToLower() == "desc") ? "desc" : "asc");
                stringBuilder.Append(" Order By " + orderField + " " + orderType);
            }
            parameter = list;
            string finalWhereCondition = stringBuilder.ToString().Trim().ToUpper();
            if (string.IsNullOrEmpty(finalWhereCondition) || finalWhereCondition.Length < 3)
            {
                return finalWhereCondition;
            }
            switch (sqlKey)
            {
                case CommonSqlKey.GetCustomer:
                    return finalWhereCondition;
                case CommonSqlKey.GetCustomerCount:
                    return finalWhereCondition;
                case CommonSqlKey.GetAuth:
                    return finalWhereCondition;
                case CommonSqlKey.GetRankValue:
                    return finalWhereCondition;
                case CommonSqlKey.GetProductByMachine:
                    return finalWhereCondition;
                case CommonSqlKey.GetProductByMachineCount:
                    return finalWhereCondition;
                case CommonSqlKey.GetTotalMoneyByClient:
                    return finalWhereCondition;
                case CommonSqlKey.GetSalesAmountByMachine:
                    return finalWhereCondition;
                case CommonSqlKey.GetSalesAmountByMachineCount:
                    return finalWhereCondition;
                case CommonSqlKey.GetStatisticSalesMoneyByDate:
                    return finalWhereCondition;
                case CommonSqlKey.GetLogin:
                    return finalWhereCondition;
                case CommonSqlKey.GetUser:
                    return finalWhereCondition;
                case CommonSqlKey.GetUserCount:
                    return finalWhereCondition;
                case CommonSqlKey.GetMenuByUser:
                    return finalWhereCondition;
                case CommonSqlKey.GetAuthDic:
                    return finalWhereCondition;
                case CommonSqlKey.GetAuthByDmsId:
                    return finalWhereCondition;
                case CommonSqlKey.GetDic:
                    return finalWhereCondition;
                case CommonSqlKey.GetRank:
                    return finalWhereCondition;
                case CommonSqlKey.GetMachineCountWithStatus:
                    return finalWhereCondition;
                case CommonSqlKey.GetMachineType:
                    return finalWhereCondition;
                case CommonSqlKey.ExportByTunnel:
                    return finalWhereCondition;
                case CommonSqlKey.ExportByProduct:
                    return finalWhereCondition;
                case CommonSqlKey.GetProductInfoByWaresId:
                    return finalWhereCondition;
            }
            if (finalWhereCondition.Substring(0, 3) == "AND")
            {
                finalWhereCondition = " WHERE " + finalWhereCondition.Substring(3, finalWhereCondition.Length-3) + " ";
            }
            return finalWhereCondition;
        }

        public static DbParameter CreateDbParameter()
        {
            DbParameter result = new MySqlParameter();
            return result;
        }

        public static DbParameter CreateDbParameter(string paramName, object value)
        {
            DbParameter dbParameter = CreateDbParameter();
            dbParameter.ParameterName = paramName;
            dbParameter.Value = value;
            return dbParameter;
        }
        public static DbParameter CreateDbParameter(string paramName, object value, DbType dbType)
        {
            DbParameter dbParameter = CreateDbParameter();
            dbParameter.DbType = dbType;
            dbParameter.ParameterName = paramName;
            dbParameter.Value = value;
            return dbParameter;
        }
        public static DbParameter CreateDbParameter(string paramName, object value, DbType dbType, int size)
        {
            DbParameter dbParameter = CreateDbParameter();
            dbParameter.DbType = dbType;
            dbParameter.ParameterName = paramName;
            dbParameter.Value = value;
            dbParameter.Size = size;
            return dbParameter;
        }
        public static DbParameter CreateDbParameter(string paramName, object value, int size)
        {
            DbParameter dbParameter = CreateDbParameter();
            dbParameter.ParameterName = paramName;
            dbParameter.Value = value;
            dbParameter.Size = size;
            return dbParameter;
        }
        public static DbParameter CreateDbOutParameter(string paramName, int size)
        {
            DbParameter dbParameter = CreateDbParameter();
            dbParameter.Direction = ParameterDirection.Output;
            dbParameter.ParameterName = paramName;
            dbParameter.Size = size;
            return dbParameter;
        }

        /// <summary>
        /// 获取除limit参数以外的参数个数
        /// </summary>
        /// <param name="conditions"></param>
        /// <returns></returns>
        private static int ConditionsCounter(IEnumerable conditions)
        {
            var result = 0;
            foreach (Condition condition in conditions)
            {
                if (condition.Operation == ConditionOperate.LimitIndex ||
                    condition.Operation == ConditionOperate.LimitLength)
                {

                }
                else
                {
                    result++;
                }
            }

            return result;
        }
    }
}
