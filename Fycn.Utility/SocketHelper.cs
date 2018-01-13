using Fycn.Model.Socket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Timers;

namespace Fycn.Utility
{
    public class SocketHelper
    {
        private static Dictionary<string, Timer> _dicTimer;
        protected static Dictionary<string, Timer> dicTimers
        {
            get
            {
                if (_dicTimer == null)
                {
                    _dicTimer = new Dictionary<string, Timer>();
                }
                return _dicTimer;
            }
            set
            {
                _dicTimer = value;
            }
        }

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
    
        //49(十六进制)开头为后端向socket的标识头
        //webCommandType标识指令的意思(十进制) 10:通知出货 11:一键补货 12:按货道补货 13:修改最大库存
        //totalSize表示需要加密的内容  去除49+webCommandType 48 size(两个字节) chunksum验证 和结尾 EE
        //socketCommand表示向机器下推的指令标识
        public static string GenerateCommand(byte webCommandType, int totalSize,byte socketCommand, List<CommandModel> lstCommandModel)
        {
            byte[] sendByte = new byte[totalSize+7];  //49+commandType + 48+size+chunk+content+EE
            sendByte[0] = 73;
            sendByte[1] = webCommandType;//
            sendByte[2] = 72;
            ByteHelper.IntToTwoByte(totalSize).CopyTo(sendByte, 3); //size
            //sendByte[4] = chunk
            sendByte[6]= socketCommand;
            int i = 0;
            foreach(CommandModel cmdModel in lstCommandModel)
            {
                //byte[] transByte = ByteHelper.strToAscii(cmdModel.Content);
                ByteHelper.strToAscii(cmdModel.Content).CopyTo(sendByte, 7 + i);
                i = i + cmdModel.Size;
            }
            sendByte[5] = GetChunk(sendByte.Skip(6).Take(totalSize).ToArray());
            sendByte[totalSize + 6] = 238;

            string machineId = ByteHelper.GenerateRealityData(sendByte.Skip(7).Take(12).ToArray(), "stringval");
            string outTradeNo = string.Empty;
            string key = ByteHelper.Ten2Hex(socketCommand.ToString());
            if (key == "42") //出货结果通知指令
            {
                outTradeNo = ByteHelper.GenerateRealityData(sendByte.Skip(19).Take(22).ToArray(), "stringval");
            } 
            // 发送前加密
            ByteHelper.Encryption(totalSize, sendByte.Skip(6).ToArray()).CopyTo(sendByte, 6);

            serverFullAddr = new IPEndPoint(serverIp, int.Parse(ConfigurationManager.AppSettings["SocketPort"]));//设置IP，端口

            socketSend(sendByte);
            SetTimeout(5000, delegate {
                socketSend(sendByte);
            },outTradeNo,key, machineId);
            
            if(key == "42") //出货结果通知指令
            {
                try
                {
                    MachineHelper.CacheOrder(outTradeNo, ByteHelper.byteToHexStr(sendByte.Skip(2).ToArray()));
                }
                catch(Exception ex)
                {
                    MachineHelper.CacheOrder(outTradeNo, ByteHelper.byteToHexStr(sendByte.Skip(2).ToArray()));
                }
                
            } 
            else 
            {
                try
                {
                    MachineHelper.CachePush(machineId, key, ByteHelper.byteToHexStr(sendByte.Skip(2).ToArray()));
                }
                catch (Exception ex)
                {
                    MachineHelper.CachePush(machineId, key, ByteHelper.byteToHexStr(sendByte.Skip(2).ToArray()));
                }

            }
            

            return "";

        }

        private static void socketSend(byte[] byteInfo)
        {
            sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //指定本地主机地址和端口号
            sock.Connect(serverFullAddr);
            try
            {
                //RedisHelper redisHelper = new RedisHelper(0);
                //redisHelper.StringSet("senddata", ByteHelper.byteToHexStr(sendByte));
                //发送数据
                sock.Send(byteInfo);
                sock.Close();
            }
            catch (Exception ex)
            {
                sock.Close();
            }
        }

        //生成chunk验证
        private static byte GetChunk(byte[] chunkBytes)
        {
            byte resultChunk = new byte();
            for (int i = 0; i < chunkBytes.Length; i++)
            {
                resultChunk ^= chunkBytes[i];
            }
            return resultChunk;
        }

        //定时轮询
        /// <summary> 
        /// 在指定时间过后执行指定的表达式 
        /// </summary> 
        /// <param name="interval">事件之间经过的时间（以毫秒为单位）</param> 
        /// <param name="action">要执行的表达式</param> 
        private static void SetTimeout(double interval, Action action, string outTradeNo, string key,string machineId)
        {
            if (key == "42") //是否为出货指令
            {
                if (!dicTimers.ContainsKey(outTradeNo))
                {
                    dicTimers.Add(outTradeNo, new Timer(interval));
                    dicTimers[outTradeNo].Elapsed += delegate (object sender, System.Timers.ElapsedEventArgs e)
                    {
                        if (MachineHelper.IsLegalOrder(outTradeNo))
                        {
                            action();
                        }
                        else
                        {

                            dicTimers[outTradeNo].Enabled = false;
                            dicTimers.Remove(outTradeNo);
                        }
                    };
                }
                dicTimers[outTradeNo].Enabled = true;
                dicTimers[outTradeNo].AutoReset = true;
            }
            else //非出货指令发两次
            {
                Timer timer = new Timer(interval);
                timer.Elapsed += delegate (object sender, System.Timers.ElapsedEventArgs e)
                {
                    
                    if (!MachineHelper.IsExistPush(machineId, key))// 判断该指令是否存在
                    {
                        timer.Enabled = false;
                    }
                    else
                    {
                        timer.Enabled = false;
                        action();
                    }
                };
            }

        }

    }
}
