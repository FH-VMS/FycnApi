using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fycn.Utility
{
    public interface ICacheStrategy
    {
        /// <summary>
        /// 添加指定ID的对象
        /// </summary>
        /// <param name="key"></param>
        /// <param name="o"></param>
        void AddObject(string key, object o);

        /// <summary>
        /// 添加指定ID的对象(关联指定文件组)
        /// </summary>
        /// <param name="key"></param>
        /// <param name="o"></param>
        /// <param name="files"></param>
        void AddObjectWithFileChange(string key, object o, string[] files);

        /// <summary>
        /// 添加指定ID的对象(关联指定键值组)
        /// </summary>
        /// <param name="key"></param>
        /// <param name="o"></param>
        /// <param name="dependKey"></param>
        void AddObjectWithDepend(string key, object o, string[] dependKey);

        /// <summary>
        /// 移除指定ID的对象
        /// </summary>
        /// <param name="key"></param>
        void RemoveObject(string key);

        /// <summary>
        /// 返回指定ID的对象
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        object RetrieveObject(string key);

        /// <summary>
        /// Check the value exist with the key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        bool CheckExistValueByKey(string key);
        

        /// <summary>
        /// 持续时间
        /// </summary>
        TimeSpan TimeOutSpan { get; set; }

        /// <summary>
        /// 到期时间
        /// </summary>
        DateTime AbsoluteTime { get; set; }
    }
}
