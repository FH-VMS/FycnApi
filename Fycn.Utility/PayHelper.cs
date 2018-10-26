using Fycn.Model.Pay;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fycn.Utility
{
    public class PayHelper
    {
        public string GeneraterTradeNo()
        {
            Random ran = new Random();
            int RandKey = ran.Next(1000, 9999);
            string out_trade_no = DateTime.Now.ToString("yyyyMMddHHmmssffff") + RandKey.ToString();
            return out_trade_no;
        }

        public KeyJsonModel AnalizeKey(string key)
        {
            KeyJsonModel keyJsonInfo = null;
            try
            {
                keyJsonInfo = JsonHandler.GetObjectFromJson<KeyJsonModel>(key);
            }
            catch (Exception e)
            {
                keyJsonInfo = JsonHandler.GetObjectFromJson<KeyJsonModel>(System.Text.Encoding.Default.GetString(ByteHelper.strToToHexByte(key)));
            }


            return keyJsonInfo;
        }

        //转换成支付需要的datetime
        public DateTime TransStrToDateTime(string strDate, string wOrA)
        {
            try
            {
                if (string.IsNullOrEmpty(strDate))
                {
                    return DateTime.Now;
                }
                if (wOrA == "w")
                {
                    if (strDate.Length == 14)
                    {
                        string year = strDate.Substring(0, 4);
                        string month = strDate.Substring(4, 2);
                        string day = strDate.Substring(6, 2);
                        string hour = strDate.Substring(8, 2);
                        string minute = strDate.Substring(10, 2);
                        string second = strDate.Substring(12, 2);
                        return Convert.ToDateTime(string.Format("{0}-{1}-{2} {3}:{4}:{5}", year, month, day, hour, minute, second));
                    }
                }
                else if (wOrA == "a")
                {
                    return Convert.ToDateTime(strDate);
                }
                return DateTime.Now;
            }
            catch (Exception e)
            {
                return DateTime.Now;
            }


        }

    }
}
