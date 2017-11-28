using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fycn.SqlDataAccess
{
    public class Condition
    {
        public string LeftBrace
        {
            get;
            set;
        }

        public string DbColumnName { get; set; }
        public string ParamName
        {
            get;
            set;
        }
        public ConditionOperate Operation
        {
            get;
            set;
        }
        public object ParamValue
        {
            get;
            set;
        }
        public string RightBrace
        {
            get;
            set;
        }
        public string Logic
        {
            get;
            set;
        }
    }

    public enum ConditionOperate : byte
    {
        Equal,
        NotEqual,
        Greater,
        GreaterThan,
        Less,
        LessThan,
        Null,
        NotNull,
        Like,
        NotLike,
        LeftLike,
        RightLike,
        Yesterday,
        Today,
        Tomorrow,
        LastWeek,
        ThisWeek,
        NextWeek,
        LastMonth,
        ThisMonth,
        NextMonth,
        BeforeDay,
        AfterDay,
        LimitIndex,
        LimitLength,
        None,
        OrderBy,
        GroupBy,
        IN,
        INWithNoPara
    }
}
