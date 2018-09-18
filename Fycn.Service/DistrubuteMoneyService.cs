using Fycn.Interface;
using Fycn.Model.Sale;
using Fycn.PaymentLib;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Fycn.Service
{
    public class DistrubuteMoneyService
    {
        public void PostMoney(SaleModel saleInfo)
        {
            int position = Array.IndexOf(PathConfig.DistrubuteAccounts, saleInfo.MerchantId);
            if (position!=-1)
            {

            }
        }
    }
}
