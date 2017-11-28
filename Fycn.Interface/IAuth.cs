using Fycn.Model.Common;
using Fycn.Model.Sys;
using Fycn.Model.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fycn.Interface
{
    public interface IAuth
    {
        [Remark("添加权限模板", ParmsNote = "模板名称，权限对应关系", ReturnNote = "0 或 1")]
        int PostAuthTemplate(string name, int rank, List<MenuModel> lstAuthModel);

        [Remark("根据模板取权限对应表", ParmsNote = "模板id", ReturnNote = "模板权限对应List")]
        List<MenuModel> GetAuthRelateByDmsId(string id);

        [Remark("更新权限模板", ParmsNote = "模板id,模板名称,用户等级，权限对应关系实体", ReturnNote = "0 or 1")]
        int UpdaetAuthTemplate(string id, string name, int rank, List<MenuModel> lstAuthModel);

         [Remark("取权限模板做字典", ParmsNote = "", ReturnNote = "实体列表")]
        List<AuthModel> GetAuthDic();

    }
}
