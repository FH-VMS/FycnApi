using Fycn.Interface;
using Fycn.Model.AccountSystem;
using Fycn.SqlDataAccess;
using Fycn.Utility;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fycn.Service
{
    public class TransferListService : AbstractService, IBase<TransferListModel>
    {
        public List<TransferListModel> GetAll(TransferListModel transferListInfo)
        {
            return null;
        }


        public int GetCount(TransferListModel transferListInfo)
        {
            return 0;
        }


        /// <summary>
        /// 新增/修改会员信息
        /// </summary>
        /// <param name="memberInfo"></param>
        /// <returns></returns>
        public int PostData(TransferListModel transferListInfo)
        {
            transferListInfo.TrasferDate = DateTime.Now;
            return GenerateDal.Create(transferListInfo);

            
        }

        /// <summary>
        /// 删除用户
        /// </summary>
        /// <returns></returns>
        public int DeleteData(string id)
        {


            return 0;


        }

        public int UpdateData(TransferListModel transferListInfo)
        {
            string userAccount = HttpContextHandler.GetHeaderObj("UserAccount").ToString();
            transferListInfo.Operator = userAccount;
            transferListInfo.TrasferDate = DateTime.Now;
            return GenerateDal.Update(CommonSqlKey.UpdateTransferList, transferListInfo);
        }
    }
}
