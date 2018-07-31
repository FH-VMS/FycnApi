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

        //public static string NotidyAddr = "http://www.fy-cn.top:8088/api";//正式
        public static string RootAliMchId = "2088621838347114"; //分账的支付宝商户号
        public static string RootWeixinMchId = "1433899402"; //分账的微信商户号
    }
}
