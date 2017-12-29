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

                    return returnByte40;
                case "30": //申请签到随机码
                    int size30 = 20;
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
                    int resultA1 = 0;
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
                        jsonModel46.t.Add(new KeyTunnelModel()
                        {
                            tid = ByteHelper.GenerateRealityData(data.Skip(25 + i * 6).Take(5).ToArray(), "stringval"),
                            n = data.Skip(30 + i * 6).Take(1).ToArray()[0].ToString()
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
                    byte resultChunk46 = new byte();
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

                    result48 = imachine.PostMaxStockAndPrice(lstPrice48, machineNum48);
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
    }
}
