using Fycn.Interface;
using Fycn.Model.Sale;
using Fycn.SqlDataAccess;
using Fycn.Utility;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fycn.Service
{
    public class CashSaleService : AbstractService, IBase<CashSaleModel>
    {
        public List<CashSaleModel> GetAll(CashSaleModel cashSaleInfo)
        {
            string userClientId = HttpContextHandler.GetHeaderObj("UserClientId").ToString();

            var conditions = new List<Condition>();

            conditions.Add(new Condition
            {
                LeftBrace = "",
                ParamName = "ClientId",
                DbColumnName = "",
                ParamValue = userClientId,
                Operation = ConditionOperate.None,
                RightBrace = "",
                Logic = ""
            });

            conditions.AddRange(CreatePaginConditions(cashSaleInfo.PageIndex, cashSaleInfo.PageSize));

            return GenerateDal.LoadByConditions<CashSaleModel>(CommonSqlKey.GetCashSaleList, conditions);
        }


        public int GetCount(CashSaleModel cashSaleInfo)
        {
            var result = 0;

            string userClientId = HttpContextHandler.GetHeaderObj("UserClientId").ToString();

            var conditions = new List<Condition>();
            conditions.Add(new Condition
            {
                LeftBrace = "",
                ParamName = "ClientId",
                DbColumnName = "",
                ParamValue = userClientId,
                Operation = ConditionOperate.None,
                RightBrace = "",
                Logic = ""
            });



            result = GenerateDal.CountByConditions(CommonSqlKey.GetCashSaleCount, conditions);

            return result;
        }


        /// <summary>
        /// 新增/修改会员信息
        /// </summary>
        /// <param name="memberInfo"></param>
        /// <returns></returns>
        public int PostData(CashSaleModel cashSaleInfo)
        {
            int result;
            result = GenerateDal.Create(cashSaleInfo);
            
            return result;
        }

        /// <summary>
        /// 删除用户
        /// </summary>
        /// <returns></returns>
        public int DeleteData(string id)
        {
            return 0;
        }

        public int UpdateData(CashSaleModel cashSaleInfo)
        {
            //操作日志
            return 0;
        }
    }
}
