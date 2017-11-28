using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fycn.Utility
{
    public class LogFactory
    {
        public static ILogger Instance = null;
        public static ILogger GetInstance()
        {
            if (Instance == null)
            {
                var t = Type.GetType("Utility." + ConfigHandler.LogType);
                if (t != null) Instance = (ILogger)Activator.CreateInstance(t, null);
            }
            return Instance;
        }
    }
}
