using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Fycn.Interface
{
    public interface IDistrubuteMoney
    {
        Task<int> PostMoneyAsync(string tradeNo);
        int PostMoney(string tradeNo);
    }
}
