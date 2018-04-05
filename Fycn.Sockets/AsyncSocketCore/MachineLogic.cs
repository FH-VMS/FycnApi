using Fycn.Sockets.AsyncSocketCore;
using Fycn.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;

namespace Fycn.Sockets
{
    public class MachineLogic
    {
        private Dictionary<string, Timer> _dicTimer;
        protected Dictionary<string,Timer> dicTimers
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
        
        //处理机器消息
        public byte[] HandleHexByte(byte[] byteInfo, AsyncSocketUserToken m_asyncSocketUserToken, AsyncSocketServer m_asyncSocketServer)
        {
            //Program.Logger.InfoFormat("receive message is {0}, machine id is {1}", ByteHelper.byteToHexStr(byteInfo), m_asyncSocketUserToken.MachineId);
            //包头
            string infoHead = byteInfo[0].ToString();
            //保活
            if (infoHead == "57")
            {
                //Program.Logger.InfoFormat("machine id is {0}", m_asyncSocketUserToken.MachineId);
                if (string.IsNullOrEmpty(m_asyncSocketUserToken.MachineId) || !MachineHelper.IsOnline(m_asyncSocketUserToken.MachineId))
                {
                    return new byte[0];
                }
                return byteInfo;
            }
            
            if (infoHead=="72")
            {


                //大小
                int infoSize = int.Parse(ByteHelper.GenerateRealityData(byteInfo.Skip(1).Take(2).ToArray(), "intval"));
                
                byte[] deencryData = byteInfo;//解密算法
                //验证码
                string infoVerify = byteInfo[3].ToString();
                //数据
                byte[] data = ByteHelper.Deencryption(infoSize, byteInfo.Skip(4).Take(infoSize).ToArray());
                //验证是否为有效包
              
                if (!IsValidPackage(infoVerify, data))
                {
                    return new byte[0];
                }

                //不签到不回复
                string commandStr = ByteHelper.Ten2Hex(data[0].ToString()).ToUpper();
         
                if (commandStr != "41"&& commandStr != "30")
                {
                    if(string.IsNullOrEmpty(m_asyncSocketUserToken.MachineId))
                    {
                        return new byte[0];
                    }
                }
                
                try
                {
                    //处理机器上传指令的逻辑
                    return new MachinePush().PushLogic(commandStr, byteInfo, data, m_asyncSocketUserToken);
                }
                catch (Exception e)
                {
                    //MessageBox.Show(e.Message);
                    return new byte[0];
                }

            } else if(infoHead=="73") { //推送
                int sendLength = byteInfo.Length - 2;
                string ip = string.Empty;

                byte[] sendInfo = new byte[byteInfo.Length];
                byteInfo.CopyTo(sendInfo, 0);
                int size73 = int.Parse(ByteHelper.GenerateRealityData(byteInfo.Skip(3).Take(2).ToArray(),"intval"));
                ByteHelper.Deencryption(size73, byteInfo.Skip(6).Take(size73).ToArray()).CopyTo(byteInfo,6);
                string machineId10 = ByteHelper.GenerateRealityData(byteInfo.Skip(7).Take(12).ToArray(), "stringval");

                ip = MachineHelper.GetIp(machineId10);
                       
                if (sendLength > 0 && !string.IsNullOrEmpty(ip))
                {
                   
                    sendToTerminal(m_asyncSocketServer,machineId10,ip, sendInfo, sendLength, 3);
                    /*
                    SetTimeout(5000, delegate {
                        Program.Logger.InfoFormat("loop machineId is {0}, loop ip is {1}, message is {2}", machineId10, ip, ByteHelper.byteToHexStr(sendInfo));
                        sendToTerminal(m_asyncSocketServer,machineId10,ip, sendInfo, sendLength, 1);
                    }, machineId10, byteInfo);
                    */
                }
                
                //x[0].co
                return new byte[0];
            } else {
                try
                {
                    string ipAndMessage = ByteHelper.GenerateRealityData(byteInfo, "stringval");
                    string ip = ipAndMessage.Split("~")[0];
                    byte[] sendByte = ByteHelper.strToToHexByte(ipAndMessage.Split("~")[1]);
                    AsyncSocketUserToken[] list = null;
                    m_asyncSocketServer.AsyncSocketUserTokenList.CopyList(ref list);
                    for (int i = 0; i < list.Length; i++)
                    {
                        if (list[i].ConnectSocket.RemoteEndPoint.ToString() == ip)
                        {
                            list[i].SendEventArgs.SetBuffer(sendByte, 0, sendByte.Length);
                            bool willRaiseEvent = list[i].ConnectSocket.SendAsync(list[i].SendEventArgs);
                            break;
                        }
                    }
                }
                catch
                {
                    //Program.Logger.InfoFormat("the first by sent was {0}", byteInfo[0]);

                    return new byte[0];
                }
               
                    
                return new byte[0];
            }
        }

       

        //11个字节做异或处理
        private bool IsValidPackage(string infoVerify, byte[] data)
        {
            try
            {
                string finalResult = string.Empty;
                byte result = new byte();
                for (int i = 0; i < data.Length; i++)
                {
                    result ^= data[i];
                }


                return result.ToString() == infoVerify;
            }
            catch (Exception e)
            {
                
                return false;
            }

        }

        private void sendToTerminal(AsyncSocketServer m_asyncSocketServer,string machineId, string ip,byte[] byteInfo,int sendLength, int count)
        {
            AsyncSocketUserToken dicUserToken = SocketDictionary.Get(machineId);
            if (dicUserToken != null)
            {
                Program.Logger.InfoFormat("machine id was {0}", machineId);
                dicUserToken.SendEventArgs.SetBuffer(byteInfo.Skip(2).ToArray(), 0, sendLength);
                for (int j = 0; j < count; j++)
                {
                    bool willRaiseEvent = dicUserToken.ConnectSocket.SendAsync(dicUserToken.SendEventArgs);
                }
                return;
            }
            else
            {
                AsyncSocketUserToken[] list = null;
                m_asyncSocketServer.AsyncSocketUserTokenList.CopyList(ref list);
                for (int i = 0; i < list.Length; i++)
                {
                    if (list[i].MachineId == machineId)
                    {
                        
                        if (MachineHelper.IsOnline(machineId))
                        {
                            ip = MachineHelper.GetIp(machineId);
                        }
                        // Program.Logger.InfoFormat("loop ip is {0}", ip);
                        if (list[i].ConnectSocket.RemoteEndPoint.ToString() == ip)
                        {
                            list[i].SendEventArgs.SetBuffer(byteInfo.Skip(2).ToArray(), 0, sendLength);

                            for (int j = 0; j < count; j++)
                            {
                                bool willRaiseEvent = list[i].ConnectSocket.SendAsync(list[i].SendEventArgs);
                            }

                            break;
                        }

                    }
                }
            }
            
        }

          /// <summary> 
          /// 在指定时间过后执行指定的表达式 
          /// </summary> 
          /// <param name="interval">事件之间经过的时间（以毫秒为单位）</param> 
          /// <param name="action">要执行的表达式</param> 
         private void SetTimeout(double interval, Action action,string machineId, byte[] byteInfo) 
         {
            string key = ByteHelper.Ten2Hex(byteInfo[6].ToString());
            if (key == "42") //是否为出货指令
            {
                string outTradeNo = ByteHelper.GenerateRealityData(byteInfo.Skip(19).Take(22).ToArray(), "stringval");
             
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
