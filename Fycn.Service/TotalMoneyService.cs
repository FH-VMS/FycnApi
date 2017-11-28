using Fycn.Interface;
using Fycn.Model.Withdraw;
using Fycn.SqlDataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fycn.Utility;

namespace Fycn.Service
{
    public class TotalMoneyService : AbstractService, IBase<TotalMoneyModel>
    {
        public List<TotalMoneyModel> GetAll(TotalMoneyModel totalMoneyInfo)
        {
            string userClientId = HttpContextHandler.GetHeaderObj("UserClientId").ToString();
            var userStatus = HttpContextHandler.GetHeaderObj("Sts").ToString();

            var dics = new Dictionary<string, object>();

            if (userStatus == "100" || userStatus == "99")
            {
                dics.Add("ClientId", "self");
            }
            else
            {
                dics.Add("ClientId", userClientId);
            }




            return GenerateDal.Load<TotalMoneyModel>(CommonSqlKey.GetTotalMoneyByClient, dics);
        }


        public int GetCount(TotalMoneyModel totalMoneyInfo)
        {
            return 0;
        }


        /// <summary>
        /// 新增/修改会员信息
        /// </summary>
        /// <param name="memberInfo"></param>
        /// <returns></returns>
        public int PostData(TotalMoneyModel totalMoneyInfo)
        {
            return 0;
        }

       

        /// <summary>
        /// 删除用户
        /// </summary>
        /// <returns></returns>
        public int DeleteData(string id)
        {
            return 0;
        }

        public int UpdateData(TotalMoneyModel totalMoneyInfo)
        {
            return 0;
        }
    }
}
