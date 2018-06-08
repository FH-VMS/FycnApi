using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using FycnApi.Base;
using Fycn.Model.Wechat;
using Fycn.Interface;
using Fycn.Service;
using Fycn.Model.Sys;
using Fycn.Model.Privilege;

namespace FycnApi.Controllers
{
    public class MemberController : ApiBaseController
    {
        private static IBase<WechatMemberModel> _IBase
        {
            get
            {
                return new MemberService();
            }
        }

        public ResultObj<List<WechatMemberModel>> GetData(string nickName = "", int pageIndex = 1, int pageSize = 10)
        {
            // IProduct service = new ProductService();
            //List<ProductModel> products = service.GetAllProducts();

            WechatMemberModel memberInfo = new WechatMemberModel();
            memberInfo.NickName = nickName;
            memberInfo.PageIndex = pageIndex;
            memberInfo.PageSize = pageSize;
            var users = _IBase.GetAll(memberInfo);
            int totalcount = _IBase.GetCount(memberInfo);

            var pagination = new Pagination { PageSize = pageSize, PageIndex = pageIndex, StartIndex = 0, TotalRows = totalcount, TotalPage = 0 };
            return Content(users, pagination);
        }

        public ResultObj<int> GivePrivilegeTicket(PrivilegeMemberRelationModel privilegeMemberInfo)
        {
            IMember _imember = new MemberService();
            return Content(_imember.GivePrivilegeTicket(privilegeMemberInfo));
        }

        public ResultObj<List<PrivilegeMemberRelationModel>> GetPrivilegeByMemberId(string memberId = "", int pageIndex = 1, int pageSize = 10)
        {
            PrivilegeMemberRelationModel privilegeMemberInfo = new PrivilegeMemberRelationModel();
            privilegeMemberInfo.MemberId = memberId;
            privilegeMemberInfo.PageIndex = pageIndex;
            privilegeMemberInfo.PageSize = pageSize;

            IWechat iwechat = new WechatService();
            int totalcount = iwechat.GetPrivilegeCountByMemberId(privilegeMemberInfo);

            var pagination = new Pagination { PageSize = pageSize, PageIndex = pageIndex, StartIndex = 0, TotalRows = totalcount, TotalPage = 0 };
            return Content(iwechat.GetPrivilegeByMemberId(privilegeMemberInfo), pagination);
        }
    }
}