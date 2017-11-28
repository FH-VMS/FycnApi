using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fycn.Utility
{
    public class DateTimeHandler
    {
        public static DateTime CurrentTime
        {
            get { return DateTime.Now; }
        }

        public static DateTime GetMaxDateTime()
        {
            return new DateTime(2100, 1, 1);
        }

        public static DateTime GetMinDateTime()
        {
            return new DateTime(1901, 1, 1);
        }

        /// <summary>
        /// yyyy-MM-dd HH:mm:ss
        /// </summary>
        public const string LongDateTimeStyle = "yyyy-MM-dd HH:mm:ss";

        /// <summary>
        /// yyyy-MM-dd HH:mm
        /// </summary>
        public const string ShortDateTimeStyle = "yyyy-MM-dd HH:mm";

        /// <summary>
        /// yyyy-MM-dd
        /// </summary>
        public const string ShortDateStyle = "yyyy-MM-dd";

        /// <summary>
        /// d日 HH:mm
        /// </summary>
        public const string ShortTimeStyle = "dd日 HH:mm";

        /// <summary>
        /// yyyy/MM/dd hh24miss
        /// </summary>
        public const string LongDateTimeStyleForOracle = "yyyy/MM/dd hh24miss";

        public const string LongDateTimeStyleToOracleString = "yyyy/MM/dd HHmmss";

        /// <summary>
        /// yyyy/MM/dd hh24mi
        /// </summary>
        public const string ShortDateTimeStyleForOracle = "yyyy/MM/dd hh24mi";

        public const string ShortDateTimeStyleToOracleString = "yyyy/MM/dd HHmm";

        /// <summary>
        /// yyyy/MM/dd
        /// </summary>
        public const string ShortDateStyleForOracle = "yyyy/MM/dd";

        /// <summary>
        /// yyyy-MM-dd
        /// </summary>
        public const string ShortDateStyleWithLine = "yyyy-MM-dd";

        /// <summary>
        /// 短日期格式 hh:mm:ss
        /// </summary>
        public const string ShortTimeStyleHHMMSS = "hh:mm:ss";

        /// <summary>
        /// yyyyMMddHHmmss
        /// </summary>
        public const string DateTimeNumberStyle = "yyyyMMddHHmmss";

        public static int GetUnixTimeWithDateTime(DateTime dt)
        {
            //return Convert.ToInt32((dt - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds);
            var dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            var dtNow = DateTime.Parse(CurrentTime.ToString());
            var toNow = dtNow.Subtract(dtStart);
            var timeStamp = toNow.Ticks.ToString();
            return Convert.ToInt32(timeStamp.Substring(0, timeStamp.Length - 7));

        }

        public static DateTime GetDateTimeWithUnixTime(int ut)
        {
            var dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            var lTime = long.Parse(ut + "0000000");
            var toNow = new TimeSpan(lTime);
            return dtStart.Add(toNow);
        }

        public static DateTime GetDateTimeByString(string strDT)
        {
            var retDt = new DateTime();
            var dtFormat = new DateTimeFormatInfo { ShortDatePattern = ShortDateStyleWithLine };
            var ret = DateTime.TryParse(strDT, dtFormat, DateTimeStyles.AllowInnerWhite, out retDt);
            return ret ? retDt : DateTime.MinValue;
        }

        public static string GetChineseDayofWeek(int day)
        {
            var result = String.Empty;
            switch (day)
            {
                case 1:
                    result = "星期天";
                    break;
                case 2:
                    result = "星期一";
                    break;
                case 3:
                    result = "星期二";
                    break;
                case 4:
                    result = "星期三";
                    break;
                case 5:
                    result = "星期四";
                    break;
                case 6:
                    result = "星期五";
                    break;
                case 7:
                    result = "星期六";
                    break;
            }
            return result;
        }

        public static string GetDayOfWeek(DayOfWeek df)
        {
            switch (df.ToString())
            {
                case "Monday":
                    return "1";
                case "Tuesday":
                    return "2";
                case "Wednesday":
                    return "3";
                case "Thursday":
                    return "4";
                case "Friday":
                    return "5";
                case "Saturday":
                    return "6";
                case "Sunday":
                    return "7";
            }
            return string.Empty;
        }

        /// <summary>
        /// 根据日期得到当前季度
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static int GetQuarterByDateTime(DateTime dateTime)
        {
            return ((dateTime.Month - 1) / 3) + 1;
        }


        /// <summary>
        /// 根据指定时间点来计算当时年龄
        /// </summary>
        /// <param name="birthdayDate">出生日期</param>
        /// <param name="dateTime">指定时间点</param>
        /// <returns>年龄结果</returns>
        public static string CalculateAge(DateTime birthdayDate, DateTime dateTime)
        {
            try
            {
                //2010-08-23
                //≤28天（或1月以内） 按天 
                //1岁以内 按几月几天 
                //12周岁以内 按几岁几月（月份按四舍五入）
                DateTime curDate = dateTime;
                int nYear = curDate.Year - birthdayDate.Year;
                int nMonth = curDate.Month - birthdayDate.Month;
                int nDay = curDate.Day - birthdayDate.Day;

                int nDaysAMonth = 0;
                int nMonthJudge = 0;
                int nYearJudge = 0;
                if (nDay < 0)
                {
                    nYearJudge = curDate.Year;
                    nMonthJudge = curDate.Month - 1;
                    if (nMonthJudge == 0)
                    {
                        nMonthJudge = 12;
                        nYearJudge -= 1;
                    }
                }
                else
                {
                    nYearJudge = curDate.Year;
                    nMonthJudge = curDate.Month;
                }

                switch (nMonthJudge)
                {
                    case 1:
                    case 3:
                    case 5:
                    case 7:
                    case 8:
                    case 10:
                    case 12:
                        nDaysAMonth = 31;
                        break;
                    case 4:
                    case 6:
                    case 9:
                    case 11:
                        nDaysAMonth = 30;
                        break;
                    default:
                        nDaysAMonth = ((nYearJudge % 4 == 0) ? 29 : 28);
                        break;
                }

                if (nDay < 0)
                {
                    nMonth -= 1;
                    nDay += nDaysAMonth;
                }
                if (nMonth < 0)
                {
                    nYear -= 1;
                    nMonth += 12;
                }

                //年龄计算是负的话，返回""
                if (nYear < 0 || nMonth < 0 || nDay < 0)
                {
                    return "";
                }

                //≤28天（或1月以内） 按天 
                if (nYear == 0 && nMonth == 0)
                {
                    return nDay + "天";
                }
                //1岁以内 按几月几天
                else if (nYear == 0)
                {
                    string sReturn = "";
                    sReturn += nMonth + "月";
                    if (nDay > 0)
                    {
                        sReturn += nDay + "天";
                    }
                    return sReturn;
                }
                //12周岁以内 按几岁几月（月份按四舍五入）
                else if (nYear < 12)
                {
                    string sReturn = "";
                    sReturn += nYear + "岁";

                    //四舍五入计算用
                    if ((double)nDay / nDaysAMonth >= 0.5)
                    {
                        nMonth += 1;
                    }

                    if (nMonth > 0)
                    {
                        sReturn += nMonth + "月";
                    }
                    return sReturn;

                }
                else
                {
                    return nYear + "岁";
                }
            }
            catch
            {
                return "";
            }
        }

        /// <summary>
        /// 根据指定格式转换时间
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static DateTime ConvertToDateTime(string dt, string format = "yyyy/MM/dd HHmm")
        {
            DateTime time;
            var dtfi = new CultureInfo("zh-CN", false).DateTimeFormat;
            DateTime.TryParseExact(dt, format, dtfi, DateTimeStyles.None, out time);
            return time;
        }

        public static DateTime GetDateTime(object obj)
        {
            DateTime result;
            if (obj != null && obj.ToString() != "")
            {
                result = DateTime.Parse(obj.ToString());
            }
            else
            {
                result = DateTime.Now;
            }
            return result;
        }

        public static string GetToday()
        {
            return DateTime.Now.ToString("yyyy-MM-dd");
        }

        /// <summary>
        /// 获取日期的当月第一天
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static DateTime GetMonthFirstDay(DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, 1);
        }

        /// <summary>
        /// 获取日期的当月的最后一天
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static DateTime GetMonthLastDay(DateTime dateTime)
        {
            return GetMonthFirstDay(dateTime).AddMonths(1).AddDays(-1);
        }
    }
}
