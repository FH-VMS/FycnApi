using Fycn.Interface;
using Fycn.Model.Machine;
using Fycn.SqlDataAccess;
using Fycn.Utility;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fycn.Service
{
    public class CashEquipmentService : AbstractService, IBase<CashEquipmentModel>
    {
        public List<CashEquipmentModel> GetAll(CashEquipmentModel cashEquipmentInfo)
        {
            string userClientId = HttpContextHandler.GetHeaderObj("UserClientId").ToString();
            if (string.IsNullOrEmpty(userClientId))
            {
                return null;
            }
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

            conditions.AddRange(CreatePaginConditions(cashEquipmentInfo.PageIndex, cashEquipmentInfo.PageSize));

            return GenerateDal.LoadByConditions<CashEquipmentModel>(CommonSqlKey.GetCashEquipmentList, conditions);
        }


        public int GetCount(CashEquipmentModel cashEquipmentInfo)
        {
            var result = 0;

            string userClientId = HttpContextHandler.GetHeaderObj("UserClientId").ToString();
            if (string.IsNullOrEmpty(userClientId))
            {
                return 0;
            }
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
            result = GenerateDal.CountByConditions(CommonSqlKey.GetCashEquipmentCount, conditions);

            return result;
        }


        /// <summary>
        /// 新增/修改会员信息
        /// </summary>
        /// <param name="memberInfo"></param>
        /// <returns></returns>
        public int PostData(CashEquipmentModel cashEquipmentInfo)
        {
            int result;
            
            result = GenerateDal.Create(cashEquipmentInfo);
            

            return result;
        }

        /// <summary>
        /// 删除用户
        /// </summary>
        /// <returns></returns>
        public int DeleteData(string id)
        {
            CashEquipmentModel cashEquipmentInfo = new CashEquipmentModel();
            cashEquipmentInfo.Id = id;
            return GenerateDal.Delete<CashEquipmentModel>(CommonSqlKey.DeleteCashSale, cashEquipmentInfo);
         
            return 0;
        }

        public int UpdateData(CashEquipmentModel cashEquipmentInfo)
        {
            if(!IsExistEquipmentInfo(cashEquipmentInfo.MachineId))
            {
               return PostData(cashEquipmentInfo);
            }

            switch (cashEquipmentInfo.UpdateType)
            {
                case "cash_status":
                    return GenerateDal.Update(CommonSqlKey.UpdateCashStatus, cashEquipmentInfo);
                case "cash_stock":
                    return GenerateDal.Update(CommonSqlKey.UpdateCashStock, cashEquipmentInfo);
                case "coin_status":
                    return GenerateDal.Update(CommonSqlKey.UpdateCoinStatus, cashEquipmentInfo);
                case "coin_stock":
                    return GenerateDal.Update(CommonSqlKey.UpdateCoinStock, cashEquipmentInfo);

            }
            return 0;
        }

        private bool IsExistEquipmentInfo(string machineId)
        {
            int result = 0;
            var conditions = new List<Condition>();
            conditions.Add(new Condition
            {
                LeftBrace = " AND ",
                ParamName = "MachineId",
                DbColumnName = "machine_id",
                ParamValue = machineId,
                Operation = ConditionOperate.Equal,
                RightBrace = "",
                Logic = ""
            });
            result = GenerateDal.CountByConditions(CommonSqlKey.IsExistEquipmentInfo, conditions);

            return result>0;
        }
    }
}
