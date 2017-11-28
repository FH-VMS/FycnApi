using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fycn.Model.Sys
{
    public class ModelTableInfo
    {
        /// <summary>
        /// 表名
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// 主键名列表，“，”分割主键组合，“|”分割组合中的值
        /// </summary>

        public List<string> KeysName { get; set; }

        /// <summary>
        /// 对象全部属性
        /// </summary>
        public List<ModelPropertyInfo> AllPropertyInfos { get; set; } 
    }
}
