using Fycn.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fycn.PaymentLib
{
    public class PathConfig
    {
        //public static string DomainConfig = "http://www.fy-cn.top/p";
        public static string DomainConfig
        {
            get
            {
                return ConfigHandler.Domain + "/p";
            }
        }

        public static string NotidyAddr
        {
            get
            {
                return ConfigHandler.Domain + ":8088/api";
            }
        }

        //分账账户的微信商户号和支付宝合作伙伴id
        public static string[] DistrubuteAccounts = { "1433899402", "2088621838347114" };
    }
}
