using Fycn.Model.Privilege;
using Fycn.Model.Sys;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fycn.Interface
{
    public interface IMember
    {
        [Remark("优惠券发至会员账户", ParmsNote = "", ReturnNote = "int")]
        int GivePrivilegeTicket(PrivilegeMemberRelationModel privilegeMemberInfo);
    }
}
