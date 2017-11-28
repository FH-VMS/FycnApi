using System;
using System.CodeDom.Compiler;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fycn.Utility;
using Newtonsoft.Json;

namespace Fycn.SqlDataAccess
{
    public class CommSqlText
    {
        public const string SqlTextName = "SqlConfigText";

        private static ConcurrentDictionary<CommonSqlKey, string> _instance;

        private static ConcurrentDictionary<CommonSqlKey, List<string>> _sqlParms;

        public static ConcurrentDictionary<CommonSqlKey, string> Instance
        {
            get
            {
                if (_instance != null && _instance.Count > 0)
                {
                    return _instance;
                }
                try
                {
                    var sqlDic = GetSqlDictionary(null);
                    _instance = new ConcurrentDictionary<CommonSqlKey, string>();
                    foreach (var sql in sqlDic)
                    {
                        if(Enum.IsDefined(typeof(CommonSqlKey), sql.Key))
                        {
                            CommonSqlKey key = (CommonSqlKey)Enum.Parse(typeof(CommonSqlKey), sql.Key);
                            if (_instance.ContainsKey(key))
                            {
                                continue;
                            }
                            _instance.AddOrUpdate(key, sql.Value, (sqlKey, s) => sql.Value);
                        }
                        
                        
                    }
                }
                catch (Exception ee)
                {
                    throw new Exception("SQLTXT DIC ERROR - " + ee.Message);
                }
                return _instance;
            }
        }


        public static ConcurrentDictionary<CommonSqlKey, List<string>> SqlParms
        {
            get
            {
                if (_sqlParms != null && _sqlParms.Count > 0)
                {
                    return _sqlParms;
                }
                _sqlParms = new ConcurrentDictionary<CommonSqlKey, List<string>>();
                foreach (var instance in Instance)
                {
                    List<string> value = GetSqlParmListWithContent(instance.Value);
                    _sqlParms.AddOrUpdate(instance.Key, value, (sqlKey, s) => value);
                }
                return _sqlParms;
            }
        }

        private static List<string> GetSqlParmListWithContent(string sqlContent)
        {
            var retList = RegexHandler.GetAllMatchList(sqlContent, "\\:[\\w]*");
            return retList.Select(ret => ret.Replace(":", "").ToUpper()).ToList();
        }

        public static Dictionary<string, string> GetSqlDictionary(string sqlTxtName)
        {
            var configPath = Directory.GetCurrentDirectory() + "\\" + SqlTextName + ".sqlconfig";

            var sqlConfigFile = File.ReadAllText(configPath);
            //return TTT.Main(sqlConfigFile) as Dictionary<string, string>;
            return GetObjFromText(String.IsNullOrEmpty(sqlTxtName) ? sqlConfigFile : sqlTxtName, "SqlConfigText", "SqlConfigDic") as Dictionary<string, string>;
        }

        private static object GetObjFromText(string sqlObjText, string className, string methodName)
        {
            /*
            var coms = CodeDomProvider.CreateProvider("C#");
            var coms_ = new CompilerParameters();
            //coms_.ReferencedAssemblies.Add("System.dll");
            coms_.GenerateExecutable = false;
            coms_.GenerateInMemory = true;

            var comres = coms.CompileAssemblyFromSource(coms_, sqlObjText);

            var ass = comres.CompiledAssembly;

            var asseval = ass.CreateInstance(className);
            if (asseval != null)
            {
                var method = asseval.GetType().GetMethod(methodName);
                var reobj = method.Invoke(asseval, null);
                GC.Collect();
                return reobj;
            }
            */

            //Dictionary<string, object> JsonData = (Dictionary<string, object>)s.DeserializeObject(sqlObjText);
            return JsonConvert.DeserializeObject<Dictionary<string, string>>(sqlObjText);
            //return null;
        }
    }
}
