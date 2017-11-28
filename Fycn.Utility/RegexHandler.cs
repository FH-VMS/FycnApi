using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Fycn.Utility
{
    public static class RegexHandler
    {
        /// <summary>
        /// 匹配获取列表
        /// </summary>
        /// <param name="sourceTxt"></param>
        /// <param name="marchTxt"></param>
        /// <returns></returns>
        public static List<String> GetAllMatchList(string sourceTxt, string marchTxt)
        {
            var r = new Regex(marchTxt, RegexOptions.IgnoreCase | RegexOptions.Multiline);
            var march = r.Matches(sourceTxt ?? "");
            return (from object m in march select m.ToString()).ToList();
        }
    }
}
