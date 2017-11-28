using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fycn.Model.Sys
{
    public class Pagination
    {
        /// <summary>
        /// 总页数
        /// </summary>
        public int TotalPage { get; set; }
        /// <summary>
        /// 总记录数
        /// </summary>
        public int TotalRows { get; set; }
        /// <summary>
        /// 当前页
        /// </summary>
        public int PageIndex { get; set; }
        /// <summary>
        /// 每页记录数,默认10
        /// </summary>
        public int PageSize { get; set; }
        /// <summary>
        /// 开始记录数（limit）
        /// </summary>
        public int StartIndex { get; set; }
    }
}
