using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fycn.Utility
{
    public class ByteHelper
    {
        //http://www.cnblogs.com/zjbtony/archive/2012/04/13/2445055.html
        //http://www.cnblogs.com/allen0118/p/3892028.html
        /// <summary>
        /// 十六进制转换成十进制
        /// </summary>
        /// <param name="hex"></param>
        /// <returns></returns>
        public static string Hex2Ten(string hex)
        {
            int ten = 0;
            for (int i = 0, j = hex.Length - 1; i < hex.Length; i++)
            {
                ten += HexChar2Value(hex.Substring(i, 1)) * ((int)Math.Pow(16, j));
                j--;
            }
            return ten.ToString();
        }

        public static int HexChar2Value(string hexChar)
        {
            switch (hexChar)
            {
                case "0":
                case "1":
                case "2":
                case "3":
                case "4":
                case "5":
                case "6":
                case "7":
                case "8":
                case "9":
                    return Convert.ToInt32(hexChar);
                case "a":
                case "A":
                    return 10;
                case "b":
                case "B":
                    return 11;
                case "c":
                case "C":
                    return 12;
                case "d":
                case "D":
                    return 13;
                case "e":
                case "E":
                    return 14;
                case "f":
                case "F":
                    return 15;
                default:
                    return 0;
            }
        }

        /// <summary>
        /// 字符串转16进制字节数组
        /// </summary>
        /// <param name="hexString"></param>
        /// <returns></returns>
        public static byte[] strToToHexByte(string hexString)
        {
            hexString = hexString.Replace(" ", "");
            if ((hexString.Length % 2) != 0)
                hexString += " ";
            byte[] returnBytes = new byte[hexString.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            return returnBytes;
        }

/// <summary>
        /// 字符串转10进制字节数组
        /// </summary>
        /// <param name="hexString"></param>
        /// <returns></returns>
        public static byte[] strToToTenByte(string hexString)
        {
            hexString = hexString.Replace(" ", "");
            if ((hexString.Length % 2) != 0)
                hexString += " ";
            byte[] returnBytes = new byte[hexString.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 10);
            return returnBytes;
        }



        /// <summary>
        /// 从十进制转换到十六进制
        /// </summary>
        /// <param name="ten"></param>
        /// <returns></returns>
        public static string Ten2Hex(string ten)
        {
            ulong tenValue = Convert.ToUInt64(ten);
            ulong divValue, resValue;
            string hex = "";
            do
            {
                //divValue = (ulong)Math.Floor(tenValue / 16);

                divValue = (ulong)Math.Floor((decimal)(tenValue / 16));

                resValue = tenValue % 16;
                hex = tenValue2Char(resValue) + hex;
                tenValue = divValue;
            }
            while (tenValue >= 16);
            if (tenValue != 0)
                hex = tenValue2Char(tenValue) + hex;
            return hex;
        }

        public static string tenValue2Char(ulong ten)
        {
            switch (ten)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                    return ten.ToString();
                case 10:
                    return "A";
                case 11:
                    return "B";
                case 12:
                    return "C";
                case 13:
                    return "D";
                case 14:
                    return "E";
                case 15:
                    return "F";
                default:
                    return "";
            }
        }


        //将十字符串转换成数组
        public static byte[] HexToArray(string info)
        {
            byte[] buff = new byte[info.Length / 2];

            int index = 0;
            for (int i = 0; i < info.Length; i += 2)
            {
                buff[index] = Convert.ToByte(info.Substring(i, 2));
                ++index;
            }

            return buff;
        }



        //把机器码转换成对应的实际数据
        public static string GenerateRealityData(byte[] source, string typeVal)
        {
            string finalResult = string.Empty;
            if (typeVal == "intval") //整形
            {
                /*
               uint result = 0;
               for(int i=0;i<source.Length;i++)
                {
                    result = Convert.ToUInt32(result + Convert.ToInt32(source[i]) * Math.Pow(255, i));
                }

                finalResult = result.ToString();
                 */
                string hexStr = byteToHexStr(source);
                finalResult = Hex2Ten(hexStr);
            }

            if (typeVal == "stringval")
            {
                char[] chars = source.Select(x => (char)x).ToArray();
                finalResult = new string(chars);
            }

            return finalResult;
        }

        /// <summary>
        /// 字节数组转16进制字符串
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string byteToHexStr(byte[] bytes)
        {
            string returnStr = "";
            if (bytes != null)
            {
                for (int i = 0; i < bytes.Length; i++)
                {
                    returnStr += bytes[i].ToString("X2");
                }
            }
            return returnStr;
        }

        public static DateTime BytesToDateTime(byte[] bytes, int offset)
        {
            if (bytes != null)
            {
                long ticks = BitConverter.ToInt64(bytes, offset);
                if (ticks < DateTime.MaxValue.Ticks && ticks > DateTime.MinValue.Ticks)
                {
                    DateTime dt = new DateTime(ticks);
                    return dt;
                }
            }
            return new DateTime();
        }



        public static byte[] DateTimeToBytes(DateTime dt)
        {
            return BitConverter.GetBytes(dt.Ticks);
        }

        public static byte[] StrToByte(string str)
        {
            return System.Text.Encoding.Default.GetBytes(str);
        }


        public static string[] SecretKey = { "12345678", "12345678", "12345678", "12345678", "12345678", "12345678", "12345678", "12345678" };

        //加密
        public static byte[] Encryption(int size, byte[] hex)
        {
            string nowSecretKey = SecretKey[size % 8];
            return hex;
        }

        //解密
        public static byte[] Deencryption(int size, byte[] hex)
        {
            string nowSecretKey = SecretKey[size % 8];
            return hex;
        }
    }
}
