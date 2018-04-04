using System;
using System.Collections.Generic;
using System.Text;

namespace Fycn.Utility
{
    public class PayHelper
    {
        public static string GeneraterTradeNo()
        {
            Random ran = new Random();
            int RandKey = ran.Next(1000, 9999);
            string out_trade_no = DateTime.Now.ToString("yyyyMMddHHmmssffff") + RandKey.ToString();
            return out_trade_no;
        }
        
    }
}
