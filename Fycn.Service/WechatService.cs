using Fycn.Interface;
using Fycn.Model.Wechat;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fycn.Service
{
    public class WechatService : AbstractService, IWechat
    {
        public int CreateMember(WechatMemberModel memberInfo)
        {
            int result;
            memberInfo.CreateDate = DateTime.Now;
            result = GenerateDal.Create(memberInfo);

            return result;
        }
    }
}
