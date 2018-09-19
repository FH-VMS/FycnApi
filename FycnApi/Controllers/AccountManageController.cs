using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fycn.Interface;
using Fycn.Model.AccountSystem;
using Fycn.Model.Sys;
using Fycn.Service;
using FycnApi.Base;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FycnApi.Controllers
{
    public class AccountManageController :  ApiBaseController
    {
        private static IBase<AccountModel> _IBase
        {
            get
            {
                return new AccountManageService();
            }
        }

        public ResultObj<List<AccountModel>> GetData(string name = "", int pageIndex = 1, int pageSize = 10)
        {
            // IProduct service = new ProductService();
            //List<ProductModel> products = service.GetAllProducts();

            AccountModel accountInfo = new AccountModel();
            accountInfo.Name = name;
            accountInfo.PageIndex = pageIndex;
            accountInfo.PageSize = pageSize;
            var users = _IBase.GetAll(accountInfo);
            int totalcount = _IBase.GetCount(accountInfo);

            var pagination = new Pagination { PageSize = pageSize, PageIndex = pageIndex, StartIndex = 0, TotalRows = totalcount, TotalPage = 0 };
            return Content(users, pagination);
        }

        public ResultObj<int> PostData([FromBody]AccountModel accountInfo)
        {
            return Content(_IBase.PostData(accountInfo));
        }

        public ResultObj<int> PutData([FromBody]AccountModel accountInfo)
        {
            return Content(_IBase.UpdateData(accountInfo));
        }

        public ResultObj<int> DeleteData(string idList)
        {
            return Content(_IBase.DeleteData(idList));
        }
    }
}