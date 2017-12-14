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
            byte[] byteSend = ByteHelper.strToToTenByte(message);
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

        public static string GenerateCommand(byte webCommandType, byte totalSize,byte socketCommand, List<CommandModel> lstCommandModel)
        {
            byte[] sendByte = new byte[totalSize+6];  //49+commandType + 48+size+chunk+content+EE
            sendByte[0] = 73;
            sendByte[1] = webCommandType;
            sendByte[2] = 72;
            sendByte[3] = totalSize;
            //sendByte[4] = chunk
            sendByte[5]= socketCommand;
            int i = 0;
            foreach(CommandModel cmdModel in lstCommandModel)
            {
                ByteHelper.strToAscii(cmdModel.Content).CopyTo(sendByte, 6 + i);
                i = i + cmdModel.Size;
            }
            sendByte[4] = GetChunk(sendByte.Skip(5).Take(totalSize).ToArray());
            sendByte[totalSize + 5] = 238;


            serverFullAddr = new IPEndPoint(serverIp, int.Parse(ConfigurationManager.AppSettings["SocketPort"]));//设置IP，端口
            sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //指定本地主机地址和端口号
            sock.Connect(serverFullAddr);
            try
            {
                //RedisHelper redisHelper = new RedisHelper(0);
                //redisHelper.StringSet("senddata", ByteHelper.byteToHexStr(sendByte));
                //发送数据
                sock.Send(sendByte);
                sock.Close();
            }
            catch (Exception ex)
            {
                sock.Close();
                return "";
            }
           

            return ByteHelper.byteToHexStr(sendByte.Skip(2).ToArray());

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
