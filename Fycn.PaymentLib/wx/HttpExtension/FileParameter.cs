using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Fycn.PaymentLib.wx.HttpExtension
{
    public class FileParameter
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="name">参数的名称</param>
        /// <param name="fileStream">调用方会自动释放</param>
        /// <param name="filename"></param>
        /// <param name="contentType"></param>
        public FileParameter(string name, Stream fileStream, string filename, string contentType)
        {
            this.Writer = s =>
            {
                var buffer = new byte[1024];
                using (fileStream)
                {
                    int readCount;
                    while ((readCount = fileStream.Read(buffer, 0, buffer.Length)) != 0)
                    {
                        s.Write(buffer, 0, readCount);
                    }
                }
            };
            this.FileName = filename;
            this.ContentType = contentType;
            this.Name = name;
        }
        /// <summary>
        /// 参数名称
        /// </summary>
        public string Name;
        /// <summary>
        /// 读写操作流
        ///  返回的是写入的字节流长度
        /// </summary>
        public Action<Stream> Writer;
        /// <summary>
        /// 文件名称
        /// </summary>
        public string FileName;
        /// <summary>
        /// 文件类型
        /// </summary>
        public string ContentType;
    }
}
