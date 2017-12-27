using Fycn.Interface;
using Fycn.Model.Machine;
using Fycn.Model.Pay;
using Fycn.Model.Sale;
using Fycn.Service;
using Fycn.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace Fycn.Sockets
{
    public class MachineLogic
    {
        //处理机器消息
        public byte[] HandleHexByte(byte[] byteInfo, AsyncSocketUserToken m_asyncSocketUserToken, AsyncSocketServer m_asyncSocketServer)
        {
            Program.Logger.InfoFormat("the message is {0}", ByteHelper.byteToHexStr(byteInfo));
            //return byteInfo;
            //ByteHelper.byteToHexStr(byteInfo.Take(4).ToArray());
            //byte[] byteInfo = ByteHelper.strToToHexByte(info);
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
            RedisHelper redisHelper = new RedisHelper(0);
            if (infoHead=="72")
            {


                //大小
                int infoSize = int.Parse(ByteHelper.GenerateRealityData(byteInfo.Skip(1).Take(2).ToArray(), "intval"));
                
                byte[] deencryData = byteInfo;//解密算法
                //验证码
                string infoVerify = byteInfo[3].ToString();
                //数据
                byte[] data = ByteHelper.Deencryption(infoSize, byteInfo.Skip(4).Take(infoSize).ToArray());
                //byte[] data = byteInfo.Skip(3).Take(infoSize).ToArray();
                //string machine_num = Encoding.ASCII.GetString(data, 1, 4); 
                //验证是否为有效包
              
                if (!IsValidPackage(infoVerify, data))
                {
                    return new byte[0];
                }

                //不签到不回复
                if(ByteHelper.Ten2Hex(data[0].ToString()).ToUpper()!="41")
                {
                    if(string.IsNullOrEmpty(m_asyncSocketUserToken.MachineId))
                    {
                        return new byte[0];
                    }
                }
             
                int retResult = 0;
                IMachine imachine = new MachineService();
                //Dao daoBll = new Dao();
                try
                {


                    //验证通过
                    switch (ByteHelper.Ten2Hex(data[0].ToString()).ToUpper())
                    {
                        case "40": //心跳
                            int size40 = 2;
                            byte[] returnByte40 = new byte[7];
                            returnByte40[0] = byteInfo[0];//包头;
                            ByteHelper.IntToTwoByte(size40).CopyTo(returnByte40,1) ; //size
                            returnByte40[4] = data[0];

                            string machineNum40 = ByteHelper.GenerateRealityData(data.Skip(1).Take(12).ToArray(), "stringval");
                            MachineHelper.Signature(machineNum40, m_asyncSocketUserToken.ConnectSocket.RemoteEndPoint.ToString());

                            returnByte40[5] = 0;

                            returnByte40[6] = 238;//
                            //验证码生成
                            byte result40Chunk = new byte();
                            byte[] finalResult40 = returnByte40.Skip(4).Take(size40).ToArray();
                            for (int i = 0; i < finalResult40.Length; i++)
                            {
                                result40Chunk ^= finalResult40[i];
                            }
                            returnByte40[3] = result40Chunk;
                            ByteHelper.Encryption(size40, finalResult40.ToArray()).CopyTo(returnByte40, 4);//加密

                            return returnByte40;

                        case "41": //签到
                            int size41 = 16;
                            int resultA1 = 0;
                            //机器编号
                            string machineNum41 = ByteHelper.GenerateRealityData(data.Skip(1).Take(12).ToArray(), "stringval");
                            //清楚上一个无用的socket连接
                            /*
                            if(redisHelper.StringGet(machineNum41)!= m_asyncSocketUserToken.ConnectSocket.RemoteEndPoint.ToString())
                            {
                                CloseNoUseSocket(redisHelper.StringGet(machineNum41), m_asyncSocketServer);
                            }
                            */resultA1 = imachine.UpdateMachineInlineTimeAndIpv4(machineNum41, m_asyncSocketUserToken.ConnectSocket.RemoteEndPoint.ToString() + "-" + m_asyncSocketUserToken.ConnectSocket.LocalEndPoint.ToString());
                            m_asyncSocketUserToken.MachineId = machineNum41;

                            byte[] returnByteA1 = new byte[21];
                            returnByteA1[0] = byteInfo[0];//包头;
                            ByteHelper.IntToTwoByte(size41).CopyTo(returnByteA1, 1); //size
                            returnByteA1[4] = data[0];


                            ByteHelper.StrToByte(DateTime.Now.ToString("yyyyMMddHHmmss")).CopyTo(returnByteA1, 5);//机器编号
                            
                            if (resultA1 == 1)
                            {
                                MachineHelper.Signature(machineNum41, m_asyncSocketUserToken.ConnectSocket.RemoteEndPoint.ToString());
                                returnByteA1[19] = 48;
                            }
                            else
                            {
                                returnByteA1[19] = 49;
                            }

                            returnByteA1[20]=238;//
                            //验证码生成
                            byte resultA1Chunk = new byte();
                            byte[] finalResultA1 = returnByteA1.Skip(4).Take(size41).ToArray();
                            for (int i = 0; i < finalResultA1.Length; i++)
                            {
                                resultA1Chunk ^= finalResultA1[i];
                            }
                            returnByteA1[3] = resultA1Chunk;
                            //SendMsg(finalResultA1, myClientSocket);
                            ByteHelper.Encryption(size41, finalResultA1.ToArray()).CopyTo(returnByteA1, 4);//加密
                           
                            return returnByteA1;

                        case "43": //上报出货结果
                            int size43 = 2;
                            // string machineNum = ByteHelper.GenerateRealityData(data.Skip(1).Take(12).ToArray(), "stringval");
                            string serialNum = ByteHelper.GenerateRealityData(data.Skip(13).Take(22).ToArray(), "stringval");
                            //清除缓存
                            RedisHelper redis43=new RedisHelper(1);
                            if(redis43.KeyExists(serialNum))
                            {
                                redis43.KeyDelete(serialNum);
                            }
                            byte[] returnByte43 = new byte[7];
                            returnByte43[0] = byteInfo[0];//包头;
                            ByteHelper.IntToTwoByte(size43).CopyTo(returnByte43, 1); //size
                            returnByte43[4] = data[0];
                            string result43 = data[35].ToString();
                            int putResult = 0;
                            if(result43 == "48")
                            {
                                 putResult = imachine.PutPayResultByOrderNo(serialNum, true);
                            }
                            else
                            {
                                 putResult = imachine.PutPayResultByOrderNo(serialNum, false);
                               
                            }
                            if (putResult == 1)
                            {
                                returnByte43[5] = 48;
                            }
                            else
                            {
                                returnByte43[5] = 49;
                            }
                            /*
                            KeyJsonModel jsonModel = new KeyJsonModel();
                            jsonModel.m = machineNum;
                        
                            byte[] lstTunnel = data.Skip(25).Take(data.Length-25).ToArray();
                            int loopTimes43 = lstTunnel.Length / 28;
                            for (int i = 0; i < loopTimes43; i++)
                            {
                                jsonModel.t.Add(new KeyTunnelModel()
                                {
                                    tn = ByteHelper.GenerateRealityData(data.Skip(25 + i * 28).Take(22).ToArray(), "stringval"),
                                    tid = ByteHelper.GenerateRealityData(data.Skip(25 + i * 28+22).Take(5).ToArray(), "stringval"),
                                    n = data.Skip(25 + i * 28 + 27).Take(1).ToArray()[0].ToString()
                                    
                                });
                            }
                        
                            //IMachine imachine = new MachineService();
                            result43 = imachine.PutPayResult(jsonModel);

                        

                            byte[] returnByte43 = new byte[30];
                            returnByte43[0] = byteInfo[0];//包头;
                            returnByte43[1] = 26; //size
                            returnByte43[3] = data[0];
                            data.Skip(1).Take(12).ToArray().CopyTo(returnByte43, 4);//机器编号
                            data.Skip(13).Take(12).ToArray().CopyTo(returnByte43, 15);//流水号

                            if (result43 == 1)
                            {
                                returnByte43[28] = 30;
                            }
                            else
                            {
                                returnByte43[28] = 31;
                            }
                                */
                            returnByte43[6]=238;//结尾
                            //验证码生成
                            byte result43Chunk = new byte();
                            byte[] finalResult43 = returnByte43.Skip(4).Take(size43).ToArray();
                            for (int i = 0; i < finalResult43.Length; i++)
                            {
                                result43Chunk ^= finalResult43[i];
                            }
                            returnByte43[3] = result43Chunk;
                            //SendMsg(finalResultA1, myClientSocket);
                            ByteHelper.Encryption(size43, finalResult43.ToArray()).CopyTo(returnByte43, 4);//加密

                            return returnByte43;
                        case "4A": //验证订单是否合法
                            int size4A = 14;
                            string orerNum4A = ByteHelper.GenerateRealityData(data.Skip(13).Take(22).ToArray(), "stringval");
                       
                            byte[] returnByte4A = new byte[19];
                            returnByte4A[0] = byteInfo[0];//包头;
                            ByteHelper.IntToTwoByte(size4A).CopyTo(returnByte4A, 1); //size
                            returnByte4A[4] = data[0];
                            data.Skip(1).Take(12).ToArray().CopyTo(returnByte4A, 5);
                            RedisHelper redis4A = new RedisHelper(1);
                            if (redis4A.KeyExists(orerNum4A))
                            {
                                returnByte4A[17] = 48;
                            }
                            else
                            {
                                returnByte4A[17] = 49;
                            }

                            returnByte4A[18] = 238;//结尾
                            //验证码生成
                            byte result4AChunk = new byte();
                            byte[] finalResult4A = returnByte4A.Skip(4).Take(size4A).ToArray();
                            for (int i = 0; i < finalResult4A.Length; i++)
                            {
                                result4AChunk ^= finalResult4A[i];
                            }
                            returnByte4A[3] = result4AChunk;
                            //SendMsg(finalResultA1, myClientSocket);
                            ByteHelper.Encryption(size4A, finalResult4A.ToArray()).CopyTo(returnByte4A, 4);//加密

                            return returnByte4A;
                        case "54": //上报一键补货结果 (一键补货)
                            
                            string machineNum65 = ByteHelper.GenerateRealityData(data.Skip(1).Take(12).ToArray(), "stringval");
                            //string serialNum45 = ByteHelper.GenerateRealityData(data.Skip(13).Take(12).ToArray(), "stringval
                            RedisHelper redis65 = new RedisHelper(1);
                            if(redis65.KeyExists(machineNum65 + "-" + 54))
                            {
                                redis65.KeyDelete(machineNum65 + "-" + 54);
                            }
                           
                           
                            //SendMsg(returnByteA6, myClientSocket);
                            return new byte[0];
                        case "53": //上报按货道补货结果

                            string machineNum53 = ByteHelper.GenerateRealityData(data.Skip(1).Take(12).ToArray(), "stringval");
                            //string serialNum45 = ByteHelper.GenerateRealityData(data.Skip(13).Take(12).ToArray(), "stringval
                            RedisHelper redis53 = new RedisHelper(1);
                            if (redis53.KeyExists(machineNum53 + "-" + 53))
                            {
                                redis53.KeyDelete(machineNum53 + "-" + 53);
                            }

                            //SendMsg(returnByteA6, myClientSocket);
                            return new byte[0];
                        case "46": //按货道补货
                            int size46 = 26;
                            string machineNum46 = ByteHelper.GenerateRealityData(data.Skip(1).Take(12).ToArray(), "stringval");
                            string serialNum46 = ByteHelper.GenerateRealityData(data.Skip(13).Take(12).ToArray(), "stringval");
                            //string tunnelNumA5 = ByteHelper.GenerateRealityData(data.Skip(9).Take(5).ToArray(), "stringval");

                            byte[] returnByte46 = new byte[31];
                            returnByte46[0] = byteInfo[0];//包头;
                            ByteHelper.IntToTwoByte(size46).CopyTo(returnByte46, 1); //size
                            returnByte46[4] = data[0];
                            data.Skip(1).Take(12).ToArray().CopyTo(returnByte46, 5);//机器编号
                            data.Skip(13).Take(12).ToArray().CopyTo(returnByte46, 16);//流水号
                            returnByte46[30] = 238;

                            byte[] tunnels = data.Skip(25).Take(data.Length - 25).ToArray();
                            int loopTimes = (tunnels.Length / 6);
                            int result46 = 0;//daoBll.FullfilGoodsByTunnel(tunnels, loopTimes, machineNumA5);
                            KeyJsonModel jsonModel46 = new KeyJsonModel();
                            jsonModel46.m = machineNum46;
                            for (int i = 0; i < loopTimes; i++)
                            {
                                jsonModel46.t.Add(new KeyTunnelModel() {
                                    tid = ByteHelper.GenerateRealityData(data.Skip(25+i*6).Take(5).ToArray(), "stringval"),
                                    n =data.Skip(30 + i * 6).Take(1).ToArray()[0].ToString()
                                });
                            }

                            result46 = imachine.GetFullfilGoodByTunnel(jsonModel46);
                                if (result46 == 1)
                                {
                                    returnByte46[29] = 30;
                                }
                                else
                                {
                                    returnByte46[29] = 31;
                                }
                            byte resultChunk46= new byte();
                            byte[] finalResult46 = returnByte46.Skip(4).Take(size46).ToArray();
                            for (int i = 0; i < returnByte46.Length; i++)
                            {
                                resultChunk46 ^= returnByte46[i];
                            }
                            returnByte46[3] = resultChunk46;
                            //SendMsg(returnByteA5, myClientSocket);
                            ByteHelper.Encryption(size46, finalResult46.ToArray()).CopyTo(returnByte46, 4);//加密
                           return returnByte46;
                        case "48": //补价格
                            int size48 = 26;
                            string machineNum48 = ByteHelper.GenerateRealityData(data.Skip(1).Take(12).ToArray(), "stringval");
                            string serialNum48 = ByteHelper.GenerateRealityData(data.Skip(13).Take(12).ToArray(), "stringval");
                            //string tunnelNumA5 = ByteHelper.GenerateRealityData(data.Skip(9).Take(5).ToArray(), "stringval");

                            byte[] returnByte48 = new byte[31];
                            returnByte48[0] = byteInfo[0];//包头;
                            ByteHelper.IntToTwoByte(size48).CopyTo(returnByte48, 1); //size
                            returnByte48[4] = data[0];
                            data.Skip(1).Take(12).ToArray().CopyTo(returnByte48, 5);//机器编号
                            data.Skip(13).Take(12).ToArray().CopyTo(returnByte48, 16);//流水号
                            returnByte48[30] = 238;

                            byte[] tunnels48 = data.Skip(25).Take(data.Length - 25).ToArray();
                            int loopTimes48 = (tunnels48.Length / 11);
                            int result48 = 0;//daoBll.FullfilGoodsByTunnel(tunnels, loopTimes, machineNumA5);
                            List<PriceAndMaxStockModel> lstPrice48 = new List<PriceAndMaxStockModel>();
                            
                            for (int i = 0; i < loopTimes48; i++)
                            {
                                lstPrice48.Add(new PriceAndMaxStockModel()
                                {
                                    tid = ByteHelper.GenerateRealityData(data.Skip(25 + i * 11).Take(5).ToArray(), "stringval"),
                                    p1 = Convert.ToDecimal(data.Skip(30 + i * 11).Take(6).ToArray()[0])
                                });
                            }

                            result48 = imachine.PostMaxStockAndPrice(lstPrice48,machineNum48);
                            if (result48 == 1)
                            {
                                returnByte48[29] = 30;
                            }
                            else
                            {
                                returnByte48[29] = 31;
                            }
                            byte resultChunk48 = new byte();
                            byte[] finalResult48 = returnByte48.Skip(4).Take(size48).ToArray();
                            for (int i = 0; i < returnByte48.Length; i++)
                            {
                                resultChunk48 ^= returnByte48[i];
                            }
                            returnByte48[3] = resultChunk48;
                            //SendMsg(returnByteA5, myClientSocket);
                            ByteHelper.Encryption(size48, finalResult48.ToArray()).CopyTo(returnByte48, 4);//加密
                            return returnByte48;
                        case "49": //设置最大库存
                            int size49 = 26;
                            string machineNum49 = ByteHelper.GenerateRealityData(data.Skip(1).Take(12).ToArray(), "stringval");
                            string serialNum49 = ByteHelper.GenerateRealityData(data.Skip(13).Take(12).ToArray(), "stringval");
                            //string tunnelNumA5 = ByteHelper.GenerateRealityData(data.Skip(9).Take(5).ToArray(), "stringval");

                            byte[] returnByte49 = new byte[31];
                            returnByte49[0] = byteInfo[0];//包头;
                            returnByte49[1] = 26; //size
                            returnByte49[4] = data[0];
                            data.Skip(1).Take(12).ToArray().CopyTo(returnByte49, 5);//机器编号
                            data.Skip(13).Take(12).ToArray().CopyTo(returnByte49, 16);//流水号
                            returnByte49[30] = 238;

                            byte[] tunnels49 = data.Skip(25).Take(data.Length - 25).ToArray();
                            int loopTimes49 = (tunnels49.Length / 7);
                            int result49 = 0;//daoBll.FullfilGoodsByTunnel(tunnels, loopTimes, machineNumA5);
                            List<PriceAndMaxStockModel> lstPrice49 = new List<PriceAndMaxStockModel>();

                            for (int i = 0; i < loopTimes49; i++)
                            {
                                lstPrice49.Add(new PriceAndMaxStockModel()
                                {
                                    tid = ByteHelper.GenerateRealityData(data.Skip(25 + i * 7).Take(5).ToArray(), "stringval"),
                                    ms = Convert.ToInt32(data.Skip(30 + i * 7).Take(2).ToArray()[0])
                                });
                            }

                            result49 = imachine.PostMaxStockAndPrice(lstPrice49, machineNum49);
                            if (result49 == 1)
                            {
                                returnByte49[29] = 30;
                            }
                            else
                            {
                                returnByte49[29] = 31;
                            }
                            byte resultChunk49 = new byte();
                            byte[] finalResult49 = returnByte49.Skip(4).Take(size49).ToArray();
                            for (int i = 0; i < returnByte49.Length; i++)
                            {
                                resultChunk49 ^= returnByte49[i];
                            }
                            returnByte49[3] = resultChunk49;
                            //SendMsg(returnByteA5, myClientSocket);
                            ByteHelper.Encryption(size49, finalResult49.ToArray()).CopyTo(returnByte49, 4);//加密
                            return returnByte49;
                        case "58":
                            SaleModel saleInfo = new SaleModel();
                            saleInfo.SalesIcId = Guid.NewGuid().ToString();
                            saleInfo.MachineId = "XXXXXX";
                            IBase<SaleModel> ibase = new SalesService();
                            ibase.PostData(saleInfo);
                            byte[] returnByte88 = new byte[2];
                            returnByte88[0] = byteInfo[0];//包头;
                            returnByte88[1] = 238;//包头;
                            return returnByte88;
                    }
                    return new byte[0];
                }
                catch (Exception e)
                {
                    //MessageBox.Show(e.Message);
                    return new byte[0];
                }

            } else if(infoHead=="73") { //推送
               
                //byte[] sendByte = new byte[100];
                int sendLength = byteInfo.Length - 2;
                string ip = string.Empty;
                //ByteHelper.HexToArray("you are succeed").CopyTo(sendByte,0);
                //AsyncSocketUserToken userToken;
                //userToken = new AsyncSocketUserToken(ProtocolConst.ReceiveBufferSize);
                //Socket sc =new Socket()
                /*
                switch (ByteHelper.Ten2Hex(byteInfo[1].ToString()).ToUpper())
                {
               
                    case "10": // 通知出货 42 +机器编号+订单编号+
                     */
                byte[] sendInfo = new byte[byteInfo.Length];
                byteInfo.CopyTo(sendInfo, 0);
                int size73 = int.Parse(ByteHelper.GenerateRealityData(byteInfo.Skip(3).Take(2).ToArray(),"intval"));
                ByteHelper.Deencryption(size73, byteInfo.Skip(6).Take(size73).ToArray()).CopyTo(byteInfo,6);
                string machineId10 = ByteHelper.GenerateRealityData(byteInfo.Skip(7).Take(12).ToArray(), "stringval");
                try
                {
                    if (redisHelper.KeyExists(machineId10)) // 若redis里没有 则去库里查询
                    {
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
                        
                        /*
                        break;
                }
                */
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
