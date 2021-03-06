﻿using Fycn.Interface;
using Fycn.Model.Machine;
using Fycn.Model.Pay;
using Fycn.Model.Sale;
using Fycn.SqlDataAccess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Fycn.Utility;
using Fycn.Model.Sys;

namespace Fycn.Service
{
    public class MachineService : AbstractService, IMachine
    {
        private static OperationLogService operationService
        {
            get
            {
                return new OperationLogService();
            }
        }
        //取商品列表
        public List<ProductForMachineModel> GetProductByMachine(ProductForMachineModel machineInfo)
        {
            var conditions = new List<Condition>();

            if (!string.IsNullOrEmpty(machineInfo.MachineId))
            {
                conditions.Add(new Condition
                {
                    LeftBrace = " AND ",
                    ParamName = "MachineId",
                    DbColumnName = "a.machine_id",
                    ParamValue = machineInfo.MachineId,
                    Operation = ConditionOperate.Equal,
                    RightBrace = "",
                    Logic = ""
                });
            }
            conditions.Add(new Condition
            {
                LeftBrace = "  ",
                ParamName = "",
                DbColumnName = "a.wares_id",
                ParamValue = "",
                Operation = ConditionOperate.GroupBy,
                RightBrace = "",
                Logic = ""
            });
            conditions.Add(new Condition
            {
                LeftBrace = "  ",
                ParamName = "ResourceUrl",
                DbColumnName = "",
                ParamValue = ConfigHandler.ResourceUrl,
                Operation = ConditionOperate.None,
                RightBrace = "",
                Logic = ""
            });
            conditions.AddRange(CreatePaginConditions(machineInfo.PageIndex, machineInfo.PageSize));
            return GenerateDal.LoadByConditions<ProductForMachineModel>(CommonSqlKey.GetProductByMachine, conditions);


        }

             public int GetCount(ProductForMachineModel machineInfo)
        {
            var result = 0;

         
            var conditions = new List<Condition>();
            if (!string.IsNullOrEmpty(machineInfo.MachineId))
            {
                conditions.Add(new Condition
                {
                    LeftBrace = " AND ",
                    ParamName = "MachineId",
                    DbColumnName = "a.machine_id",
                    ParamValue = machineInfo.MachineId,
                    Operation = ConditionOperate.Equal,
                    RightBrace = "",
                    Logic = ""
                });
            }
            conditions.Add(new Condition
            {
                LeftBrace = "  ",
                ParamName = "",
                DbColumnName = "a.wares_id",
                ParamValue = "",
                Operation = ConditionOperate.GroupBy,
                RightBrace = "",
                Logic = ""
            });

            result = GenerateDal.LoadDataTableByConditions(CommonSqlKey.GetProductByMachineCount, conditions).Rows.Count;

            return result;
        }


         //取商品列表安卓
        public List<ProductForMachineModel> GetProductAndroid(ProductForMachineModel machineInfo)
        {
            var conditions = new List<Condition>();

            if (!string.IsNullOrEmpty(machineInfo.MachineId))
            {
                conditions.Add(new Condition
                {
                    LeftBrace = " AND ",
                    ParamName = "MachineId",
                    DbColumnName = "a.machine_id",
                    ParamValue = machineInfo.MachineId,
                    Operation = ConditionOperate.Equal,
                    RightBrace = "",
                    Logic = ""
                });
            }
            conditions.Add(new Condition
            {
                LeftBrace = "  ",
                ParamName = "ResourceUrl",
                DbColumnName = "",
                ParamValue = ConfigHandler.ResourceUrl,
                Operation = ConditionOperate.None,
                RightBrace = "",
                Logic = ""
            });
            conditions.Add(new Condition
            {
                LeftBrace = "  ",
                ParamName = "",
                DbColumnName = "a.wares_id",
                ParamValue = "",
                Operation = ConditionOperate.GroupBy,
                RightBrace = "",
                Logic = ""
            });
            conditions.AddRange(CreatePaginConditions(machineInfo.PageIndex, machineInfo.PageSize));
            return GenerateDal.LoadByConditions<ProductForMachineModel>(CommonSqlKey.GetProductAndroid, conditions);


        }

     public int GetProductAndroidCount(ProductForMachineModel machineInfo)
        {
            var result = 0;

         
            var conditions = new List<Condition>();
            if (!string.IsNullOrEmpty(machineInfo.MachineId))
            {
                conditions.Add(new Condition
                {
                    LeftBrace = " AND ",
                    ParamName = "MachineId",
                    DbColumnName = "a.machine_id",
                    ParamValue = machineInfo.MachineId,
                    Operation = ConditionOperate.Equal,
                    RightBrace = "",
                    Logic = ""
                });
            }
            conditions.Add(new Condition
            {
                LeftBrace = "  ",
                ParamName = "",
                DbColumnName = "a.wares_id",
                ParamValue = "",
                Operation = ConditionOperate.GroupBy,
                RightBrace = "",
                Logic = ""
            });

            result = GenerateDal.LoadDataTableByConditions(CommonSqlKey.GetProductAndroidCount, conditions).Rows.Count;

            return result;
        }


        //微信支付结果插入数据库
        public int PostPayResultW(KeyJsonModel keyJsonModel, string tradeNo, string sellerId, string buyerId, string isConcern,string payDate)
        {
            try
            {
                GenerateDal.BeginTransaction();

                foreach (KeyTunnelModel keyTunnelInfo in keyJsonModel.t)
                {
                    SaleModel saleInfo = new SaleModel();
                    saleInfo.SalesIcId = Guid.NewGuid().ToString();
                    saleInfo.MachineId = keyJsonModel.m;
                    saleInfo.SalesDate = DateTime.Now;
                    saleInfo.SalesNumber = string.IsNullOrEmpty(keyTunnelInfo.n) ? 1 : Convert.ToInt32(keyTunnelInfo.n);
                    saleInfo.PayDate = TransStrToDateTime(payDate, "w");
                    saleInfo.PayInterface = "微信";
                    saleInfo.PayType = "微信";
                    saleInfo.TradeNo = tradeNo;
                    saleInfo.GoodsId = keyTunnelInfo.tid;
                    saleInfo.TradeStatus = 1;
                    saleInfo.MerchantId = sellerId;
                    saleInfo.BuyerId = buyerId;
                    saleInfo.IsWeixinConcern = isConcern;
                    saleInfo.TradeAmount = Convert.ToDouble(keyTunnelInfo.p);
                    saleInfo.ServiceCharge = Math.Round(Convert.ToDouble(keyTunnelInfo.p) * ConfigHandler.WeixinRate, 2, MidpointRounding.AwayFromZero);
                    saleInfo.WaresId = keyTunnelInfo.wid;
                    saleInfo.WaresName = GetProductNameByWaresId(keyTunnelInfo.wid);
                    GenerateDal.Create(saleInfo);
                    //更新存存
                    UpdateCurrStock(keyJsonModel.m, keyTunnelInfo.tid, saleInfo.SalesNumber);
                }
                
                GenerateDal.CommitTransaction();
                return 1;
            }
            catch (Exception e)
            {
                GenerateDal.RollBack();
                return 0;
            }


        }

        //支付宝支付结果插入数据库
        public int PostPayResultA(KeyJsonModel keyJsonModel, string outTradeNo, string tradeNo, string sellerId, string buyerId, string payDate)
        {
            try
            {
                if (string.IsNullOrEmpty(outTradeNo))
                {
                    return 0;
                }
                int isExist = GetCountByTradeNo(outTradeNo);
                if(isExist>0)
                {
                    return 0;
                }
                GenerateDal.BeginTransaction();

                foreach (KeyTunnelModel keyTunnelInfo in keyJsonModel.t)
                {
                    SaleModel saleInfo = new SaleModel();
                    saleInfo.SalesIcId = Guid.NewGuid().ToString();
                    saleInfo.MachineId = keyJsonModel.m;
                    saleInfo.SalesDate = DateTime.Now;
                    saleInfo.SalesNumber = string.IsNullOrEmpty(keyTunnelInfo.n) ? 1 : Convert.ToInt32(keyTunnelInfo.n);
                    saleInfo.PayDate = TransStrToDateTime(payDate,"a");
                    saleInfo.PayInterface = "支付宝";
                    saleInfo.PayType = "支付宝";
                    saleInfo.TradeNo = outTradeNo;
                    saleInfo.ComId = tradeNo;
                    saleInfo.GoodsId = keyTunnelInfo.tid;
                    saleInfo.TradeStatus = 1;
                    saleInfo.MerchantId = sellerId;
                    saleInfo.BuyerId = buyerId;
                    saleInfo.TradeAmount = Convert.ToDouble(keyTunnelInfo.p);
                    saleInfo.ServiceCharge = Math.Round(Convert.ToDouble(keyTunnelInfo.p) * ConfigHandler.ZhifubaoRate, 2, MidpointRounding.AwayFromZero);
                    saleInfo.WaresId = keyTunnelInfo.wid;
                    saleInfo.WaresName = GetProductNameByWaresId(keyTunnelInfo.wid);
                    GenerateDal.Create(saleInfo);
                    //更新存存
                    UpdateCurrStock(keyJsonModel.m, keyTunnelInfo.tid, saleInfo.SalesNumber);
                }

                GenerateDal.CommitTransaction();
                return 1;

            }
            catch (Exception e)
            {
                GenerateDal.RollBack();
                return 0;
            }


        }

        private DateTime TransStrToDateTime(string strDate, string wOrA)
        {
            try
            {
                if (string.IsNullOrEmpty(strDate))
                {
                    return DateTime.Now;
                }
                if (wOrA == "w")
                {
                    if (strDate.Length == 14)
                    {
                        string year = strDate.Substring(0, 4);
                        string month = strDate.Substring(4, 2);
                        string day = strDate.Substring(6, 2);
                        string hour = strDate.Substring(8, 2);
                        string minute = strDate.Substring(10, 2);
                        string second = strDate.Substring(12, 2);
                        return Convert.ToDateTime(string.Format("{0}-{1}-{2} {3}:{4}:{5}", year, month, day, hour, minute, second));
                    }
                }
                else if (wOrA == "a")
                {
                    return Convert.ToDateTime(strDate);
                }
                return DateTime.Now;
            }
            catch(Exception e)
            {
                return DateTime.Now;
            }

            
        }

        //根据商品id取商品名称
        private string GetProductNameByWaresId(string waresId)
        {
            var conditions = new List<Condition>();

            if (!string.IsNullOrEmpty(waresId))
            {
                conditions.Add(new Condition
                {
                    LeftBrace = " AND ",
                    ParamName = "WaresId",
                    DbColumnName = "wares_id",
                    ParamValue = waresId,
                    Operation = ConditionOperate.Equal,
                    RightBrace = "",
                    Logic = ""
                });
            }
            DataTable result = GenerateDal.LoadDataTableByConditions(CommonSqlKey.GetProductNameByWaresId, conditions);
            if (result.Rows.Count > 0)
            {
                return result.Rows[0][0].ToString();
            }
            else
            {
                return "";
            }
        }

        //支付后更新减库存
        private void UpdateCurrStock(string machineId, string tunnelId, int saleNumber)
        {
            TunnelInfoModel tunnelInfo = new TunnelInfoModel();
            tunnelInfo.MachineId = machineId;
            tunnelInfo.GoodsStuId = tunnelId;
            tunnelInfo.CurrStock = saleNumber;
            GenerateDal.Execute(CommonSqlKey.UpdateCurrStock, tunnelInfo);
        }

        //出货失败后更新加库存
        private void UpdateAddCurrStock(string machineId, string tunnelId, int saleNumber)
        {
            try
            {
                TunnelInfoModel tunnelInfo = new TunnelInfoModel();
                tunnelInfo.MachineId = machineId;
                tunnelInfo.GoodsStuId = tunnelId;
                tunnelInfo.CurrStock = saleNumber;
                GenerateDal.Execute(CommonSqlKey.UpdateAddCurrStock, tunnelInfo);
            }
            catch
            {

            }
           
        }

        //是否已存在此单
        public int GetCountByTradeNo(string tradeNo)
        {
            var result = 0;


            var conditions = new List<Condition>();
            conditions.Add(new Condition
            {
                LeftBrace = " AND ",
                ParamName = "TradeNo",
                DbColumnName = "trade_no",
                ParamValue = tradeNo,
                Operation = ConditionOperate.Equal,
                RightBrace = "",
                Logic = ""
            });

            result = GenerateDal.CountByConditions(CommonSqlKey.GetCountByTradeNo, conditions);

            return result;
        }

        // 根据订单号更新出货结果
        public int PutPayResultByOrderNo(string tradeNo, bool result)
        {
            try
            {
                var conditions = new List<Condition>();

                conditions.Add(new Condition
                {
                    LeftBrace = " AND ",
                    ParamName = "TradeNo",
                    DbColumnName = "trade_no",
                    ParamValue = tradeNo,
                    Operation = ConditionOperate.Equal,
                    RightBrace = "",
                    Logic = ""
                });
                List<SaleModel> lstSaleModel = GenerateDal.LoadByConditions<SaleModel>(CommonSqlKey.GetSalesByNo, conditions);
                if(lstSaleModel.Count == 0)
                {
                    return 2;
                }
                GenerateDal.BeginTransaction();

                SaleModel saleModel = lstSaleModel[0];
                if (saleModel != null && saleModel.TradeStatus == 1)
                {
                    //SaleModel saleInfo = new SaleModel();
                    saleModel.SalesDate = DateTime.Now;
                    saleModel.RealitySaleNumber = 1;
                    if (result)
                    {
                        saleModel.TradeStatus = 2;
                    }
                    else
                    {
                        saleModel.RealitySaleNumber = 0;
                        saleModel.TradeStatus = 5;
                        UpdateAddCurrStock(saleModel.MachineId, saleModel.GoodsId, 1);
                        

                    }
                    GenerateDal.Update(CommonSqlKey.UpdatePayResult, saleModel);
                }
                GenerateDal.CommitTransaction();
                RefundService refund = new RefundService();
                if(saleModel.PayInterface=="微信")
                {
                        refund.PostRefundW(lstSaleModel);
                }
                else if(saleModel.PayInterface=="支付宝")
                {
                    refund.PostRefundA(lstSaleModel);
                }
            }
            catch(Exception e)
            {
                return 0;
            }
            
            return 1;
        }

        //上报出货结果  更新销售表状态
        public int PutPayResult(KeyJsonModel keyJsonInfo)
        {
            try
            {
                GenerateDal.BeginTransaction();

                foreach (KeyTunnelModel keyTunnelInfo in keyJsonInfo.t)
                {
                    List<SaleModel> lstSaleModel = GetSalesByNo(keyJsonInfo.m, keyTunnelInfo.tid, keyTunnelInfo.tn);
                    if (lstSaleModel.Count > 0&&lstSaleModel[0].TradeStatus==1)
                    {
                        SaleModel saleInfo = new SaleModel();
                        saleInfo.SalesDate = DateTime.Now;
                        saleInfo.GoodsId = keyTunnelInfo.tid;
                        saleInfo.MachineId = keyJsonInfo.m;
                        saleInfo.TradeNo = keyTunnelInfo.tn;
                        saleInfo.RealitySaleNumber = Convert.ToInt32(keyTunnelInfo.n);
                        saleInfo.TradeStatus = Convert.ToInt32(keyTunnelInfo.s);
                        //出货失败后库存回滚
                        if (saleInfo.TradeStatus == 5)
                        {
                            saleInfo.RealitySaleNumber=0;
                            UpdateAddCurrStock(saleInfo.MachineId, saleInfo.GoodsId, lstSaleModel[0].SalesNumber);
                        }
                        if (saleInfo.TradeStatus == 3)
                        {
                            saleInfo.RealitySaleNumber=0;
                            UpdateAddCurrStock(saleInfo.MachineId, saleInfo.GoodsId, lstSaleModel[0].SalesNumber - saleInfo.RealitySaleNumber);
                        }
                        GenerateDal.Update(CommonSqlKey.UpdatePayResult, saleInfo);
                    }
                   
                }

                GenerateDal.CommitTransaction();

            }
            catch (Exception e)
            {
                GenerateDal.RollBack();
                return 0;
            }
            return 1;
        }

        private List<SaleModel> GetSalesByNo(string machineId,string tunnelId,string tradeNo)
        {
            if (string.IsNullOrEmpty(machineId) || string.IsNullOrEmpty(tunnelId) || string.IsNullOrEmpty(tradeNo))
            {
                return null;
            }
            var conditions = new List<Condition>();

            
                conditions.Add(new Condition
                {
                    LeftBrace = " AND ",
                    ParamName = "MachineId",
                    DbColumnName = "machine_id",
                    ParamValue = machineId,
                    Operation = ConditionOperate.Equal,
                    RightBrace = "",
                    Logic = ""
                });
            
            conditions.Add(new Condition
            {
                LeftBrace = " AND ",
                ParamName = "GoodsId",
                DbColumnName = "goods_id",
                ParamValue = tunnelId,
                Operation = ConditionOperate.Equal,
                RightBrace = "",
                Logic = ""
            });
            conditions.Add(new Condition
            {
                LeftBrace = " AND ",
                ParamName = "TradeNo",
                DbColumnName = "trade_no",
                ParamValue = tradeNo,
                Operation = ConditionOperate.Equal,
                RightBrace = "",
                Logic = ""
            });
            return GenerateDal.LoadByConditions<SaleModel>(CommonSqlKey.GetSalesByNo, conditions);
        }

        //一键补货操作
        public int GetFullfilGood(string machineId)
        {
            try
            {
                TunnelInfoModel tunnelInfo = new TunnelInfoModel();
                tunnelInfo.MachineId = machineId;
                //int delResult = GenerateDal.Delete<TunnelInfoModel>(CommonSqlKey.DeleteTunnelStatusByMachine, tunnelInfo);
                var conditions = new List<Condition>();
              
                conditions.Add(new Condition
                {
                    LeftBrace = " AND ",
                    ParamName = "MachineId",
                    DbColumnName = "machine_id",
                    ParamValue = machineId,
                    Operation = ConditionOperate.Equal,
                    RightBrace = "",
                    Logic = ""
                });

                int resultStatusCount = GenerateDal.CountByConditions(CommonSqlKey.IsExistTunnelInfo, conditions);
                int resultConfigCount = GenerateDal.CountByConditions(CommonSqlKey.GetTunnelConfigCount, conditions);
                if (resultStatusCount == resultConfigCount)
                {
                    GenerateDal.Execute<TunnelInfoModel>(CommonSqlKey.UpdateFullfilGoodsOneKey, tunnelInfo);
                }
                else
                {
                    GenerateDal.Delete<TunnelInfoModel>(CommonSqlKey.DeleteTunnelStatusByMachine, tunnelInfo);
                    GenerateDal.Execute<TunnelInfoModel>(CommonSqlKey.FullfilGoodsOneKey, tunnelInfo);
                }
                
                //往机器下行表里插入库存改变的数据
                PostToMachine(machineId, "st");
                //机器管理操作日志
                operationService.PostData(new OperationLogModel() { MachineId = machineId,OptContent="一键补货" });
                /*
                GenerateDal.BeginTransaction();
                TunnelInfoModel tunnelInfo = new TunnelInfoModel();
                tunnelInfo.MachineId = machineId;
                GenerateDal.Delete<TunnelInfoModel>(CommonSqlKey.DeleteTunnelStatusByMachine, tunnelInfo);
                GenerateDal.Execute<TunnelInfoModel>(CommonSqlKey.FullfilGoodsOneKey, tunnelInfo);

                GenerateDal.CommitTransaction();
                */
            }
            catch (Exception e)
            {
                //GenerateDal.RollBack();
                return 0;
            }
            return 1;
        }

        //按货道更新
        public int GetFullfilGoodByTunnel(KeyJsonModel keyJsonModel)
        {
            try
            {
                GenerateDal.BeginTransaction();
                foreach (KeyTunnelModel keyTunnelInfo in keyJsonModel.t)
                {
                    TunnelInfoModel tunnelInfo = new TunnelInfoModel();
                    tunnelInfo.MachineId = keyJsonModel.m;
                    tunnelInfo.GoodsStuId = keyTunnelInfo.tid;
                    if (!string.IsNullOrEmpty(keyTunnelInfo.n))
                    {
                        tunnelInfo.CurrStock = Convert.ToInt32(keyTunnelInfo.n);
                    }
                    
                    if (string.IsNullOrEmpty(keyTunnelInfo.s))
                    {
                        tunnelInfo.CurrStatus = "1";
                    }
                    else
                    {
                        tunnelInfo.CurrStatus = keyTunnelInfo.s;
                    }
                   
                    tunnelInfo.UpdateDate = DateTime.Now;
                    tunnelInfo.CabinetId = GetCabinetIdByMachine(keyJsonModel.m);
                    /*
                    var conditions = new List<Condition>();

                    conditions.Add(new Condition
                    {
                        LeftBrace = " AND ",
                        ParamName = "MachineId",
                        DbColumnName = "machine_id",
                        ParamValue = tunnelInfo.MachineId,
                        Operation = ConditionOperate.Equal,
                        RightBrace = "",
                        Logic = ""
                    });

                    conditions.Add(new Condition
                    {
                        LeftBrace = " AND ",
                        ParamName = "GoodsStuId",
                        DbColumnName = "goods_stu_id",
                        ParamValue = tunnelInfo.GoodsStuId,
                        Operation = ConditionOperate.Equal,
                        RightBrace = "",
                        Logic = ""
                    });

                    conditions.Add(new Condition
                    {
                        LeftBrace = " AND ",
                        ParamName = "CabinetId",
                        DbColumnName = "cabinet_id",
                        ParamValue = tunnelInfo.CabinetId,
                        Operation = ConditionOperate.Equal,
                        RightBrace = "",
                        Logic = ""
                    });

                    int resultStatusCount = GenerateDal.CountByConditions(CommonSqlKey.IsExistTunnelInfo, conditions);
                    if (resultStatusCount > 0)
                    {
                       
                    }
                    else
                    {
                        GenerateDal.Create<TunnelInfoModel>(tunnelInfo);
                    }
                    */
                    GenerateDal.Delete<TunnelInfoModel>(CommonSqlKey.DeleteTunnelStatusByMachineAndTunnel, tunnelInfo);
                    GenerateDal.Create<TunnelInfoModel>(tunnelInfo);
                }

                //往机器下行表里插入库存改变的数据
                PostToMachine(keyJsonModel.m, "st");
                //操作日志
                operationService.PostData(new OperationLogModel() { MachineId = keyJsonModel.m, OptContent = "按货道补货" });
                GenerateDal.CommitTransaction();
                
            }
            catch (Exception e)
            {
                GenerateDal.RollBack();
                return 0;
            }
            return 1;
        }

        private string GetCabinetIdByMachine(string machineId)
        {
            var conditions = new List<Condition>();

            if (!string.IsNullOrEmpty(machineId))
            {
                conditions.Add(new Condition
                {
                    LeftBrace = " AND ",
                    ParamName = "MachineId",
                    DbColumnName = "b.machine_id",
                    ParamValue = machineId,
                    Operation = ConditionOperate.Equal,
                    RightBrace = "",
                    Logic = ""
                });
            }
            DataTable dt = GenerateDal.LoadDataTableByConditions(CommonSqlKey.GetCabinetId, conditions);
            if (dt.Rows.Count > 0)
            {
                return dt.Rows[0][0].ToString();
            } else
            {
                return "";
            }
            
        }

       

        //心跳包
        public DataTable GetBeepHeart(string machineId)
        {
            IDictionary<string,object> dicParam = new Dictionary<string,object>();
            dicParam.Add("P_MachineId",machineId);
            UpdateMachineInlineTime(machineId);
            return GenerateDal.LoadDataTable(CommonSqlKey.GetBeepHeart, dicParam);
        }

        public int UpdateMachineInlineTime(string machineId)
        {
            MachineListModel machineList = new MachineListModel();
            machineList.MachineId = machineId;
            machineList.LatestDate = DateTime.Now;

            return GenerateDal.Update(CommonSqlKey.UpdateMachineInlineTime, machineList);
        }

        public int UpdateMachineInlineTimeAndIpv4(string machineId,int signal,int temp40,int door40, string ipv4)
        {
            MachineListModel machineList = new MachineListModel();
            machineList.MachineId = machineId;
            machineList.IpV4 = ipv4;
            machineList.Signal=signal;
            machineList.MachineTemp=Convert.ToDouble(temp40);
            machineList.Door=door40;
            machineList.LatestDate = DateTime.Now;

            return GenerateDal.Update(CommonSqlKey.UpdateMachineInlineTimeAndIpv4, machineList);
        }

        //机器上报下行处理结果
        public int GetHandleResult(string machineId, string machineStatus)
        {
            try
            {
                ToMachineModel toMachineInfo = new ToMachineModel();
                toMachineInfo.MachineId = machineId;
                toMachineInfo.MachineStatus = machineStatus;
                GenerateDal.Delete<ToMachineModel>(CommonSqlKey.DeleteToMachine, toMachineInfo);
            }
            catch (Exception e)
            {
                //GenerateDal.RollBack();
                return 0;
            }
            return 1;
        }

        //向机器下行表插入数据
        public int PostToMachine(string machineId, string machineStatus)
        {
            /*
            try
            {
                if (!string.IsNullOrEmpty(machineId) && !string.IsNullOrEmpty(machineStatus))
                {
                    GenerateDal.BeginTransaction();
                    ToMachineModel toMachineInfo = new ToMachineModel();
                    toMachineInfo.MachineId = machineId;
                    toMachineInfo.MachineStatus = machineStatus;
                    GenerateDal.Delete<ToMachineModel>(CommonSqlKey.DeleteToMachine, toMachineInfo);
                    GenerateDal.Create<ToMachineModel>(toMachineInfo);

                    GenerateDal.CommitTransaction();
                }
                
               
            }
            catch (Exception e)
            {
                GenerateDal.RollBack();
                return 0;
            }
            */
            return 1;
        }


        //向机器下行价格
        public DataTable GetToMachinePrice(string machineId, int startNo, int len)
        {
            var conditions = new List<Condition>();

            if (!string.IsNullOrEmpty(machineId))
            {
                conditions.Add(new Condition
                {
                    LeftBrace = " AND ",
                    ParamName = "MachineId",
                    DbColumnName = "machine_id",
                    ParamValue = machineId,
                    Operation = ConditionOperate.Equal,
                    RightBrace = "",
                    Logic = ""
                });
            }

           

            conditions.Add(new Condition
            {
                LeftBrace = "  ",
                ParamName = "TunnelId",
                DbColumnName = "tunnel_id",
                ParamValue = "asc",
                Operation = ConditionOperate.OrderBy,
                RightBrace = "",
                Logic = ""
            });

            conditions.Add(new Condition
            {
                LeftBrace = "",
                ParamName = "Index",
                DbColumnName = "",
                ParamValue = startNo,
                Operation = ConditionOperate.LimitIndex,
                RightBrace = "",
                Logic = ""

            });

            conditions.Add(new Condition
            {
                LeftBrace = "",
                ParamName = "Length",
                DbColumnName = "",
                ParamValue = len,
                Operation = ConditionOperate.LimitLength,
                RightBrace = "",
                Logic = ""

            });


            return GenerateDal.LoadDataTableByConditions(CommonSqlKey.ToMachinePrice, conditions);
        }


        //向机器下行当前补货库存
        public DataTable GetToMachineStock(string machineId, int startNo, int len)
        {
            var conditions = new List<Condition>();

            if (!string.IsNullOrEmpty(machineId))
            {
                conditions.Add(new Condition
                {
                    LeftBrace = " AND ",
                    ParamName = "MachineId",
                    DbColumnName = "machine_id",
                    ParamValue = machineId,
                    Operation = ConditionOperate.Equal,
                    RightBrace = "",
                    Logic = ""
                });
            }



            conditions.Add(new Condition
            {
                LeftBrace = "  ",
                ParamName = "GoodsStuId",
                DbColumnName = "goods_stu_id",
                ParamValue = "asc",
                Operation = ConditionOperate.OrderBy,
                RightBrace = "",
                Logic = ""
            });

            conditions.Add(new Condition
            {
                LeftBrace = "",
                ParamName = "Index",
                DbColumnName = "",
                ParamValue = startNo,
                Operation = ConditionOperate.LimitIndex,
                RightBrace = "",
                Logic = ""

            });

            conditions.Add(new Condition
            {
                LeftBrace = "",
                ParamName = "Length",
                DbColumnName = "",
                ParamValue = len,
                Operation = ConditionOperate.LimitLength,
                RightBrace = "",
                Logic = ""

            });


            return GenerateDal.LoadDataTableByConditions(CommonSqlKey.ToMachineStock, conditions);
        }

        //机器端设置价格和最大库存上报
        public int PostMaxStockAndPrice(List<PriceAndMaxStockModel> lstPriceAndStock, string machineId)
        {
            try
            {
                GenerateDal.BeginTransaction();
                foreach (PriceAndMaxStockModel priceAndStock in lstPriceAndStock)
                {
                    TunnelConfigModel tc = new TunnelConfigModel();
                    tc.MachineId = machineId;
                    tc.TunnelId = priceAndStock.tid;
                    tc.CashPrices = priceAndStock.p1;
                    tc.AlipayPrices = priceAndStock.p2;
                    tc.WpayPrices = priceAndStock.p3;
                    tc.IcPrices = priceAndStock.p4;
                    tc.MaxPuts = priceAndStock.ms;

                    if (priceAndStock.p1 == 0 && priceAndStock.ms != 0)
                    {
                        GenerateDal.Update(CommonSqlKey.UpdateMaxPuts, tc);
                    }
                    else if (priceAndStock.p1 != 0 && priceAndStock.ms == 0)
                    {
                        GenerateDal.Update(CommonSqlKey.UpdatePrice, tc);
                    }
                    else
                    {
                        GenerateDal.Update(CommonSqlKey.PostPriceAndMaxStock, tc);
                    }


                }
                PostToMachine(machineId, "p");
                //操作日志
                operationService.PostData(new OperationLogModel() { MachineId = machineId, OptContent = "机器端设置价格和库存" });
                GenerateDal.CommitTransaction();

            }
            catch (Exception e)
            {
                //GenerateDal.RollBack();
                return 0;
            }
            return 1;
           
        }

        //机器端设置现金价格
        public int PostCashPrice(List<PriceAndMaxStockModel> lstPriceAndStock, string machineId)
        {
            try
            {
                GenerateDal.BeginTransaction();
                foreach (PriceAndMaxStockModel priceAndStock in lstPriceAndStock)
                {
                    TunnelConfigModel tc = new TunnelConfigModel();
                    tc.MachineId = machineId;
                    tc.TunnelId = priceAndStock.tid;
                    tc.CashPrices = priceAndStock.p1;
                    tc.IcPrices= priceAndStock.p1;
                    tc.WpayPrices= priceAndStock.p1;
                    tc.AlipayPrices= priceAndStock.p1;


                    GenerateDal.Update(CommonSqlKey.UpdateCashPrice, tc);


                }
                PostToMachine(machineId, "p");
                //操作日志
                operationService.PostData(new OperationLogModel() { MachineId = machineId, OptContent = "机器端设置价格和库存" });
                GenerateDal.CommitTransaction();

            }
            catch (Exception e)
            {
                //GenerateDal.RollBack();
                return 0;
            }
            return 1;

        }

        //机器端设置最大库存
        public int PostMaxPuts(List<PriceAndMaxStockModel> lstPriceAndStock, string machineId)
        {
            try
            {
                GenerateDal.BeginTransaction();
                foreach (PriceAndMaxStockModel priceAndStock in lstPriceAndStock)
                {
                    TunnelConfigModel tc = new TunnelConfigModel();
                    tc.MachineId = machineId;
                    tc.TunnelId = priceAndStock.tid;
                    tc.MaxPuts = priceAndStock.ms;

                    GenerateDal.Update(CommonSqlKey.UpdateMaxPuts, tc);


                }
                GenerateDal.CommitTransaction();

            }
            catch (Exception e)
            {
                //GenerateDal.RollBack();
                return 0;
            }
            return 1;

        }

        //机器配置发给机器
        public DataTable GetMachineSetting(string machineId)
        {
            var conditions = new List<Condition>();

            if (!string.IsNullOrEmpty(machineId))
            {
                conditions.Add(new Condition
                {
                    LeftBrace = " AND ",
                    ParamName = "MachineId",
                    DbColumnName = "machine_id",
                    ParamValue = machineId,
                    Operation = ConditionOperate.Equal,
                    RightBrace = "",
                    Logic = ""
                });
            }

            return GenerateDal.LoadDataTableByConditions(CommonSqlKey.GetMachineSetting, conditions);
        }

        //取机器情况根据machine_id
        public DataTable GetMachineByMachineId(string machineId)
        {
            var conditions = new List<Condition>();

            if (!string.IsNullOrEmpty(machineId))
            {
                conditions.Add(new Condition
                {
                    LeftBrace = " AND ",
                    ParamName = "DeviceId",
                    DbColumnName = "device_id",
                    ParamValue = machineId,
                    Operation = ConditionOperate.Equal,
                    RightBrace = "",
                    Logic = ""
                });
            }

            return GenerateDal.LoadDataTableByConditions(CommonSqlKey.GetMachineByMachineId, conditions);
        }

        //取Ip根据machine_id
        public DataTable GetIpByMachineId(string machineId)
        {
            var conditions = new List<Condition>();

            if (!string.IsNullOrEmpty(machineId))
            {
                conditions.Add(new Condition
                {
                    LeftBrace = " AND ",
                    ParamName = "DeviceId",
                    DbColumnName = "device_id",
                    ParamValue = machineId,
                    Operation = ConditionOperate.Equal,
                    RightBrace = "",
                    Logic = ""
                });
            }

            return GenerateDal.LoadDataTableByConditions(CommonSqlKey.GetIpByMachineId, conditions);
        }

        //根据设备id取机器id
        public string GetMachineIdByDeviceId(string deviceId)
        {
            var conditions = new List<Condition>();

            if (string.IsNullOrEmpty(deviceId))
            {
                return "";
            }

            conditions.Add(new Condition
            {
                LeftBrace = " AND ",
                ParamName = "DeviceId",
                DbColumnName = "device_id",
                ParamValue = deviceId,
                Operation = ConditionOperate.Equal,
                RightBrace = "",
                Logic = ""
            });
            var lstMachine = GenerateDal.LoadByConditions<MachineListModel>(CommonSqlKey.GetMachineIdByDeviceId, conditions);
            if(lstMachine.Count==0)
            {
                return "";
            }
            else
            {
                return lstMachine[0].MachineId;
            }
        }
    }
}
