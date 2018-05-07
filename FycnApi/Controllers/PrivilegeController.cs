using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using FycnApi.Base;
using Fycn.Model.Privilege;
using Fycn.Interface;
using Fycn.Service;
using Fycn.Model.Sys;

namespace FycnApi.Controllers
{
    public class PrivilegeController : ApiBaseController
    {
        private static IBase<PrivilegeModel> _IBase
        {
            get
            {
                return new PrivilegeService();
            }
        }

        public ResultObj<List<PrivilegeModel>> GetData(string privilegeName = "", string principleType = "", string clientName = "",int pageIndex = 1, int pageSize = 10)
        {
            // IProduct service = new ProductService();
            //List<ProductModel> products = service.GetAllProducts();

            PrivilegeModel privilegeInfo = new PrivilegeModel();
            privilegeInfo.PrivilegeName = privilegeName;
            privilegeInfo.PrincipleType = principleType;
            privilegeInfo.ClientName = clientName;
            privilegeInfo.PageIndex = pageIndex;
            privilegeInfo.PageSize = pageSize;
            var users = _IBase.GetAll(privilegeInfo);
            int totalcount = _IBase.GetCount(privilegeInfo);

            var pagination = new Pagination { PageSize = pageSize, PageIndex = pageIndex, StartIndex = 0, TotalRows = totalcount, TotalPage = 0 };
            return Content(users, pagination);
        }

        public ResultObj<int> PostData([FromBody]PrivilegeModel privilegeInfo)
        {
           
            return Content(_IBase.PostData(privilegeInfo));
            

        }

        public ResultObj<int> PutData([FromBody]PrivilegeModel privilegeInfo)
        {
            return Content(_IBase.UpdateData(privilegeInfo));
        }

        public ResultObj<int> DeleteData(string idList)
        {
            return Content(_IBase.DeleteData(idList));
        }
    }
}