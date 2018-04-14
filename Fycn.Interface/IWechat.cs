using Fycn.Model.Sys;
using Fycn.Model.Wechat;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fycn.Interface
{
    public interface IWechat
    {
        [Remark("新建会员", ParmsNote = "会员实体", ReturnNote = "int")]
        int CreateMember(WechatMemberModel memberInfo);
    }
}
