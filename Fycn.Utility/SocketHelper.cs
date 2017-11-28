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
        private static IPAddress serverIp = IPAddress.Parse("192.168.20.190");
        private static IPEndPoint serverFullAddr;//完整终端地址

        private static Socket sock;

        public static void SendMessage(string message)
        {
            serverFullAddr = new IPEndPoint(serverIp, 9999);//设置IP，端口
            sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //指定本地主机地址和端口号
            sock.Connect(serverFullAddr);
            byte[] byteSend = System.Text.Encoding.Default.GetBytes("A2 88 32");
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
    }
}
