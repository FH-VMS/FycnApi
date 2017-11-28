using Fycn.Model.Sys;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Castle.Core.Internal;

namespace Fycn.Utility
{
    public class ServiceInfoHelper
    {
        private static List<RemoteServiceEntity> _remoteServiceList;
        public static List<RemoteServiceEntity> RemoteServiceList
        {
            get { return _remoteServiceList ?? (_remoteServiceList = GetRemoteServiceList()); }
        }
        private static List<RemoteServiceEntity> GetRemoteServiceList()
        {
            var typeList = new List<Type>(Assembly.Load("Interface").GetTypes());
            var remoteServiceList = new List<RemoteServiceEntity>();
            foreach (var type in typeList)
            {
                var resModuleName = type.Name.TrimStart('I');
                var methods = type.GetMethods();
                foreach (var methodInfo in methods)
                {
                    var attr = methodInfo.GetAttribute<RemarkAttribute>();
                    if (attr != null)
                    {
                        var remoteService = new RemoteServiceEntity();
                        var parms = methodInfo.GetParameters();
                        var parmsStr = new StringBuilder("(");
                        foreach (var parameterInfo in parms)
                        {
                            var parmName = parameterInfo.Name;
                            var parmType = parameterInfo.ParameterType;
                            parmsStr.AppendFormat("{0} {1},", parmType, parmName);
                        }
                        var returnClass = methodInfo.ReturnType;
                        remoteService.InterfaceNo = attr.No;
                        remoteService.ResModule = resModuleName;
                        remoteService.MethodBehavior = methodInfo.Name;
                        remoteService.InterfaceRemark = attr.Notes;
                        remoteService.ParmsRemark = parms.Length == 0 ? "" : attr.ParmsNote + " ：" + parmsStr.ToString().TrimEnd(',') + ")";
                        remoteService.ReturnClass = returnClass.IsGenericType ? returnClass.GetGenericArguments()[0].Name : returnClass.ToString();
                        remoteService.ReturnRemark = attr.ReturnNote;
                        remoteServiceList.Add(remoteService);
                    }
                }
            }
            //remoteServiceList.Sort();
            //remoteServiceList.Sort(delegate(RemoteServiceEntity x, RemoteServiceEntity y)
            //{
            //    if (x.InterfaceNo == null && y.InterfaceNo == null) return 0;
            //    if (x.InterfaceNo == null) return -1;
            //    if (y.InterfaceNo == null) return 1;
            //    return String.CompareOrdinal(x.InterfaceNo, y.InterfaceNo);
            //});
            remoteServiceList.Sort(delegate(RemoteServiceEntity x, RemoteServiceEntity y)
            {
                return String.CompareOrdinal(x.ResModule, y.ResModule);
            });

            return remoteServiceList;
        }

        private static Dictionary<string, Type> _tableEntityTypes;

        public static Dictionary<string, Type> TableEntityTypes
        {
            get { return _tableEntityTypes ?? (_tableEntityTypes = GetTableEntityTypes()); }
        }

        private static Dictionary<string, Type> GetTableEntityTypes()
        {
            var resDic = new Dictionary<string, Type>();
            var typeList = new List<Type>(Assembly.Load("WiiCare.CoreInterface").GetTypes());
            foreach (var type in typeList)
            {
                var tableInfo = CommDbHelper.GetModelDataTableInfo(type);
                if (String.IsNullOrEmpty(tableInfo.TableName) || resDic.ContainsKey(tableInfo.TableName)) continue;
                resDic.Add(tableInfo.TableName, type);
            }
            return resDic;
        }


        /// <summary>
        /// 生成前端API文件
        /// </summary>
        public static void WriteWebAPI()
        {
            var typeList = new List<Type>(Assembly.Load("Interface").GetTypes());
            var remoteServiceList = new List<RemoteServiceEntity>();
            foreach (var type in typeList)
            {
                var resModuleName = type.Name.TrimStart('I');
                var methods = type.GetMethods();
                foreach (var methodInfo in methods)
                {
                    var attr = methodInfo.GetAttribute<RemarkAttribute>();
                    if (attr != null)
                    {
                        var remoteService = new RemoteServiceEntity();
                        var parms = methodInfo.GetParameters();
                        var parmsStr = new StringBuilder("(");
                        foreach (var parameterInfo in parms)
                        {
                            var parmName = parameterInfo.Name;
                            var parmType = parameterInfo.ParameterType;
                            parmsStr.AppendFormat("{0} {1},", parmType, parmName);
                        }
                        remoteService.InterfaceNo = attr.No;
                        remoteService.ResModule = resModuleName;
                        remoteService.MethodBehavior = methodInfo.Name;
                        remoteService.InterfaceRemark = attr.Notes;
                        remoteService.ParmsRemark = parms.Length == 0 ? "" : attr.ParmsNote + " ：" + parmsStr.ToString().TrimEnd(',') + ")";
                        remoteService.ReturnRemark = attr.ReturnNote;
                        remoteServiceList.Add(remoteService);
                    }
                }
            }
            remoteServiceList.Sort((x, y) => String.CompareOrdinal(x.ResModule, y.ResModule));



            FileStream fs = new FileStream("E:\\APIs.js", FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);

            //开始写入
            sw.WriteLine("/* ****************");
            sw.WriteLine("*	");
            sw.WriteLine("*	平台数据API列表");
            sw.WriteLine("*	");
            sw.WriteLine("*	@后台自动生成");
            sw.WriteLine("*	" + DateTime.Now.ToShortDateString());
            sw.WriteLine("* ");
            sw.WriteLine("**************** */");
            sw.WriteLine("import apiService from './apiService'");
            sw.WriteLine("const api = {");

            var sOldModule = "";
            var moduleList = new List<string>();
            var nIndex = 0;
            foreach (var item in remoteServiceList)
            {
                nIndex++;
                if (moduleList.Contains(item.MethodBehavior))
                {
                    continue;
                }
                moduleList.Add(item.MethodBehavior);
                if (sOldModule != item.ResModule)
                {
                    if (sOldModule != "")
                    {
                        sw.WriteLine("  }, ");
                    }

                    sw.WriteLine("  " + item.ResModule + (item.ResModule == "Bed" ? "s" : "") + ": {");
                }

                sw.WriteLine("      " + item.MethodBehavior + ": function (data) {");
                sw.WriteLine("          return apiService('" + item.ResModule + "', '" + item.MethodBehavior + "', '" + GetInvokeType(item.MethodBehavior) + "', data)");
                sw.Write("      }");

                if (nIndex < remoteServiceList.Count)
                {
                    if (remoteServiceList[nIndex].ResModule == item.ResModule)
                    {
                        sw.Write(",");
                    }
                }
                sw.WriteLine();
                sOldModule = item.ResModule;
            }
            sw.WriteLine("	}");
            sw.WriteLine("}");
            sw.WriteLine(" ");
            sw.WriteLine("export default api");
            sw.WriteLine("	");

            //清空缓冲区
            sw.Flush();
            //关闭流
            sw.Close();
            fs.Close();

        }

        /// <summary>
        /// 生成前端Model文件
        /// </summary>
        public static void WriteWebModels()
        {
            var typeList = new List<Type>(Assembly.Load("Model").GetTypes()).FindAll(t => t.FullName.IndexOf("Sys", StringComparison.Ordinal) == -1);
            typeList.Sort((x, y) => String.CompareOrdinal(x.FullName, y.FullName));

            var fs = new FileStream("E:\\models.js", FileMode.Create);
            var sw = new StreamWriter(fs);
            //开始写入
            sw.WriteLine("/* ****************");
            sw.WriteLine("*	");
            sw.WriteLine("*	数据model列表 ");
            sw.WriteLine("*	");
            sw.WriteLine("*	@后台自动生成");
            sw.WriteLine("*	" + DateTime.Now.ToShortDateString());
            sw.WriteLine("* ");
            sw.WriteLine("**************** */");
            sw.WriteLine("const Front = { ");
            sw.WriteLine("   BaseSetting: {");
            sw.WriteLine("      PageSize: 10");
            sw.WriteLine("   }, ");
            sw.WriteLine(" ");

            var sOldModule = "";
            var nIndex = 0;
            var ingoreList = new List<string>
            {
                "ResultCode",
                "ResultObj`1",
                "<>c__DisplayClass5",
                "<>c__DisplayClassc",
                "<>c__DisplayClass6",
                "<>c__DisplayClassd"
            };
            var isFirstEntity = false;
            foreach (var type in typeList)
            {
                var typeSplit = type.FullName.Split('.');
                if (typeSplit.Length <= 1)
                {
                    continue;
                }
                var sCurModule = typeSplit[typeSplit.Length - 2];

                if (sCurModule.Length <= 0)
                {
                    continue;
                }

                if (sOldModule != sCurModule)
                {
                    if (sOldModule != "")
                    {
                        sw.WriteLine();
                        sw.WriteLine("  },");
                    }

                    sw.WriteLine("  " + sCurModule + ": {");
                    isFirstEntity = true;
                }

                if (!ingoreList.Contains(type.Name))
                {
                    if (!isFirstEntity)
                    {
                        sw.Write(",");
                        sw.WriteLine();
                    }
                    sw.WriteLine("      " + type.Name + ": {");

                    var properties = type.GetProperties();

                    for (var i = 0; i < properties.Length; i++)
                    {
                        sw.Write("          " + properties[i].Name + ": ''");
                        if (i != properties.Length - 1)
                        {
                            sw.WriteLine(",");
                        }
                        else
                        {
                            sw.WriteLine();
                        }
                    }
                    sw.WriteLine("      }");
                    isFirstEntity = false;
                }
                sOldModule = sCurModule;
                nIndex++;
            }
            sw.WriteLine();
            sw.WriteLine("  } ");
            sw.WriteLine("} ");
            sw.WriteLine("export default Front");
            sw.WriteLine(" ");
            //清空缓冲区
            sw.Flush();
            //关闭流
            sw.Close();
            fs.Close();
        }


        public static string GetInvokeType(string methodName)
        {
            var type = "POST";
            string typeName = methodName.ToUpper().Substring(0, 3);
            switch (typeName)
            {
                case "POS":
                    {
                        type = "POST";
                        break;
                    }
                case "GET":
                    {
                        type = "GET";
                        break;
                    }
                case "PUT":
                    {
                        type = "PUT";
                        break;
                    }
                case "DEL":
                    {
                        type = "DELETE";
                        break;
                    }
            }
            return type;
        }
    }
}
