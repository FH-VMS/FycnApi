using Fycn.Model.Sys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fycn.Interface
{
    [Remark("取数据列表的公用接口")]
    public interface IBase<T>
    {
        [Remark("取数据列表通用接口", ParmsNote = "对应实体", ReturnNote = "返回返页列表")]
        List<T> GetAll(T tmodel);

        [Remark("通用添加接口", ParmsNote = "对应实体", ReturnNote = "0 or 1")]
        int PostData(T tmodel);

        [Remark("通用获取分页行数", ParmsNote = "对应实体", ReturnNote = "总行数")]
        int GetCount(T tmodel);


        [Remark("通用更新数据", ParmsNote = "对应实体", ReturnNote = "0 or 1")]
        int UpdateData(T tmodel);

        [Remark("通用删除接口", ParmsNote = "string", ReturnNote = "0 or 1")]
        int DeleteData(string id);

        
    }
}
