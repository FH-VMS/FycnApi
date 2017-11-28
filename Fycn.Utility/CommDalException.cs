using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fycn.Utility
{
    public class CommDalException : Exception
    {
        /// <summary>
        /// 10 默认等级，20 验证错误 - 字段不为空异常
        /// </summary>
        private int ExceptionLevel { get; set; }

        public string SqlTxt { get; set; }

        public CommDalException(string expMsg)
            : base(expMsg)
        {
            ExceptionLevel = 10;
        }

        public CommDalException(int level, string expMsg)
            : base(expMsg)
        {
            ExceptionLevel = level;
        }
    }
}
