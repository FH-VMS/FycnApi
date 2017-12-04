using Fycn.Interface;
using Fycn.Model.Machine;
using Fycn.Model.Pay;
using Fycn.Model.Sale;
using Fycn.Service;
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
            //return byteInfo;
            //Utility.byteToHexStr(byteInfo.Take(4).ToArray());
            //byte[] byteInfo = Utility.strToToHexByte(info);
            //包头
            string infoHead = byteInfo[0].ToString();
            if(infoHead=="72")
            {

            
                //大小
                int infoSize = Convert.ToInt32(byteInfo[1]);
                byte[] deencryData = byteInfo;//解密算法
                //验证码
                string infoVerify = deencryData[2].ToString();
                //数据
                byte[] data = deencryData.Skip(3).Take(infoSize).ToArray();
                //string machine_num = Encoding.ASCII.GetString(data, 1, 4); 
                //验证是否为有效包
                /*
                if (!IsValidPackage(infoVerify, data))
                {
                    return new byte[0];
                }
                */
                int retResult = 0;
                IMachine imachine = new MachineService();
                //Dao daoBll = new Dao();
                try
                {


                    //验证通过
                    switch (Utility.Ten2Hex(data[0].ToString()).ToUpper())
                    {

                        case "41": //签到
                            int resultA1 = 0;
                            //机器编号
                            string machineNum41 = Utility.GenerateRealityData(data.Skip(1).Take(12).ToArray(), "stringval");


                            resultA1 = imachine.UpdateMachineInlineTimeAndIpv4(machineNum41, m_asyncSocketUserToken.ConnectSocket.RemoteEndPoint.ToString() + "-" + m_asyncSocketUserToken.ConnectSocket.LocalEndPoint.ToString());



                            byte[] returnByteA1 = new byte[20];
                            returnByteA1[0] = byteInfo[0];//包头;
                            returnByteA1[1] = 16; //size
                            returnByteA1[3] = data[0];


                            Utility.StrToByte(DateTime.Now.ToString("yyyyMMddHHmmss")).CopyTo(returnByteA1, 4);//机器编号
                            
                            if (resultA1 == 1)
                            {
                                returnByteA1[18] = 48;
                            }
                            else
                            {
                                returnByteA1[18] = 49;
                            }

                            returnByteA1[19]=238;//
                            //验证码生成
                            byte resultA1Chunk = new byte();
                            byte[] finalResultA1 = returnByteA1.Skip(3).Take(returnByteA1[1]).ToArray();
                            for (int i = 0; i < finalResultA1.Length; i++)
                            {
                                resultA1Chunk ^= finalResultA1[i];
                            }
                            returnByteA1[2] = resultA1Chunk;
                            //SendMsg(finalResultA1, myClientSocket);
                            Utility.Encryption(returnByteA1[1], finalResultA1.ToArray()).CopyTo(returnByteA1, 3);//加密
                           
                            return returnByteA1;
                        
                        case "43": //上报出货结果
                            int result43 = 0;
                            string machineNum = Utility.GenerateRealityData(data.Skip(1).Take(12).ToArray(), "stringval");
                            string serialNum = Utility.GenerateRealityData(data.Skip(13).Take(12).ToArray(), "stringval");
                            KeyJsonModel jsonModel = new KeyJsonModel();
                            jsonModel.m = machineNum;
                        
                            byte[] lstTunnel = data.Skip(25).Take(data.Length-25).ToArray();
                            int loopTimes43 = lstTunnel.Length / 28;
                            for (int i = 0; i < loopTimes43; i++)
                            {
                                jsonModel.t.Add(new KeyTunnelModel()
                                {
                                    tn = Utility.GenerateRealityData(data.Skip(25 + i * 28).Take(22).ToArray(), "stringval"),
                                    tid = Utility.GenerateRealityData(data.Skip(25 + i * 28+22).Take(5).ToArray(), "stringval"),
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

                            data[29]=238;//流水号
                            //验证码生成
                            byte result43Chunk = new byte();
                            byte[] finalResult43 = returnByte43.Skip(3).Take(returnByte43[1]).ToArray();
                            for (int i = 0; i < finalResult43.Length; i++)
                            {
                                result43Chunk ^= finalResult43[i];
                            }
                            returnByte43[2] = result43Chunk;
                            //SendMsg(finalResultA1, myClientSocket);
                            Utility.Encryption(returnByte43[1], finalResult43.ToArray()).CopyTo(returnByte43, 3);//加密

                            return returnByte43;
                        case "45": //满仓信息 (一键补货)
                            int result45 = 0;
                            string machineNum45 = Utility.GenerateRealityData(data.Skip(1).Take(12).ToArray(), "stringval");
                            string serialNum45 = Utility.GenerateRealityData(data.Skip(13).Take(12).ToArray(), "stringval");
                        
                        
                            byte[] return45 = new byte[30];
                            return45[0] = byteInfo[0];//包头;
                            return45[1] = 26; //size
                            return45[3] = data[0];
                            data.Skip(1).Take(12).ToArray().CopyTo(return45, 4);
                        

                            //验证码生成
                            byte resultChunk45 = new byte();
                            byte[] finalResult45 = return45.Skip(3).Take(return45[1]).ToArray();

                            result45=imachine.GetFullfilGood(machineNum45);
                            for (int i = 0; i < finalResult45.Length; i++)
                            {
                                resultChunk45 ^= finalResult45[i];
                            }
                            if (result45 == 1)
                            {
                                return45[28] = 30;
                            }
                            else
                            {
                                return45[28] = 31;
                            }
                            return45[2] = resultChunk45;
                            return45[29] = 238;
                            Utility.Encryption(return45[1], finalResult45.ToArray()).CopyTo(return45, 3);//加密
                            //SendMsg(returnByteA6, myClientSocket);
                            return return45;
                        case "A0": //心跳包
                            string machineNumA0 = Utility.GenerateRealityData(data.Skip(1).Take(4).ToArray(), "intval");
                            int execResultA0 = 0;//daoBll.ExistMachine(machineNumA0);
                            byte[] returnByteA0 = new byte[10];
                            returnByteA0[0] = byteInfo[0];//包头;
                            returnByteA0[1] = 5; //size
                            returnByteA0[3] = data[0];
                            data.Skip(1).Take(4).ToArray().CopyTo(returnByteA0, 4);
                            returnByteA0[9] = 238;
                            //验证码生成
                            byte resultA0 = new byte();
                            byte[] finalResultA0 = returnByteA0.Skip(3).Take(returnByteA0[1]).ToArray();
                            for (int i = 0; i < finalResultA0.Length; i++)
                            {
                                resultA0 ^= finalResultA0[i];
                            }
                            if (execResultA0 > 1)
                            {
                                returnByteA0[8] = 0;
                            }
                            else
                            {
                                returnByteA0[8] = 6;
                            }
                            returnByteA0[2] = resultA0;
                            //SendMsg(returnByteA0, myClientSocket);
                            return finalResultA0;
                        case "A7": //读卡验证信息
                            int execResult = 0;//daoBll.VerifyCard(Utility.byteToHexStr(data.Skip(1).Take(4).ToArray()));
                            if (execResult <= 0)
                            {
                                retResult = 10;
                            }
                            byte[] returnByte = new byte[6];
                            returnByte[0] = byteInfo[0];//包头;
                            returnByte[1] = 2; //size
                            returnByte[3] = data[0];
                            returnByte[4] = (byte)retResult;
                            returnByte[5] = 238;
                            //验证码生成
                            byte result = new byte();
                            byte[] finalResult = returnByte.Skip(3).Take(returnByte[1]).ToArray();
                            for (int i = 0; i < finalResult.Length; i++)
                            {
                                result ^= finalResult[i];
                            }
                            returnByte[2] = result;
                            //SendMsg(returnByte, myClientSocket);

                            return returnByte;
                        case "46": //按货道补货
                            string machineNum46 = Utility.GenerateRealityData(data.Skip(1).Take(12).ToArray(), "stringval");
                            string serialNum46 = Utility.GenerateRealityData(data.Skip(13).Take(12).ToArray(), "stringval");
                            //string tunnelNumA5 = Utility.GenerateRealityData(data.Skip(9).Take(5).ToArray(), "stringval");

                            byte[] returnByte46 = new byte[30];
                            returnByte46[0] = byteInfo[0];//包头;
                            returnByte46[1] = 26; //size
                            returnByte46[3] = data[0];
                            data.Skip(1).Take(12).ToArray().CopyTo(returnByte46, 4);//机器编号
                            data.Skip(13).Take(12).ToArray().CopyTo(returnByte46, 15);//流水号
                            returnByte46[29] = 238;

                            byte[] tunnels = data.Skip(25).Take(data.Length - 25).ToArray();
                            int loopTimes = (tunnels.Length / 6);
                            int result46 = 0;//daoBll.FullfilGoodsByTunnel(tunnels, loopTimes, machineNumA5);
                            KeyJsonModel jsonModel46 = new KeyJsonModel();
                            jsonModel46.m = machineNum46;
                            for (int i = 0; i < loopTimes; i++)
                            {
                                jsonModel46.t.Add(new KeyTunnelModel() {
                                    tid = Utility.GenerateRealityData(data.Skip(25+i*6).Take(5).ToArray(), "stringval"),
                                    n =data.Skip(30 + i * 6).Take(1).ToArray()[0].ToString()
                                });
                            }

                            result46 = imachine.GetFullfilGoodByTunnel(jsonModel46);
                                if (result46 == 1)
                                {
                                    returnByte46[28] = 30;
                                }
                                else
                                {
                                    returnByte46[28] = 31;
                                }
                            byte resultChunk46= new byte();
                            byte[] finalResult46 = returnByte46.Skip(3).Take(returnByte46[1]).ToArray();
                            for (int i = 0; i < returnByte46.Length; i++)
                            {
                                resultChunk46 ^= returnByte46[i];
                            }
                            returnByte46[2] = resultChunk46;
                            //SendMsg(returnByteA5, myClientSocket);
                            Utility.Encryption(returnByte46[1], returnByte46.Skip(2).Take(returnByte46.Length-3).ToArray()).CopyTo(returnByte46, 3);//加密
                            return returnByte46;
                        case "48": //补价格
                            string machineNum48 = Utility.GenerateRealityData(data.Skip(1).Take(12).ToArray(), "stringval");
                            string serialNum48 = Utility.GenerateRealityData(data.Skip(13).Take(12).ToArray(), "stringval");
                            //string tunnelNumA5 = Utility.GenerateRealityData(data.Skip(9).Take(5).ToArray(), "stringval");

                            byte[] returnByte48 = new byte[30];
                            returnByte48[0] = byteInfo[0];//包头;
                            returnByte48[1] = 26; //size
                            returnByte48[3] = data[0];
                            data.Skip(1).Take(12).ToArray().CopyTo(returnByte48, 4);//机器编号
                            data.Skip(13).Take(12).ToArray().CopyTo(returnByte48, 15);//流水号
                            returnByte48[29] = 238;

                            byte[] tunnels48 = data.Skip(25).Take(data.Length - 25).ToArray();
                            int loopTimes48 = (tunnels48.Length / 11);
                            int result48 = 0;//daoBll.FullfilGoodsByTunnel(tunnels, loopTimes, machineNumA5);
                            List<PriceAndMaxStockModel> lstPrice48 = new List<PriceAndMaxStockModel>();
                            
                            for (int i = 0; i < loopTimes48; i++)
                            {
                                lstPrice48.Add(new PriceAndMaxStockModel()
                                {
                                    tid = Utility.GenerateRealityData(data.Skip(25 + i * 11).Take(5).ToArray(), "stringval"),
                                    p1 = Convert.ToDecimal(data.Skip(30 + i * 11).Take(6).ToArray()[0])
                                });
                            }

                            result48 = imachine.PostMaxStockAndPrice(lstPrice48,machineNum48);
                            if (result48 == 1)
                            {
                                returnByte48[28] = 30;
                            }
                            else
                            {
                                returnByte48[28] = 31;
                            }
                            byte resultChunk48 = new byte();
                            byte[] finalResult48 = returnByte48.Skip(3).Take(returnByte48[1]).ToArray();
                            for (int i = 0; i < returnByte48.Length; i++)
                            {
                                resultChunk48 ^= returnByte48[i];
                            }
                            returnByte48[2] = resultChunk48;
                            //SendMsg(returnByteA5, myClientSocket);
                            Utility.Encryption(returnByte48[1], finalResult48.ToArray()).CopyTo(returnByte48, 3);//加密
                            return returnByte48;
                        case "49": //设置最大库存
                            string machineNum49 = Utility.GenerateRealityData(data.Skip(1).Take(12).ToArray(), "stringval");
                            string serialNum49 = Utility.GenerateRealityData(data.Skip(13).Take(12).ToArray(), "stringval");
                            //string tunnelNumA5 = Utility.GenerateRealityData(data.Skip(9).Take(5).ToArray(), "stringval");

                            byte[] returnByte49 = new byte[30];
                            returnByte49[0] = byteInfo[0];//包头;
                            returnByte49[1] = 26; //size
                            returnByte49[3] = data[0];
                            data.Skip(1).Take(12).ToArray().CopyTo(returnByte49, 4);//机器编号
                            data.Skip(13).Take(12).ToArray().CopyTo(returnByte49, 15);//流水号
                            returnByte49[29] = 238;

                            byte[] tunnels49 = data.Skip(25).Take(data.Length - 25).ToArray();
                            int loopTimes49 = (tunnels49.Length / 7);
                            int result49 = 0;//daoBll.FullfilGoodsByTunnel(tunnels, loopTimes, machineNumA5);
                            List<PriceAndMaxStockModel> lstPrice49 = new List<PriceAndMaxStockModel>();

                            for (int i = 0; i < loopTimes49; i++)
                            {
                                lstPrice49.Add(new PriceAndMaxStockModel()
                                {
                                    tid = Utility.GenerateRealityData(data.Skip(25 + i * 7).Take(5).ToArray(), "stringval"),
                                    ms = Convert.ToInt32(data.Skip(30 + i * 7).Take(2).ToArray()[0])
                                });
                            }

                            result49 = imachine.PostMaxStockAndPrice(lstPrice49, machineNum49);
                            if (result49 == 1)
                            {
                                returnByte49[28] = 30;
                            }
                            else
                            {
                                returnByte49[28] = 31;
                            }
                            byte resultChunk49 = new byte();
                            byte[] finalResult49 = returnByte49.Skip(3).Take(returnByte49[1]).ToArray();
                            for (int i = 0; i < returnByte49.Length; i++)
                            {
                                resultChunk49 ^= returnByte49[i];
                            }
                            returnByte49[2] = resultChunk49;
                            //SendMsg(returnByteA5, myClientSocket);
                            Utility.Encryption(returnByte49[1], finalResult49.ToArray()).CopyTo(returnByte49, 3);//加密
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
                    return byteInfo;
                }
                catch (Exception e)
                {
                    //MessageBox.Show(e.Message);
                    return byteInfo;
                }

            } else if(infoHead=="73") {
                IMachine imachine = new MachineService();
                byte[] sendByte = new byte[100];
                int sendLength = Utility.StrToByte("you are succeed").Length;
                string ip = string.Empty;
                Utility.StrToByte("you are succeed").CopyTo(sendByte,0);
                //AsyncSocketUserToken userToken;
                //userToken = new AsyncSocketUserToken(ProtocolConst.ReceiveBufferSize);
                //Socket sc =new Socket()
                switch (Utility.Ten2Hex(byteInfo[1].ToString()).ToUpper())
                {
                    case "10":
                        string machineId10 = Utility.GenerateRealityData(byteInfo.Skip(2).Take(12).ToArray(), "stringval");
                        DataTable dt = imachine.GetIpByMachineId(machineId10);
                        if(dt.Rows.Count>0)
                        {
                            ip = dt.Rows[0]["ip_v4"].ToString();
                        }
                        break;
                }
                if (sendLength > 0 && !string.IsNullOrEmpty(ip))
                {
                    AsyncSocketUserToken[] list = null;
                    m_asyncSocketServer.AsyncSocketUserTokenList.CopyList(ref list);
                    for (int i = 0; i < list.Length; i++)
                    {
                        if (list[i].ConnectSocket.Connected&& list[i].ConnectSocket.RemoteEndPoint.ToString() == ip.Split("-")[0])
                        {
                            list[0].SendEventArgs.SetBuffer(sendByte, 0, sendLength);
                            bool willRaiseEvent = list[0].ConnectSocket.SendAsync(list[0].SendEventArgs);
                            break;
                        }
                    }
                    
                }
                
                //x[0].co
                return new byte[0];
            } else {
                string ipAndMessage = Utility.GenerateRealityData(byteInfo, "stringval");
                string ip = ipAndMessage.Split("~")[0];
                byte[] sendByte = Utility.StrToByte(ipAndMessage.Split("~")[1]);
                AsyncSocketUserToken[] list = null;
                m_asyncSocketServer.AsyncSocketUserTokenList.CopyList(ref list);
                for (int i = 0; i < list.Length; i++)
                {
                    if (list[i].ConnectSocket.RemoteEndPoint.ToString() == ip)
                    {
                        list[0].SendEventArgs.SetBuffer(sendByte, 0, sendByte.Length);
                        bool willRaiseEvent = list[0].ConnectSocket.SendAsync(list[0].SendEventArgs);
                        break;
                    }
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
    }
}
