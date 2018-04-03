using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fycn.PaymentLib
{
    public class PathConfig
    {
        //public static string DomainConfig = "http://www.fy-cn.top/p";
        public static string DomainConfig = "http://wechat.markhsiu.com/p";

        public static string NotidyAddr = "http://120.27.217.224:8088/api";//测试

        //public static string NotidyAddr = "http://106.14.190.9:8088/api";//正式
        public static string RootAliMchId = "2088621838347114"; //分账的支付宝商户号
        public static string RootWeixinMchId = "1433899402"; //分账的微信商户号
    }
}
