using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace Fycn.PaymentLib.wx.HttpExtension
{
    public class FyHttpRequest
    {
        public FyHttpRequest()
        {
            FormParameters = new List<FormParameter>();
            FileParameters = new List<FileParameter>();
        }

        /// <summary>
        /// 请求地址信息
        /// </summary>
        public Uri Uri { get; set; }

        /// <summary>
        ///  如果此值设置，则忽略 Uri 值
        /// </summary>
        public string AddressUrl { get; set; }

        /// <summary>
        /// 请求方式
        /// </summary>
        public HttpMethod HttpMethod { get; set; } = HttpMethod.Get;

        ///// <summary>
        /////  是否允许自动重定向
        ///// 默认为true  最大可重定向次数为 5
        ///// </summary>
        //public bool AllowAutoRedirect { get; set; } = true;

        ///// <summary>
        /////   是否使用压缩
        ///// </summary>
        //public bool AutoDecompres { get; set; }

        ///// <summary>
        /////   是否使用cookie 
        ///// </summary>
        //public bool UseCookies { get; set; } = false;

        ///// <summary>
        /////  是否使用代理
        ///// </summary>
        //public bool UseProxy { get; set; } = true;

        /// <summary>
        ///   reqMessage 设置方法
        ///    如果当前的设置不能满足需求，可以通过这里再次设置
        /// </summary>
        public Action<HttpRequestMessage> RequestSet { get; set; }

        #region   请求的内容参数

        /// <summary>
        /// 文件参数列表
        /// </summary>
        public List<FileParameter> FileParameters { get; set; }

        /// <summary>
        /// 是否存在文件
        /// </summary>
        public bool HasFile => FileParameters.Any();


        /// <summary>
        /// 非文件参数列表
        /// </summary>
        public List<FormParameter> FormParameters { get; set; }

        #endregion

        /// <summary>
        /// 自定义内容实体
        /// eg:当上传文件时，无法自定义内容
        /// </summary>
        public string CustomBody { get; set; }

        /// <summary>
        ///    超时时间（毫秒）
        /// </summary>
        public int TimeOutMilSeconds { get; set; }
    }
}
