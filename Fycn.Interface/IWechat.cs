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
        int CreateMember(WechatMemberModel memberInfo, ClientMemberRelationModel clientMemberInfo);

        [Remark("判断该客户是否存在该会员", ParmsNote = "会员实体", ReturnNote = "int")]
        List<WechatMemberModel> IsExistMember(WechatMemberModel clientMemberInfo);

        [Remark("插入客户与会员关系表", ParmsNote = "会员实体", ReturnNote = "int")]
        int CreateClientAndMemberRelation(ClientMemberRelationModel clientMemberInfo);
    }
}
