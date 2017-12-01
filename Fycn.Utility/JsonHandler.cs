using Fycn.Model.Sys;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace Fycn.Utility
{
    public class JsonHandler
    {
        public static T GetObjectFromJson<T>(string jsonStr)
        {
            if (String.IsNullOrEmpty(jsonStr)) jsonStr = "[]";
            if (jsonStr.IndexOf("%7b") >= 0 || jsonStr.IndexOf("%7d") >= 0 || jsonStr.ToUpper().IndexOf("%E") >= 0)
            {
                jsonStr = HttpUtility.UrlDecode(jsonStr);
            }
            return JsonConvert.DeserializeObject<T>(jsonStr, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });
        }

        public static T GetTryObjectFromJson<T>(string jsonStr, out bool isSuccess)
        {
            try
            {
                isSuccess = true;
                return GetObjectFromJson<T>(jsonStr);
            }
            catch (Exception)
            {
                isSuccess = false;
                return Activator.CreateInstance<T>();
            }
        }


        public static string GetJsonStrFromObject(object o)
        {
            return GetJsonStrFromObject(o, true);
        }

        public static string GetJsonStrFromObject(object o, bool needEncode)
        {
            var jsonSeriaSetting = new JsonSerializerSettings
            {
                DateFormatHandling = DateFormatHandling.MicrosoftDateFormat,
            };

            var str = JsonConvert.SerializeObject(o, jsonSeriaSetting);
            return needEncode ? HttpUtility.UrlEncode(str) : str;
        }

        public static string DataTable2Json(DataTable dt)
        {
            StringBuilder jsonBuilder = new StringBuilder();
            jsonBuilder.Append("[");
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                jsonBuilder.Append("{");
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    jsonBuilder.Append("\"");
                    jsonBuilder.Append(dt.Columns[j].ColumnName);
                    jsonBuilder.Append("\":\"");
                    jsonBuilder.Append(dt.Rows[i][j].ToString());
                    jsonBuilder.Append("\",");
                }
                jsonBuilder.Remove(jsonBuilder.Length - 1, 1);
                jsonBuilder.Append("},");
            }
            jsonBuilder.Remove(jsonBuilder.Length - 1, 1);
            jsonBuilder.Append("]");
          
            return jsonBuilder.ToString();
        }

        public static DataTable JsonToDataTable(string strJson)
        {
            //转换json格式
            strJson = strJson.Replace(",\"", "*\"").Replace("\":", "\"#").ToString();
            //取出表名  
            var rg = new Regex(@"(?<={)[^:]+(?=:\[)", RegexOptions.IgnoreCase);
            string strName = rg.Match(strJson).Value;
            DataTable tb = null;
            //去除表名  
            strJson = strJson.Substring(strJson.IndexOf("[") + 1);
            strJson = strJson.Substring(0, strJson.IndexOf("]"));
            //获取数据  
            rg = new Regex(@"(?<={)[^}]+(?=})");
            MatchCollection mc = rg.Matches(strJson);
            for (int i = 0; i < mc.Count; i++)
            {
                string strRow = mc[i].Value;
                string[] strRows = strRow.Split('*');
                //创建表  
                if (tb == null)
                {
                    tb = new DataTable();
                    tb.TableName = strName;
                    foreach (string str in strRows)
                    {
                        var dc = new DataColumn();
                        string[] strCell = str.Split('#');
                        if (strCell[0].Substring(0, 1) == "\"")
                        {
                            int a = strCell[0].Length;
                            dc.ColumnName = strCell[0].Substring(1, a - 2);
                        }
                        else
                        {
                            dc.ColumnName = strCell[0];
                        }
                        tb.Columns.Add(dc);
                    }
                    tb.AcceptChanges();
                }
                //增加内容  
                DataRow dr = tb.NewRow();
                for (int r = 0; r < strRows.Length; r++)
                {
                    dr[r] = strRows[r].Split('#')[1].Trim().Replace("，", ",").Replace("：", ":").Replace("\"", "");
                }
                tb.Rows.Add(dr);
                tb.AcceptChanges();
            }
            return tb;
        } 
    }
}
