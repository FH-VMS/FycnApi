using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Web;

namespace Fycn.Utility
{
    public class FileHandler
    {
        // <summary>  
        /// 写文件  
        /// </summary>  
        /// <param name="Path">文件路径</param>  
        /// <param name="Name">文件名(包括后缀名)</param>  
        /// <param name="content">内容</param>  
        /// <returns></returns>  
        public static bool WriteFile(string Path, string Name, string content)
        {
            try
            {
                string pathVal = Directory.GetCurrentDirectory() +"/" + Path;
                if (!Directory.Exists(pathVal))
                {
                    Directory.CreateDirectory(pathVal);
                }
                string fname = pathVal + Name; ;
                if (!File.Exists(fname))
                {
                    FileStream fs = File.Create(fname);
                    fs.Close();
                }
                StreamWriter sw = new StreamWriter(fname, false, System.Text.Encoding.GetEncoding("utf-8"));
                sw.WriteLine(content);
                sw.Close();
                sw.Dispose();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>  
        /// 读文件  
        /// </summary>  
        /// <param name="path">文件路径</param>  
        /// <returns></returns>  
        public static string ReadFile(string Path)
        {
            try
            {
                StreamReader sr = new StreamReader(Directory.GetCurrentDirectory() + "/" + Path, System.Text.Encoding.GetEncoding("utf-8"));
                string content = sr.ReadToEnd().ToString();
                sr.Close();
                return content;
            }
            catch
            {
                return string.Empty;

            }
        }

        public static void DeleteFile(string path)
        {
            try
            {
                string finalPath = Directory.GetCurrentDirectory() + "/" + path;
                File.Delete(finalPath);
            }
            catch
            {
               

            }
        }


        /// <summary>  
        /// 字符串压缩  
        /// </summary>  
        /// <param name="strSource"></param>  
        /// <returns></returns>  
        public static byte[] Compress(byte[] data)
        {
            try
            {
                MemoryStream ms = new MemoryStream();
                GZipStream zip = new GZipStream(ms, CompressionMode.Compress, true);
                zip.Write(data, 0, data.Length);
                zip.Close();
                byte[] buffer = new byte[ms.Length];
                ms.Position = 0;
                ms.Read(buffer, 0, buffer.Length);
                ms.Close();
                return buffer;

            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        /// <summary>  
        /// 字符串解压缩  
        /// </summary>  
        /// <param name="strSource"></param>  
        /// <returns></returns>  
        public static byte[] Decompress(byte[] data)
        {
            try
            {
                MemoryStream ms = new MemoryStream(data);
                GZipStream zip = new GZipStream(ms, CompressionMode.Decompress, true);
                MemoryStream msreader = new MemoryStream();
                byte[] buffer = new byte[0x1000];
                while (true)
                {
                    int reader = zip.Read(buffer, 0, buffer.Length);
                    if (reader <= 0)
                    {
                        break;
                    }
                    msreader.Write(buffer, 0, reader);
                }
                zip.Close();
                ms.Close();
                msreader.Position = 0;
                buffer = msreader.ToArray();
                msreader.Close();
                return buffer;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public static string CompressString(string str)
        {
            string compressString = "";
            byte[] compressBeforeByte = Encoding.GetEncoding("UTF-8").GetBytes(str);
            byte[] compressAfterByte = Compress(compressBeforeByte);
            //compressString = Encoding.GetEncoding("UTF-8").GetString(compressAfterByte);    
            compressString = Convert.ToBase64String(compressAfterByte);
            return compressString;
        }

        public static string DecompressString(string str)
        {
            string compressString = "";
            //byte[] compressBeforeByte = Encoding.GetEncoding("UTF-8").GetBytes(str);    
            byte[] compressBeforeByte = Convert.FromBase64String(str);
            byte[] compressAfterByte = Decompress(compressBeforeByte);
            compressString = Encoding.GetEncoding("UTF-8").GetString(compressAfterByte);
            return compressString;
        }

        public static void LogMachineData(string data)
        {
            File.AppendAllText("/root/123.txt", data);
        }
    }
}
