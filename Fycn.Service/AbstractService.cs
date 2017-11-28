using Fycn.SqlDataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Fycn.Utility;
using Fycn.Model.Sys;
using Microsoft.Extensions.Options;

namespace Fycn.Service
{
    public class AbstractService
    {
        
        private static ISqlGenerator _generateDal;



        protected ISqlGenerator GenerateDal
        {
            get
            {
                if (_generateDal == null)
                {
                    try
                    {
                        var projectName = ConfigurationManager.AppSettings["ProjectName"];
                        var dalAssemble = Assembly.Load(projectName+".SqlDataAccess");

                        if (dalAssemble != null)

                            _generateDal = (ISqlGenerator)dalAssemble.CreateInstance(projectName+".SqlDataAccess.SqlGenerator");
                        else
                            throw new Exception("no generatedal defined");
                    }
                    catch (Exception ee)
                    {
                        //Logger.LogInfo(ee.Message, 0, LogType.FATAL);
                        throw new Exception(ee.Message);
                    }

                }
                return _generateDal;
            }
        }

        protected static IEnumerable<Condition> CreatePaginConditions(int pageIndex, int pageSize)
        {
            var conditions = new List<Condition>();
            if (pageIndex == 1)
            {
                var conditionIndex = new Condition
                {
                    LeftBrace = "",
                    ParamName = "Index",
                    DbColumnName = "",
                    ParamValue = pageIndex - 1,
                    Operation = ConditionOperate.LimitIndex,
                    RightBrace = "",
                    Logic = ""

                };
                conditions.Add(conditionIndex);
            }
            else
            {
                var conditionIndex = new Condition
                {
                    LeftBrace = "",
                    ParamName = "Index",
                    DbColumnName = "",
                    ParamValue = (pageIndex - 1) * pageSize,
                    Operation = ConditionOperate.LimitIndex,
                    RightBrace = "",
                    Logic = ""

                };
                conditions.Add(conditionIndex);
            }

            var conditionLength = new Condition
            {
                LeftBrace = "",
                ParamName = "Length",
                DbColumnName = "",
                ParamValue = pageSize,
                Operation = ConditionOperate.LimitLength,
                RightBrace = "",
                Logic = ""

            };
            conditions.Add(conditionLength);

            return conditions;
        }

    }
}
