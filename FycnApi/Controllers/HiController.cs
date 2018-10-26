using System;
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
            log.Info(string.Format("machine id is {0}, code is {1}, retBack is {2}", machineId, code, retBack));
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
            log.Info(string.Format("openid id is {0}", payInfo.openid));
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
            log.Info(string.Format("access token is {0}", accessToken));
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

            log.Info(string.Format("userinfo is {0}", jsonUserInfo));
            //if(iwechat.IsExistMember(memberInfo))
            //log.Info("test");

            return Content(payState);
        }


        //微信支付立即出货
        public ResultObj<PayStateModel> PostDataW(string machineId, string openId,string hiPara="-1", [FromBody]List<ProductPayModel> lstProductPay=null)
        {
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
                        wid=productPay.WaresId,
                        n="1",
                        tn = payInfo.trade_no
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


                

                payState.ProductJson = JsonHandler.GetJsonStrFromObject(lstProductPay, false);

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
                    payState.TotalMoney = (totalFee <= 0 ? Convert.ToDecimal(0.01) : totalFee);
                    payInfo.product_name = productNames.Length > 25 ? productNames.Substring(0, 25) : productNames;
                }
                else
                {
                    payConfig.NOTIFY_URL = PathConfig.NotidyAddr + "/Hi/PostPayW"; //结果通知方法
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
                // FileHandler.WriteFile("data/", JsApi.payInfo.trade_no + ".wa", JsApi.payInfo.jsonProduct);

                WxPayData unifiedOrderResult = jsApi.GetUnifiedOrderResult(payInfo, payConfig);
                // Log.Write("GetDataW", "step step");
                string wxJsApiParam = jsApi.GetJsApiParameters(payConfig, payInfo);//获取H5调起JS API参数       
                payState.RequestState = "1";
                payState.RequestData = wxJsApiParam;



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
                   

                }
                return "<xml><return_code><![CDATA[SUCCESS]]></return_code></xml>";
            }
            catch (Exception ex)
            {
                return "<xml><return_code><![CDATA[FAIL]]></return_code></xml>";
            }

            //File.WriteAllText(@"c:\text.txt", postStr); 
        }


        //检查该商品是否支付一元嗨
        public ResultObj<int> IsSupportActivity(string waresId)
        {
            return Content(0);
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

            IAdRelation _iad = new AdRelationService();
            return Content(_iad.GetAdSource(machineId));
        }
    }
}