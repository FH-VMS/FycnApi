using System;
using System.Collections.Generic;
using System.Text;

namespace Fycn.PaymentLib.wx
{
    public class TransferModel
    {
        public string partner_trade_no
        {
            get;
            set;
        }

        public string openid
        {
            get;
            set;
        }

        public string re_user_name
        {
            get;
            set;
        }

        public int amount
        {
            get;
            set;
        }

        public string desc
        {
            get;
            set;
        }
        
    }
}
