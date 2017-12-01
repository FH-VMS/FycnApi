using Fycn.Model.Sale;
using Fycn.Model.Sys;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Fycn.Interface
{
    public interface IStatistic
    {
        //取机器的销售额
         [Remark("取机器的销售额", ParmsNote = "", ReturnNote = "DataTable")]
        DataTable GetSalesAmountByMachine(string salesDateStart, string salesDateEnd, bool needPage, int pageIndex, int pageSize);

         [Remark("取机器的销售额总行数", ParmsNote = "", ReturnNote = "int")]
         int GetSalesAmountByMachineCount(string salesDateStart, string salesDateEnd, bool needPage, int pageIndex, int pageSize);

        [Remark("统计销售额根据销售日期", ParmsNote = "", ReturnNote = "DataTable")]
        DataTable GetStatisticSalesMoneyByDate(SaleModel saleInfo);
    }
}
