using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fycn.Model.Machine
{
    //机器端修改价格和最大库存
    public class PriceAndMaxStockModel
    {
        public string tid
        {
            get;
            set;
        }

        public decimal p1
        {
            get;
            set;
        }

        public decimal p2
        {
            get;
            set;
        }

        public decimal p3
        {
            get;
            set;
        }
        public decimal p4
        {
            get;
            set;
        }

        public int ms
        {
            get;
            set;
        }
    }

    public class PriceAndMaxStock
    {
        public string m
        {
            get;
            set;
        }

        public List<PriceAndMaxStockModel> t
        {
            get;
            set;
        }
    }
}
