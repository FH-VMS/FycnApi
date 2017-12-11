using Fycn.Model.Socket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Fycn.Utility
{
    public class SocketHelper
    {
        private static IPAddress serverIp = IPAddress.Parse(ConfigurationManager.AppSettings["SocketIp"]);
        private static IPEndPoint serverFullAddr;//完整终端地址

        private static Socket sock;

        public static void SendMessage(string message)
        {
            serverFullAddr = new IPEndPoint(serverIp, int.Parse(ConfigurationManager.AppSettings["SocketPort"]));//设置IP，端口
            sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //指定本地主机地址和端口号
            sock.Connect(serverFullAddr);
            byte[] byteSend = strToToHexByte(message);
            try
            {
                //发送数据
                sock.Send(byteSend);

            }
            catch (Exception ex)
            {

            }
            sock.Close();
        }

        //将十字符串转换成数组
        /// <summary>
        /// 字符串转16进制字节数组
        /// </summary>
        /// <param name="hexString"></param>
        /// <returns></returns>
        private static byte[] strToToHexByte(string hexString)
        {
            hexString = hexString.Replace(" ", "");
            if ((hexString.Length % 2) != 0)
                hexString += " ";
            byte[] returnBytes = new byte[hexString.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            return returnBytes;
        }

        public static void SendStrMessageTest(string ip,string message)
        {
            serverFullAddr = new IPEndPoint(serverIp, int.Parse(ConfigurationManager.AppSettings["SocketPort"]));//设置IP，端口
            sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //指定本地主机地址和端口号
            sock.Connect(serverFullAddr);
            byte[] byteSend = System.Text.Encoding.Default.GetBytes(ip.Trim()+"~"+message.Trim());
            try
            {
                //发送数据
                sock.Send(byteSend);

            }
            catch (Exception ex)
            {

            }
            sock.Close();
        }

        public static void GenerateCommand(byte commandType, byte totalSize, List<CommandModel> lstCommandModel)
        {
            byte[] sendByte = new byte[totalSize+6];  //49+commandType + 48+size+chunk+content+EE
            sendByte[0] = 73;
            sendByte[1] = commandType;
            sendByte[2] = 72;
            sendByte[3] = totalSize;
            //sendByte[4] = chunk
            int i = 0;
            foreach(CommandModel cmdModel in lstCommandModel)
            {
                ByteHelper.HexToArray(cmdModel.Content).CopyTo(sendByte, 5 + i);
                i = i + cmdModel.Size;
            }
            sendByte[4] = GetChunk(sendByte.Skip(5).Take(totalSize).ToArray());
            sendByte[totalSize + 5] = 238;

        }

        private static byte GetChunk(byte[] chunkBytes)
        {
            byte resultChunk = new byte();
            for (int i = 0; i < chunkBytes.Length; i++)
            {
                resultChunk ^= chunkBytes[i];
            }
            return resultChunk;
        }
    }
}
