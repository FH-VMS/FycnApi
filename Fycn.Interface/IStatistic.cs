using Fycn.Model.Sale;
using Fycn.Model.Statistic;
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

        [Remark("统计支付笔数", ParmsNote = "", ReturnNote = "列表实体")]
        List<ClassModel> GetPayNumbers();

        [Remark("根据时间统计支付笔数", ParmsNote = "", ReturnNote = "列表实体")]
        List<ClassModel> GetPayNumbersByDate(string salesDateStart, string salesDateEnd, string type);

        [Remark("根据时间分类取销售额", ParmsNote = "", ReturnNote = "列表实体")]
        List<ClassModel> GetGroupSalesMoney(string salesDateStart, string salesDateEnd, string type);

        [Remark("根据时间分类商品", ParmsNote = "", ReturnNote = "列表实体")]
        List<ClassModel> GetGroupProduct(string salesDateStart, string salesDateEnd, bool needPage, int pageIndex, int pageSize);

        [Remark("根据机器分类销售额", ParmsNote = "", ReturnNote = "列表实体")]
        List<ClassModel> GetGroupMoneyByMachine(string salesDateStart, string salesDateEnd, bool needPage = true, int pageIndex = 1, int pageSize = 10);
    }
}
