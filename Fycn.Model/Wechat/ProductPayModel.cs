using System;
using System.Collections.Generic;
using System.Text;

namespace Fycn.Model.Wechat
{
    public class ProductPayModel
    {
        public string TradeNo
        {
            get;
            set;
        }

        public string TradeAmount
        {
            get;
            set;
        }

        public string WaresId
        {
            get;
            set;
        }

        public int Number
        {
            get;
            set;
        }

        public string WaresName
        {
            get;
            set;
        }

        public int IsGroup
        {
            get;
            set;
        }
    }
}
