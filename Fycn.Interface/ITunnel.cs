using Fycn.Model.Machine;
using Fycn.Model.Sys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fycn.Interface
{
    public interface ITunnel
    {
        [Remark("取货道模板", ParmsNote = "对应实体", ReturnNote = "返回返页列表")]
        List<TunnelConfigModel> GetAll(TunnelConfigModel tmodel);

        [Remark("通用添加货道接口", ParmsNote = "对应实体", ReturnNote = "0 or 1")]
        int PostData(List<TunnelConfigModel> tmodel);


        [Remark("通用更新数据", ParmsNote = "对应实体", ReturnNote = "0 or 1")]
        int UpdateData(TunnelConfigModel tmodel);

        [Remark("通用删除接口", ParmsNote = "string", ReturnNote = "0 or 1")]
        int DeleteData(string id);
    }
}
