using Fycn.Interface;
using Fycn.Model.Machine;
using Fycn.Model.Pay;
using Fycn.Model.Sale;
using Fycn.Service;
using Fycn.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fycn.Sockets.AsyncSocketCore
{
    public class MachinePush
    {
        public byte[] PushLogic(string commandStr, byte[] byteInfo, byte[] data, AsyncSocketUserToken m_asyncSocketUserToken)
        {
            IMachine imachine = new MachineService();
            //验证通过
            switch (commandStr)
            {
                case "40": //心跳
                    int size40 = 2;
                    byte[] returnByte40 = new byte[7];
                    returnByte40[0] = byteInfo[0];//包头;
                    ByteHelper.IntToTwoByte(size40).CopyTo(returnByte40, 1); //size
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
                    try
                    {
                        imachine.UpdateMachineInlineTimeAndIpv4(machineNum40, m_asyncSocketUserToken.ConnectSocket.RemoteEndPoint.ToString());
                    }
                    catch(Exception e)
                    {
                        return returnByte40;
                    }
                    return returnByte40;
                case "30": //申请签到随机码
                    int size30 = 19;
                    //机器编号
                    string machineNum30 = ByteHelper.GenerateRealityData(data.Skip(1).Take(12).ToArray(), "stringval");

                    byte[] returnByte30 = new byte[24];
                    returnByte30[0] = byteInfo[0];//包头;
                    ByteHelper.IntToTwoByte(size30).CopyTo(returnByte30, 1); //size
                                                                             //returnByte30[4] = data[0];
                    data.Take(13).ToArray().CopyTo(returnByte30, 4);

                    ByteHelper.StrToByte(MachineHelper.GenerateCode(machineNum30, "code")).CopyTo(returnByte30, 17);//机器编号


                    returnByte30[23] = 238;//
                                           //验证码生成
                    byte result30Chunk = new byte();
                    byte[] finalResult30 = returnByte30.Skip(4).Take(size30).ToArray();
                    for (int i = 0; i < finalResult30.Length; i++)
                    {
                        result30Chunk ^= finalResult30[i];
                    }
                    returnByte30[3] = result30Chunk;
                    //SendMsg(finalResultA1, myClientSocket);
                    ByteHelper.Encryption(size30, finalResult30.ToArray()).CopyTo(returnByte30, 4);//加密

                    return returnByte30;
                case "41": //签到
                    int size41 = 16;
                    //机器编号
                    string machineNum41 = ByteHelper.GenerateRealityData(data.Skip(1).Take(12).ToArray(), "stringval");
                    string signCode = ByteHelper.GenerateRealityData(data.Skip(13).Take(6).ToArray(), "stringval");

                    //resultA1 = imachine.UpdateMachineInlineTimeAndIpv4(machineNum41, m_asyncSocketUserToken.ConnectSocket.RemoteEndPoint.ToString() + "-" + m_asyncSocketUserToken.ConnectSocket.LocalEndPoint.ToString());
                    m_asyncSocketUserToken.MachineId = machineNum41;

                    byte[] returnByteA1 = new byte[21];
                    returnByteA1[0] = byteInfo[0];//包头;
                    ByteHelper.IntToTwoByte(size41).CopyTo(returnByteA1, 1); //size
                    returnByteA1[4] = data[0];


                    ByteHelper.StrToByte(DateTime.Now.ToString("yyyyMMddHHmmss")).CopyTo(returnByteA1, 5);//机器编号

                    if (MachineHelper.IsLegal(machineNum41, signCode, "code"))
                    {
                        try
                        {
                            ICommon common41 = new CommonService();
                            int machineCount = common41.CheckMachineId(machineNum41);
                            if (machineCount > 0)
                            {
                                MachineHelper.ClearCode(machineNum41, "code");
                                MachineHelper.Signature(machineNum41, m_asyncSocketUserToken.ConnectSocket.RemoteEndPoint.ToString());
                                returnByteA1[19] = 48;
                            }
                            else
                            {
                                returnByteA1[19] = 49;
                            }
                        }
                        catch (Exception e)
                        {
                            returnByteA1[19] = 50;
                        }

                    }
                    else
                    {
                        returnByteA1[19] = 50;
                    }

                    returnByteA1[20] = 238;//
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
                    RedisHelper redis43 = new RedisHelper(1);
                    if (redis43.KeyExists(serialNum))
                    {
                        redis43.KeyDelete(serialNum);
                    }
                    byte[] returnByte43 = new byte[7];
                    returnByte43[0] = byteInfo[0];//包头;
                    ByteHelper.IntToTwoByte(size43).CopyTo(returnByte43, 1); //size
                    returnByte43[4] = data[0];
                    string result43 = data[35].ToString();
                    int putResult = 0;
                    if (result43 == "48")
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
                    returnByte43[6] = 238;//结尾
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

                    if (MachineHelper.IsLegalOrder(orerNum4A)) //判断是否为合法订单
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
                    if (redis65.KeyExists(machineNum65 + "-" + 54))
                    {
                        redis65.KeyDelete(machineNum65 + "-" + 54);
                    }

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
                case "65": // 终端->服务器 一键补货
                    int size65 = 14;
                    string machineId65 = ByteHelper.GenerateRealityData(data.Skip(1).Take(12).ToArray(), "stringval");

                    byte[] returnByte65 = new byte[19];
                    returnByte65[0] = byteInfo[0];//包头;
                    ByteHelper.IntToTwoByte(size65).CopyTo(returnByte65, 1); //size
                    returnByte65[4] = data[0];
                    data.Skip(1).Take(12).ToArray().CopyTo(returnByte65, 5);
                    
                    int result = imachine.GetFullfilGood(machineId65);
                    try
                    {
                        if (result > 0)
                        {
                            returnByte65[17] = 48;
                        }
                        else
                        {
                            returnByte65[17] = 49;
                        }
                    }
                    catch (Exception e)
                    {
                        returnByte65[17] = 49;
                    }
                    returnByte65[18] = 238;//结尾
                                           //验证码生成
                    byte result65Chunk = new byte();
                    byte[] finalResult65 = returnByte65.Skip(4).Take(size65).ToArray();
                    for (int i = 0; i < finalResult65.Length; i++)
                    {
                        result65Chunk ^= finalResult65[i];
                    }
                    returnByte65[3] = result65Chunk;
                    //SendMsg(finalResultA1, myClientSocket);
                    ByteHelper.Encryption(size65, finalResult65.ToArray()).CopyTo(returnByte65, 4);//加密

                    return returnByte65;
                case "66": // 终端->服务器 详细补货
                    int size66 = 14;
                    string machineNum66 = ByteHelper.GenerateRealityData(data.Skip(1).Take(12).ToArray(), "stringval");

                    byte[] returnByte66 = new byte[19];
                    returnByte66[0] = byteInfo[0];//包头;
                    ByteHelper.IntToTwoByte(size66).CopyTo(returnByte66, 1); //size
                    returnByte66[4] = data[0];
                    data.Skip(1).Take(12).ToArray().CopyTo(returnByte66, 5);//机器编号

                    byte[] tunnels66 = data.Skip(13).Take(data.Length - 13).ToArray();
                    int loopTimes66 = (tunnels66.Length / 7);
                   
                    KeyJsonModel jsonModel66 = new KeyJsonModel();
                    jsonModel66.m = machineNum66;
                    jsonModel66.t = new List<KeyTunnelModel>();
                    for (int i = 0; i < loopTimes66; i++)
                    {
                        jsonModel66.t.Add(new KeyTunnelModel()
                        {
                            tid = ByteHelper.GenerateRealityData(data.Skip(13 + i * 7).Take(5).ToArray(), "stringval"),
                            n = ByteHelper.GenerateRealityData(data.Skip(18 + i * 7).Take(2).ToArray(),"stringval")
                        });
                    }
                    try
                    {
                        int result66 = imachine.GetFullfilGoodByTunnel(jsonModel66);
                        if (result66 > 0)
                        {
                            returnByte66[17] = 48;
                        }
                        else
                        {
                            returnByte66[17] = 49;
                        }
                    }
                    catch (Exception e)
                    {
                        returnByte66[17] = 49;
                    }
                    returnByte66[18] = 238;
                    byte resultChunk66 = new byte();
                    byte[] finalResult66 = returnByte66.Skip(4).Take(size66).ToArray();
                    for (int i = 0; i < finalResult66.Length; i++)
                    {
                        resultChunk66 ^= finalResult66[i];
                    }
                    returnByte66[3] = resultChunk66;
                    //SendMsg(returnByteA5, myClientSocket);
                    ByteHelper.Encryption(size66, finalResult66.ToArray()).CopyTo(returnByte66, 4);//加密
                    return returnByte66;
                case "67": // 终端->服务器 补现金价格
                    int size67 = 14;
                    string machineNum67 = ByteHelper.GenerateRealityData(data.Skip(1).Take(12).ToArray(), "stringval");
                    //string serialNum48 = ByteHelper.GenerateRealityData(data.Skip(13).Take(12).ToArray(), "stringval");
                    //string tunnelNumA5 = ByteHelper.GenerateRealityData(data.Skip(9).Take(5).ToArray(), "stringval");

                    byte[] returnByte67 = new byte[19];
                    returnByte67[0] = byteInfo[0];//包头;
                    ByteHelper.IntToTwoByte(size67).CopyTo(returnByte67, 1); //size
                    returnByte67[4] = data[0];
                    data.Skip(1).Take(12).ToArray().CopyTo(returnByte67, 5);//机器编号
                    

                    byte[] tunnels67 = data.Skip(13).Take(data.Length - 13).ToArray();
                    int loopTimes67 = (tunnels67.Length / 10);
                 
                    List<PriceAndMaxStockModel> lstPrice67 = new List<PriceAndMaxStockModel>();

                    for (int i = 0; i < loopTimes67; i++)
                    {
                        lstPrice67.Add(new PriceAndMaxStockModel()
                        {
                            tid = ByteHelper.GenerateRealityData(data.Skip(13 + i * 10).Take(5).ToArray(), "stringval"),
                            p1 = (decimal)int.Parse(ByteHelper.GenerateRealityData(data.Skip(18 + i * 10).Take(5).ToArray(), "stringval"))/100
                        });
                    }
                    try
                    {
                        int result67 = imachine.PostCashPrice(lstPrice67, machineNum67);
                        if (result67 == 1)
                        {
                            returnByte67[17] = 48;
                        }
                        else
                        {
                            returnByte67[17] = 49;
                        }
                    }
                    catch(Exception e)
                    {
                        returnByte67[17] = 49;
                    }
                    
                    returnByte67[18] = 238;
                    byte resultChunk67 = new byte();
                    byte[] finalResult67 = returnByte67.Skip(4).Take(size67).ToArray();
                    for (int i = 0; i < finalResult67.Length; i++)
                    {
                        resultChunk67 ^= finalResult67[i];
                    }
                    returnByte67[3] = resultChunk67;
                    //SendMsg(returnByteA5, myClientSocket);
                    ByteHelper.Encryption(size67, finalResult67.ToArray()).CopyTo(returnByte67, 4);//加密
                    return returnByte67;
                case "68": // 终端->服务器 补最大库存
                    int size68 = 14;
                    string machineNum68 = ByteHelper.GenerateRealityData(data.Skip(1).Take(12).ToArray(), "stringval");

                    byte[] returnByte68 = new byte[19];
                    returnByte68[0] = byteInfo[0];//包头;
                    ByteHelper.IntToTwoByte(size68).CopyTo(returnByte68, 1); //size
                    returnByte68[4] = data[0];
                    data.Skip(1).Take(12).ToArray().CopyTo(returnByte68, 5);//机器编号


                    byte[] tunnels68 = data.Skip(13).Take(data.Length - 13).ToArray();
                    int loopTimes68 = (tunnels68.Length / 7);

                    List<PriceAndMaxStockModel> lstPrice68 = new List<PriceAndMaxStockModel>();

                    for (int i = 0; i < loopTimes68; i++)
                    {
                        lstPrice68.Add(new PriceAndMaxStockModel()
                        {
                            tid = ByteHelper.GenerateRealityData(data.Skip(13 + i * 7).Take(5).ToArray(), "stringval"),
                            ms = int.Parse(ByteHelper.GenerateRealityData(data.Skip(18 + i * 7).Take(2).ToArray(),"stringval"))
                        });
                    }
                    try
                    {
                        int result68 = imachine.PostMaxPuts(lstPrice68, machineNum68);
                        if (result68 == 1)
                        {
                            returnByte68[17] = 48;
                        }
                        else
                        {
                            returnByte68[17] = 49;
                        }
                    }
                    catch (Exception e)
                    {
                        returnByte68[17] = 49;
                    }
                    returnByte68[18] = 238;
                    byte resultChunk68 = new byte();
                    byte[] finalResult68 = returnByte68.Skip(4).Take(size68).ToArray();
                    for (int i = 0; i < finalResult68.Length; i++)
                    {
                        resultChunk68 ^= finalResult68[i];
                    }
                    returnByte68[3] = resultChunk68;
                    //SendMsg(returnByteA5, myClientSocket);
                    ByteHelper.Encryption(size68, finalResult68.ToArray()).CopyTo(returnByte68, 4);//加密
                    return returnByte68;
            }
            return new byte[0];
        }
    }
}
