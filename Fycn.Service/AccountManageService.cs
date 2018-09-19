using Fycn.Interface;
using Fycn.Model.AccountSystem;
using Fycn.SqlDataAccess;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fycn.Service
{
    public class AccountManageService : AbstractService, IBase<AccountModel>
    {
        public List<AccountModel> GetAll(AccountModel accountManageInfo)
        {
           
            var result = new List<AccountModel>();
            var conditions = new List<Condition>();
           
            if (!string.IsNullOrEmpty(accountManageInfo.Name))
            {
                conditions.Add(new Condition
                {
                    LeftBrace = " AND ",
                    ParamName = "Name",
                    DbColumnName = "a.name",
                    ParamValue = "%" + accountManageInfo.Name + "%",
                    Operation = ConditionOperate.Like,
                    RightBrace = "",
                    Logic = ""
                });
            }

            conditions.AddRange(CreatePaginConditions(accountManageInfo.PageIndex, accountManageInfo.PageSize));


            result = GenerateDal.LoadByConditions<AccountModel>(CommonSqlKey.GetAccountManageList, conditions);






            return result;
        }


        public int GetCount(AccountModel accountManageInfo)
        {
            var result = 0;

            var conditions = new List<Condition>();
            if (!string.IsNullOrEmpty(accountManageInfo.Name))
            {
                conditions.Add(new Condition
                {
                    LeftBrace = " AND ",
                    ParamName = "Name",
                    DbColumnName = "name",
                    ParamValue = "%" + accountManageInfo.Name + "%",
                    Operation = ConditionOperate.Like,
                    RightBrace = "",
                    Logic = ""
                });
            }

            result = GenerateDal.CountByConditions(CommonSqlKey.GetAccountManageCount, conditions);

            return result;
        }


        /// <summary>
        /// 新增/修改会员信息
        /// </summary>
        /// <param name="memberInfo"></param>
        /// <returns></returns>
        public int PostData(AccountModel accountManageInfo)
        {
            accountManageInfo.Id = Guid.NewGuid().ToString();
        
            return GenerateDal.Create(accountManageInfo); 
        }

        /// <summary>
        /// 删除用户
        /// </summary>
        /// <returns></returns>
        public int DeleteData(string id)
        {
            AccountModel accountManageInfo = new AccountModel();
            accountManageInfo.Id = id;
            return GenerateDal.Delete<AccountModel>(CommonSqlKey.DeleteAccountManage, accountManageInfo);
        }

        public int UpdateData(AccountModel accountManageInfo)
        {
            return GenerateDal.Update(CommonSqlKey.UpdateAccountManage, accountManageInfo);
        }
    }
}
