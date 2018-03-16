using System;
using System.Collections.Generic;
using System.Text;

namespace Fycn.PaymentLib.wx.HttpExtension
{
    public class FormParameter
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public FormParameter(string name, object value)
        {
            Name = name;
            Value = value;
            //Type = type;
            //Domain = string.Empty;
        }



        /// <summary>
        /// 参数名称
        /// </summary>
        public string Name;
        /// <summary>
        /// 参数值
        /// </summary>
        public object Value;

        ///// <summary>
        /////  cookie的域名   -- cookie 类型时需要
        ///// </summary>
        //public string Domain;

        /// <summary>
        /// 重写ToString返回   name=value编码后的格式
        /// </summary>
        /// <returns>String</returns>
        public override string ToString()
        {
            return $"{Name}={Value}";
        }
    }
}
