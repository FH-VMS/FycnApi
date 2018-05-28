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

    }
}
