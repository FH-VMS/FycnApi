using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fycn.Utility
{

    public interface ILogger
    {
        void LogTrace(string sLogInfo);
        void LogInfo(string sLogInfo);
        void LogInfo(string sLogInfo, int logLevel, LogType logType);
    }
}
