using Fycn.Sockets.AsyncSocketCore;
using Fycn.Utility;
using System;
using System.Linq;

namespace Fycn.Sockets
{
    public class MachineLogic
    {
        //处理机器消息
        public byte[] HandleHexByte(byte[] byteInfo, AsyncSocketUserToken m_asyncSocketUserToken, AsyncSocketServer m_asyncSocketServer)
        {
            Program.Logger.InfoFormat("the message is {0}", ByteHelper.byteToHexStr(byteInfo));
            //包头
            string infoHead = byteInfo[0].ToString();
            //保活
            if (infoHead == "57")
            {
                if (string.IsNullOrEmpty(m_asyncSocketUserToken.MachineId))
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
                /*
                try
                {
                   
                    if (MachineHelper.IsOnline(machineId10)) // 若redis里没有 则去库里查询
                    {
                        RedisHelper redisHelper = new RedisHelper(0);
                        ip = redisHelper.StringGet(machineId10);
                    }
                    else
                    {
                       
                        IMachine imachine = new MachineService();
                        DataTable dt = imachine.GetIpByMachineId(machineId10);
                        if (dt.Rows.Count > 0)
                        {
                            ip = dt.Rows[0]["ip_v4"].ToString().Split("-")[0];
                            MachineHelper.Signature(machineId10, ip);
                        }
                       
                    }
                }
                catch
                {
                   
                    IMachine imachine = new MachineService();
                    DataTable dt = imachine.GetIpByMachineId(machineId10);
                    if (dt.Rows.Count > 0)
                    {
                        ip = dt.Rows[0]["ip_v4"].ToString().Split("-")[0];
                        MachineHelper.Signature(machineId10, ip);
                    }
                  
                }
                 */
                ip = MachineHelper.GetIp(machineId10);
                       
                if (sendLength > 0 && !string.IsNullOrEmpty(ip))
                {
                   
                    sendToTerminal(m_asyncSocketServer,machineId10,ip, sendInfo, sendLength, 3);
                    SetTimeout(5000, delegate {
                        sendToTerminal(m_asyncSocketServer,machineId10,ip, sendInfo, sendLength, 1);
                    }, machineId10, byteInfo);
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
                    Program.Logger.InfoFormat("the first by sent was {0}", byteInfo[0]);

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
                    AsyncSocketUserToken[] list = null;
                    m_asyncSocketServer.AsyncSocketUserTokenList.CopyList(ref list);
                    for (int i = 0; i < list.Length; i++)
                    {
                        if (list[i].MachineId == machineId)
                        {
                            RedisHelper redis0=new RedisHelper(0);
                            if(redis0.KeyExists(machineId))
                            {
                                ip = redis0.StringGet(machineId);
                            }
                            // Program.Logger.InfoFormat("loop ip is {0}", ip);
                            if(list[i].ConnectSocket.RemoteEndPoint.ToString() == ip) 
                            {
                                   list[i].SendEventArgs.SetBuffer(byteInfo.Skip(2).ToArray(), 0, sendLength);
                                    
                                    for(int j=0;j<count;j++)
                                    {
                                        bool willRaiseEvent = list[i].ConnectSocket.SendAsync(list[i].SendEventArgs);
                                    }
                                    
                                    break;
                            }
                           
                        }
                    }
        }

          /// <summary> 
          /// 在指定时间过后执行指定的表达式 
          /// </summary> 
          /// <param name="interval">事件之间经过的时间（以毫秒为单位）</param> 
          /// <param name="action">要执行的表达式</param> 
         private static void SetTimeout(double interval, Action action,string machineId, byte[] byteInfo) 
         { 
             System.Timers.Timer timer = new System.Timers.Timer(interval); 
             timer.Elapsed += delegate(object sender, System.Timers.ElapsedEventArgs e) 
             { 
                 RedisHelper helper0=new RedisHelper(0);
                if(!helper0.KeyExists(machineId)) //判断机器是否在线
                {
                    timer.Enabled = false; 
                } 
                else
                {
                     RedisHelper helper1=new RedisHelper(1);
                    string key =ByteHelper.Ten2Hex(byteInfo[6].ToString());
                    if(key=="42") //是否为出货指令
                    {
                         string outTradeNo = ByteHelper.GenerateRealityData(byteInfo.Skip(19).Take(22).ToArray(), "stringval");
                         if(helper1.KeyExists(outTradeNo))
                         {
                            action();
                         }
                         else
                         {
                            timer.Enabled=false;
                         }
                    }
                    else //非出货指令发两次
                    {
                        if(!helper1.KeyExists(machineId+"-"+key))// 判断该指令是否存在
                         {
                            timer.Enabled = false; 
                        }
                        else
                        {
                             timer.Enabled = false; 
                             action(); 
                        }
                    }
                }
                 
             }; 
             timer.AutoReset= true;
             timer.Enabled = true; 
         } 
    }
}
