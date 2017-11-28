

using System;
using System.Security.Cryptography;
using System.Text;

namespace Fycn.Utility
{
    public class Md5
    {
        public static string md5(string str, int code)  //code 16 或 32 
        {
            byte[] result = Encoding.Default.GetBytes(str);    //tbPass为输入密码的文本框
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] output = md5.ComputeHash(result);
            if (code == 16)
            {
                return BitConverter.ToString(output, 4, 8).Replace("-", "");
            }
            else
            {
                return BitConverter.ToString(output).Replace("-", "");
            }
            
        }
    }
    
}
