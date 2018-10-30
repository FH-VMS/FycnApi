﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using Fycn.Interface;
using Fycn.Model.Ad;
using Fycn.Model.Android;
using Fycn.Model.Machine;
using Fycn.Model.Pay;
using Fycn.Model.Privilege;
using Fycn.Model.Product;
using Fycn.Model.Sale;
using Fycn.Model.Socket;
using Fycn.Model.Sys;
using Fycn.Model.Wechat;
using Fycn.PaymentLib;
using Fycn.PaymentLib.wx;
using Fycn.Service;
using Fycn.Utility;
using FycnApi.Base;
using log4net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Payment.wx;

namespace FycnApi.Controllers
{
    //[Produces("application/json")]
    //[Route("api/Hi")]
    public class HiController : ApiBaseController
    {
        public ResultObj<List<AndroidProductModel>> GetProductByMachine(string machineId, string waresTypeId = "", int pageIndex = 1, int pageSize = 10)
        {
            if (string.IsNullOrEmpty(machineId))
            {
                return Content(new List<AndroidProductModel>());
            }
            //KeyJsonModel keyJsonInfo = AnalizeKey(k);
            // IProduct service = new ProductService();
            //List<ProductModel> products = service.GetAllProducts();
            //k = "ABC123456789";
            //机器运行情况

            /*
            DataTable dt = _IMachine.GetMachineByMachineId(k);
            if (dt == null || dt.Rows.Count == 0)
            {
                return Content(new List<ProductForMachineModel>(), ResultCode.Success, "机器不存在", new Pagination { });
            }
            //判断机器是否在线 时间大于十五分钟为离线
            if (string.IsNullOrEmpty(dt.Rows[0][0].ToString()))
            {
                return Content(new List<ProductForMachineModel>(), ResultCode.Success, "机器不在线", new Pagination { });
            }
            int intval = Convert.ToInt32(dt.Rows[0][0]);
            if (intval > 900)
            {
                return Content(new List<ProductForMachineModel>(), ResultCode.Success, "机器不在线", new Pagination { });
            }
            */
            /*
            if (!MachineHelper.IsOnline(machineId))
            {
                return Content(new List<ProductForMachineModel>(), ResultCode.Success, "机器不在线", new Pagination { });
            }
            */
            AndroidProductModel machineInfo = new AndroidProductModel();
            machineInfo.MachineId = machineId;
            machineInfo.PageIndex = pageIndex;
            machineInfo.PageSize = pageSize;
            IAndroid imachine = new AndroidService();
            var data = imachine.GetProductAndroid(machineInfo);
            //int totalcount = imachine.GetProductAndroidCount(machineInfo);

            //var pagination = new Pagination { PageSize = pageSize, PageIndex = pageIndex, StartIndex = 0, TotalRows = totalcount, TotalPage = 0 };

            return Content(data);
        }

        public ResultObj<List<AndroidProductTypeModel>> GetProductTypeByMachine(string machineId)
        {
            if (string.IsNullOrEmpty(machineId))
            {
                return Content(new List<AndroidProductTypeModel>());
            }

            IAndroid imachine = new AndroidService();
            var data = imachine.GetProductTypeByMachine(machineId);
            return Content(data);
        }

        public ResultObj<PayStateModel> WechatAuth(string machineId, string code, string retBack = "")
        {
            var log = LogManager.GetLogger("FycnApi", "wechat");
            string clientId = string.Empty;
            //log.Info(string.Format("machine id is {0}, code is {1}, retBack is {2}", machineId, code, retBack));
            string url = string.Empty;
            //KeyJsonModel keyJsonInfo = PayHelper.AnalizeKey(k);
            IPay _ipay = new PayService();
            WxPayConfig payConfig = _ipay.GenerateConfigModelW(machineId);
            clientId = payConfig.ClientId;
            PayStateModel payState = new PayStateModel();
            if (string.IsNullOrEmpty(payConfig.APPID))
            {
                payState.RequestState = "2";
                payState.ProductJson = "";
                payState.RequestData = "";
                return Content(payState);

            }
            PayModel payInfo = new PayModel();
            //JsApi.payInfo = new PayModel();
            payInfo.k = clientId;
            JsApi jsApi = new JsApi();
            string backUrl = "";
            if (!string.IsNullOrEmpty(retBack))
            {
                backUrl = backUrl + HttpUtility.UrlDecode(retBack);
            }
            else
            {
                return null;
            }
            jsApi.GetOpenidAndAccessToken(code, payConfig, payInfo, backUrl, "snsapi_userinfo");
            //log.Info(string.Format("openid id is {0}", payInfo.openid));
            if (string.IsNullOrEmpty(payInfo.openid))
            {
                payState.RequestState = "0";
                payState.ProductJson = "";
                payState.RequestData = payInfo.redirect_url;
                return Content(payState);
            }
            //根据code 返回access_token
            //string urlAcess = string.Format("https://api.weixin.qq.com/sns/oauth2/access_token?appid={0}&secret={1}&code={2}&grant_type=authorization_code", payConfig.APPID,payConfig.APPSECRET,code);
            //string jsonResult = HttpService.Get(urlAcess);
            //log.Info("access_token:" + jsonResult);

            //Dictionary<string,string> dicAcess = JsonHandler.GetObjectFromJson<Dictionary<string,string>>(jsonResult);
            string accessToken = payInfo.access_token;//dicAcess["access_token"];
            //log.Info(string.Format("access token is {0}", accessToken));
            //取授权用户信息
            string urlUserInfo = string.Format("https://api.weixin.qq.com/sns/userinfo?access_token={0}&openid={1}&lang=zh_CN", accessToken, payInfo.openid);
            string jsonUserInfo = HttpService.Get(urlUserInfo);

            payState.RequestState = "1";
            payState.ProductJson = jsonUserInfo;
            payState.RequestData = "";
            IWechat iwechat = new WechatService();
            WechatMemberModel memberInfo = new WechatMemberModel();
            memberInfo.ClientId = clientId;
            memberInfo.OpenId = payInfo.openid;
            List<WechatMemberModel> lstMemberInfo = iwechat.IsExistMember(memberInfo);
            if (lstMemberInfo.Count == 0)
            {
                WechatMemberModel createMemberInfo = JsonHandler.GetObjectFromJson<WechatMemberModel>(jsonUserInfo);
                createMemberInfo.ClientId = clientId;
                iwechat.CreateMember(createMemberInfo);
            }

            //log.Info(string.Format("userinfo is {0}", jsonUserInfo));
            //if(iwechat.IsExistMember(memberInfo))
            //log.Info("test");

            return Content(payState);
        }


        //微信支付立即出货
        public ResultObj<PayStateModel> PostDataW(string machineId, string openId,string hiPara="-1", [FromBody]List<ProductPayModel> lstProductPay=null)
        {
            var log = LogManager.GetLogger("FycnApi", "wechat");
            if(lstProductPay==null)
            {
                PayStateModel payStateNull = new PayStateModel();
                return Content(payStateNull, ResultCode.Success, "传参有误", new Pagination { });
            }
            string clientId = string.Empty;
            try
            {
                RedisHelper redisHelper = new RedisHelper(0);
                if (!redisHelper.KeyExists(machineId))
                {
                    PayStateModel payStateNull = new PayStateModel();
                    return Content(payStateNull, ResultCode.Success, "机器不在线", new Pagination { });
                }

                IPay _ipay = new PayService();
                //移动支付赋值
                WxPayConfig payConfig = _ipay.GenerateConfigModelW(machineId);
                clientId = payConfig.ClientId;

                log.Info(string.Format("lstProductPay is {0}", JsonHandler.GetJsonStrFromObject(lstProductPay)));
                JsApi jsApi = new JsApi();
                PayModel payInfo = new PayModel();
                payInfo.openid = openId;
                //JsApi.payInfo = new PayModel();
                //生成code 根据code取微信支付的openid和access_token
                //jsApi.GetOpenidAndAccessToken(code, payConfig, payInfo, "/wechat.html#/pay?clientId=" + clientId, "");

                PayStateModel payState = new PayStateModel();


                //JSAPI支付预处理

                //string result = HttpService.Get(payInfo.redirect_url);
                //生成交易号
                payInfo.trade_no = new PayHelper().GeneraterTradeNo();
                //payInfo.jsonProduct = clientId;
                //取商品信息

                KeyJsonModel keyJsonInfo = new KeyJsonModel();
                keyJsonInfo.m = machineId;
                keyJsonInfo.t = new List<KeyTunnelModel>();
                foreach (ProductPayModel productPay in lstProductPay)
                {
                    keyJsonInfo.t.Add(new KeyTunnelModel()
                    {
                        tid= productPay.WaresId,
                        n="1"
                    });
                }

                decimal totalFee = 0;
                string productNames = string.Empty;
                List<ProductModel> lstProduct = new List<ProductModel>();
                lstProduct = _ipay.GetProducInfoByWaresId(machineId, keyJsonInfo.t);
                //遍历商品
                foreach (ProductModel productInfo in lstProduct)
                {
                    var tunnelInfo = (from m in keyJsonInfo.t
                                      where m.tid == productInfo.WaresId
                                      select m).ToList<KeyTunnelModel>();
                    if (tunnelInfo.Count > 0)
                    {
                        productInfo.Num = string.IsNullOrEmpty(tunnelInfo[0].n) ? "1" : tunnelInfo[0].n;
                        totalFee = totalFee + Convert.ToInt32(productInfo.Num) * Convert.ToDecimal(productInfo.UnitW);
                        productNames = productNames + productInfo.WaresName + ",";
                        productInfo.TradeNo = payInfo.trade_no;
                        tunnelInfo[0].p = productInfo.UnitW;

                        tunnelInfo[0].wid = productInfo.WaresId;
                        tunnelInfo[0].tid = productInfo.TunnelId;

                    }


                }


                

                // payState.ProductJson = JsonHandler.GetJsonStrFromObject(lstProductPay, false);

                //string total_fee = "1";
                //检测是否给当前页面传递了相关参数

                // 1.先取购买商品的详情返回给用户   并缓存到页面   2.支付成功后跳转到支付结果页并把缓存数据插入到销售记录表
                //若传递了相关参数，则调统一下单接口，获得后续相关接口的入口参数 

                // jsApiPay.openid = openid;
                /* 
                decimal privilegeMoney = 0;
                int weixinMoney = 0;
                if (lstPrivilege.Count > 0)
                {
                    if (lstPrivilege[0].Money>0)
                    {
                        privilegeMoney = lstPrivilege[0].Money;
                        weixinMoney = Convert.ToInt32(((totalFee - privilegeMoney) * 100));
                    } 
                    else
                    {
                        privilegeMoney = lstPrivilege[0].Discount;
                        weixinMoney = Convert.ToInt32(((totalFee) * 100)* (privilegeMoney/10));
                    }
                   
                } 
                else
                {
                    */
                if (hiPara == "-1")
                {
                    payConfig.NOTIFY_URL = PathConfig.NotidyAddr + "/Hi/PostPayResultImmediateltyW"; //结果通知方法
                    int weixinMoney = Convert.ToInt32((totalFee) * 100);
                    //}

                    payInfo.total_fee = (weixinMoney <= 0 ? 1 : weixinMoney);
                    log.Info(string.Format("total free is {0}", payInfo.total_fee));
                    payState.TotalMoney = (totalFee <= 0 ? Convert.ToDecimal(0.01) : totalFee);
                    payInfo.product_name = productNames.Length > 25 ? productNames.Substring(0, 25) : productNames;
                }
                else
                {
                    payConfig.NOTIFY_URL = PathConfig.NotidyAddr + "/Hi/PostPayResultW"; //结果通知方法
                    int weixinMoney = Convert.ToInt32((totalFee) * 100);
                    //}

                    payInfo.total_fee = Convert.ToInt32(Decimal.Parse(hiPara)*100);

                    payState.TotalMoney = Decimal.Parse(hiPara);
                    payInfo.product_name = productNames.Length > 25 ? productNames.Substring(0, 25) : productNames + "一元嗨("+hiPara +"倍)";
                }
                
                //payInfo.jsonProduct = JsonHandler.GetJsonStrFromObject(keyJsonInfo, false);
               
                //写入交易中转
                if (payInfo.total_fee == 0)
                {
                    payState.RequestState = "2";
                    payState.RequestData = "";
                    return Content(payState);
                }

                payInfo.jsonProduct = JsonHandler.GetJsonStrFromObject(keyJsonInfo, false);
                log.Info(string.Format("jsonProduct is {0}", payInfo.jsonProduct));
                log.Info(string.Format("openid is {0}", payInfo.openid));
                // FileHandler.WriteFile("data/", JsApi.payInfo.trade_no + ".wa", JsApi.payInfo.jsonProduct);

                WxPayData unifiedOrderResult = jsApi.GetUnifiedOrderResult(payInfo, payConfig);
                // Log.Write("GetDataW", "step step");
                string wxJsApiParam = jsApi.GetJsApiParameters(payConfig, payInfo);//获取H5调起JS API参数       
                payState.RequestState = "1";
                payState.RequestData = wxJsApiParam;
                payState.ProductJson = payInfo.trade_no;


                return Content(payState);

            }
            catch (Exception ex)
            {
                PayStateModel payStateError = new PayStateModel();
                payStateError.RequestState = "3";
                payStateError.RequestData = ex.Message;
                return Content(payStateError);
            }
            return Content(new PayStateModel());
        }
        
        // 微信支付结果
        public string PostPayResultImmediateltyW()
        {
            try
            {
                var request = Fycn.Utility.HttpContext.Current.Request;
                int len = (int)request.ContentLength;
                byte[] b = new byte[len];
                Fycn.Utility.HttpContext.Current.Request.Body.Read(b, 0, len);
                string postStr = Encoding.UTF8.GetString(b);
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(postStr);
                // 商户交易号
                XmlNode tradeNoNode = xmlDoc.SelectSingleNode("xml/out_trade_no");
                /*
                RedisHelper helper = new RedisHelper(0);
                
                if (!helper.KeyExists(tradeNoNode.InnerText))
                {
                    return Content(1);
                }
                */
                /*
                IMachine _imachine = new MachineService();
                if (_imachine.GetCountByTradeNo(tradeNoNode.InnerText) > 0)
                {
                    return Content(1);
                }
                */


                //支付结果
                XmlNode payResultNode = xmlDoc.SelectSingleNode("xml/result_code");
                if (payResultNode.InnerText.ToUpper() == "SUCCESS")
                {
                    /*******************************放到微信支付通知参数里，因参数只支付最大128个字符长度，所以注释修改*****************************/
                    //XmlNode tunnelNode = xmlDoc.SelectSingleNode("xml/attach");
                    //KeyJsonModel keyJsonModel = JsonHandler.GetObjectFromJson<KeyJsonModel>(tunnelNode.InnerText);
                    XmlNode attachNode = xmlDoc.SelectSingleNode("xml/attach");
                    string jsonProduct = attachNode.InnerText;//helper.StringGet(tradeNoNode.InnerText);
                    XmlNode mchIdNode = xmlDoc.SelectSingleNode("xml/mch_id"); // 商户号
                    XmlNode openidNode = xmlDoc.SelectSingleNode("xml/openid"); //买家唯一标识
                    XmlNode isSubNode = xmlDoc.SelectSingleNode("xml/is_subscribe"); // 是否为公众号关注者
                    XmlNode timeEndNode = xmlDoc.SelectSingleNode("xml/time_end"); // 是否为公众号关注者
                                                                                   //string jsonProduct = FileHandler.ReadFile("data/" + tradeNoNode.InnerText + ".wa");

                    KeyJsonModel keyJsonModel = JsonHandler.GetObjectFromJson<KeyJsonModel>(jsonProduct);
                    IMachine _imachine = new MachineService();
                    int result = _imachine.PostPayResultW(keyJsonModel, tradeNoNode.InnerText, mchIdNode.InnerText, openidNode.InnerText, isSubNode.InnerText, timeEndNode.InnerText);
                    if (result == 1)
                    {
                        List<CommandModel> lstCommand = new List<CommandModel>();
                        lstCommand.Add(new CommandModel()
                        {
                            Content = keyJsonModel.m,
                            Size = 12
                        });
                        lstCommand.Add(new CommandModel()
                        {
                            Content = tradeNoNode.InnerText,
                            Size = 22
                        });
                        lstCommand.Add(new CommandModel()
                        {
                            Content = keyJsonModel.t[0].tid,
                            Size = 5
                        });
                        lstCommand.Add(new CommandModel()
                        {
                            Content = "3",
                            Size = 1
                        });

                        //var log = LogManager.GetLogger("FycnApi", "weixin");
                        //log.Info("test");
                        //log.Info(tradeNoNode.InnerText);
                        SocketHelper.GenerateCommand(10, 41, 66, lstCommand);
                        //删除文件
                        //helper.KeyDelete(tradeNoNode.InnerText);
                        //FileHandler.DeleteFile("data/" + tradeNoNode.InnerText + ".wa");
                    }

                }
                return "<xml><return_code><![CDATA[SUCCESS]]></return_code></xml>";
            }
            catch (Exception ex)
            {
                return "<xml><return_code><![CDATA[FAIL]]></return_code></xml>";
            }

            //File.WriteAllText(@"c:\text.txt", postStr); 
        }

        // 微信支付结果
        public string PostPayResultW()
        {
            var log = LogManager.GetLogger("FycnApi", "wechat");
            try
            {
                var request = Fycn.Utility.HttpContext.Current.Request;
                int len = (int)request.ContentLength;
                byte[] b = new byte[len];
                Fycn.Utility.HttpContext.Current.Request.Body.Read(b, 0, len);
                string postStr = Encoding.UTF8.GetString(b);
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(postStr);
                // 商户交易号
                XmlNode tradeNoNode = xmlDoc.SelectSingleNode("xml/out_trade_no");

             

                /*
                IMachine _imachine = new MachineService();
                if (_imachine.GetCountByTradeNo(tradeNoNode.InnerText) > 0)
                {
                    return Content(1);
                }
                */


                //支付结果
                XmlNode payResultNode = xmlDoc.SelectSingleNode("xml/result_code");
                if (payResultNode.InnerText.ToUpper() == "SUCCESS")
                {
                    /*******************************放到微信支付通知参数里，因参数只支付最大128个字符长度，所以注释修改*****************************/
                    //XmlNode tunnelNode = xmlDoc.SelectSingleNode("xml/attach");
                    //KeyJsonModel keyJsonModel = JsonHandler.GetObjectFromJson<KeyJsonModel>(tunnelNode.InnerText);
                    XmlNode attachNode = xmlDoc.SelectSingleNode("xml/attach");
                    string jsonProduct = attachNode.InnerText;//helper.StringGet(tradeNoNode.InnerText);
                    XmlNode mchIdNode = xmlDoc.SelectSingleNode("xml/mch_id"); // 商户号
                    XmlNode openidNode = xmlDoc.SelectSingleNode("xml/openid"); //买家唯一标识
                    XmlNode isSubNode = xmlDoc.SelectSingleNode("xml/is_subscribe"); // 是否为公众号关注者
                    XmlNode timeEndNode = xmlDoc.SelectSingleNode("xml/time_end"); // 是否为公众号关注者
                    XmlNode feeNode = xmlDoc.SelectSingleNode("xml/total_fee"); // 订单金额 单位为分
                                                                                   //string jsonProduct = FileHandler.ReadFile("data/" + tradeNoNode.InnerText + ".wa");
                                                                                   //log.Info("nnnnnnn" + tradeNoNode.InnerText);
                                                                                   //log.Info("aaaaaaa"+retProducts);
                    KeyJsonModel keyJsonModel = JsonHandler.GetObjectFromJson<KeyJsonModel>(jsonProduct);
                    if(keyJsonModel!=null && keyJsonModel.t.Count>0)
                    {
                        keyJsonModel.t[0].p = (Convert.ToInt32(feeNode.InnerText) / 100).ToString();
                    }
                    IHi _ihi = new HiService();
                    _ihi.PostPayResultW(keyJsonModel, tradeNoNode.InnerText, mchIdNode.InnerText, openidNode.InnerText, isSubNode.InnerText, timeEndNode.InnerText);
                    List<ActivityPrivilegeRelationModel> lstPrivilegeRelation = _ihi.IsSupportActivity(keyJsonModel.m);
                    if(lstPrivilegeRelation.Count == 0) //未摇中
                    {
                        _ihi.DoReward(keyJsonModel, tradeNoNode.InnerText, openidNode.InnerText, false);
                    }
                    else
                    {
                        if(IsReward(lstPrivilegeRelation[0], keyJsonModel.t[0], feeNode.InnerText))
                        {
                            //中奖
                            _ihi.DoReward(keyJsonModel, tradeNoNode.InnerText, openidNode.InnerText, true);
                        }
                        else
                        { //未摇中
                            _ihi.DoReward(keyJsonModel, tradeNoNode.InnerText, openidNode.InnerText, false);
                        }
                    }

                    // int result = _iwechat.PostPayResultW(lstProductPay, mchIdNode.InnerText, openidNode.InnerText, isSubNode.InnerText, timeEndNode.InnerText, jsonProduct);


                }
                return "<xml><return_code><![CDATA[SUCCESS]]></return_code></xml>";
            }
            catch (Exception ex)
            {
                //log.Info("bbbb" + ex.Message);
                return "<xml><return_code><![CDATA[FAIL]]></return_code></xml>";
            }

            //File.WriteAllText(@"c:\text.txt", postStr); 
        }

        

        //根据机器id取机器位置
        public ResultObj<MachineLocationModel> GetMachineLocationById(string machineId)
        {
            if(string.IsNullOrEmpty(machineId))
            {
                return null;
            }
            IHi ihi = new HiService();
            var lstMachines = ihi.GetMachineLocationById(machineId);
            if (lstMachines.Count > 0)
            {
                return Content(lstMachines[0]);
            }
            else
            {
                return null;
            }
        }

        //取广告图片
        public ResultObj<List<SourceToMachineModel>> GetAd(string machineId)
        {

            IHi ihi = new HiService();
            return Content(ihi.GetAdSource(machineId,"3"));
        }

        public ResultObj<bool> IsSupportActivity(string machineId)
        {
            IHi ihi = new HiService();
            var lstResult = ihi.IsSupportActivity(machineId);
            if (lstResult.Count == 0)
            {
                return Content(false);
            }
            else
            {
                return Content(true);
            }
        }

        //概率计算，是否摇中
        private bool IsReward(ActivityPrivilegeRelationModel privilegeRelation, KeyTunnelModel tunnelInfo, string fee)
        {
            if(Convert.ToInt32(fee)>=decimal.Parse(tunnelInfo.p)*100)
            {
                return false;
            }

            Random ran = new Random();
            int RandKey = ran.Next(0, 10000);
            decimal result = ((Convert.ToInt32(fee) * 100) / decimal.Parse(tunnelInfo.p));
            if (privilegeRelation.Rate!=0)
            {
                result = result * privilegeRelation.Rate;
            }
           
            if(RandKey<result)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        [HttpPost]
        public ResultObj<int> GetTradeStatusByTradeNo(string tradeNo)
        {
            IHi ihi = new HiService();
            List<SaleModel> lstSale = ihi.GetTradeStatusByTradeNo(tradeNo);
            if(lstSale.Count==0)
            {
                return Content(0); //等待
            }
            if(lstSale[0].TradeStatus==10)
            {
                return Content(2);//未嗨中
            }
            if(lstSale[0].TradeStatus == 7)
            {
                return Content(1);//嗨中了
            }

            return Content(3);//其他错误
        }


        //取出待取货的取货卡列表
        public ResultObj<List<ClientSalesRelationModel>> GetWaitingPickupByMachine(string machineId,string openId)
        {
            if(string.IsNullOrEmpty(machineId)||string.IsNullOrEmpty(openId))
            {
                return null;
            }
            IHi ihi = new HiService();
            List<ClientSalesRelationModel> lstClientSales = ihi.GetWaitingPickupByMachine(machineId, openId);

            return Content(lstClientSales);
        }

        //立即取货
        [HttpPost]
        public ResultObj<int> PickupImmediately(string tradeNo)
        {
            if(string.IsNullOrEmpty(tradeNo))
            {
                return Content(0);
            }
            IHi ihi = new HiService();
            List<ClientSalesRelationModel>  lstClientSales = ihi.VerifyPickupByTradeNo(tradeNo);

            if(lstClientSales==null || lstClientSales.Count==0 || lstClientSales[0].CodeStatus!=1)
            {
                return Content(0);
            }
            ClientSalesRelationModel salesInfo = lstClientSales[0];
            var lstProduct = ihi.GetProducInfoByWaresId(salesInfo.MachineId, salesInfo.WaresId);
            if (lstProduct == null || lstProduct.Count == 0)
            {
                return Content(0);
            }
            List<CommandModel> lstCommand = new List<CommandModel>();
            lstCommand.Add(new CommandModel()
            {
                Content = salesInfo.MachineId,
                Size = 12
            });
            lstCommand.Add(new CommandModel()
            {
                Content = salesInfo.TradeNo,
                Size = 22
            });
            lstCommand.Add(new CommandModel()
            {
                Content = lstProduct[0].TunnelId,
                Size = 5
            });
            lstCommand.Add(new CommandModel()
            {
                Content = "3",
                Size = 1
            });

            //var log = LogManager.GetLogger("FycnApi", "weixin");
            //log.Info("test");
            //log.Info(tradeNoNode.InnerText);
            SocketHelper.GenerateCommand(10, 41, 66, lstCommand);

            return Content(1);
        }
    }
}