using FycnApi.Base;
using Fycn.Interface;
using Fycn.Model.Pay;
using Fycn.Model.Sys;
using Payment.wx;
using Fycn.PaymentLib.ali;
using Fycn.PaymentLib.wx;
using Fycn.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using Fycn.Utility;
using Alipay.AopSdk.Core;
using Alipay.AopSdk.Core.Domain;
using Alipay.AopSdk.Core.Request;
using Alipay.AopSdk.Core.Response;
using log4net;
using System.Web;

namespace FycnApi.Controllers
{
    public class PaymentController : ApiBaseController
    {
        public string PostPayNow()
        {
            //NativePay nativePay = new NativePay();


            //生成扫码支付模式二url
            //string url2 = nativePay.GetPayUrl("123456789");

            return null;
        }

        #region 按货道购买商品

        //微信支付
        public ResultObj<PayStateModel> GetDataW(string k, string code)
        {
            bool isPayByTunnel = true;
            try
            {
              
            //解码机器传过来的key值
            //解析k值
            KeyJsonModel keyJsonInfo = AnalizeKey(k);
            RedisHelper redisHelper=new RedisHelper(0);
            if(!redisHelper.KeyExists(keyJsonInfo.m))
            {
                PayStateModel payStateNull = new PayStateModel();
                return Content(payStateNull,ResultCode.Success, "机器不在线", new Pagination { });
            }
            //移动支付赋值
            WxPayConfig payConfig =GenerateConfigModelW(keyJsonInfo.m);
                JsApi jsApi = new JsApi();
                PayModel payInfo = new PayModel();
                //JsApi.payInfo = new PayModel();
                payInfo.k = k;
                //生成code 根据code取微信支付的openid和access_token
                jsApi.GetOpenidAndAccessToken(code, payConfig,payInfo);
           
            PayStateModel payState = new PayStateModel();
            if (string.IsNullOrEmpty(payInfo.openid))
            {
                payState.RequestState = "0";
                payState.ProductJson = "";
                payState.RequestData = payInfo.redirect_url;
                return Content(payState);
            }

            //JSAPI支付预处理
            
               
                if (string.IsNullOrEmpty(keyJsonInfo.m) || keyJsonInfo.t.Count == 0)
                {
                    payState.RequestState = "2";
                    payState.ProductJson = "";
                    payState.RequestData = "";
                    return Content(payState);
                }
                //生成交易号
                payInfo.trade_no = GeneraterTradeNo();
                //取商品信息
                IPay _ipay = new PayService();

                decimal totalFee = 0;
                string productNames = string.Empty;
                List<ProductModel> lstProduct = new List<ProductModel>();
                if (keyJsonInfo.t.Count > 0 && keyJsonInfo.t[0].tid.Length > 10)
                {
                    isPayByTunnel = false;
                }
            
                //按货道付款
                if (isPayByTunnel)
                {
                    lstProduct = _ipay.GetProducInfo(keyJsonInfo.m, keyJsonInfo.t);
                    //遍历商品
                    foreach (ProductModel productInfo in lstProduct)
                    {
                        var tunnelInfo = (from m in keyJsonInfo.t
                                          where m.tid == productInfo.TunnelId
                                          select m).ToList<KeyTunnelModel>();
                        if (tunnelInfo.Count > 0)
                        {
                            productInfo.Num = string.IsNullOrEmpty(tunnelInfo[0].n) ? "1" : tunnelInfo[0].n;
                            totalFee = totalFee + Convert.ToInt32(productInfo.Num) * Convert.ToDecimal(productInfo.UnitW);
                            productNames = productNames + productInfo.WaresName + ",";
                            productInfo.TradeNo = payInfo.trade_no;
                            tunnelInfo[0].p = productInfo.UnitW;
                            tunnelInfo[0].wid = productInfo.WaresId;

                        }
                    }
                }
                else
                {
                    lstProduct = _ipay.GetProducInfoByWaresId(keyJsonInfo.m, keyJsonInfo.t);
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
                }

                
                payInfo.product_name = productNames.Length > 25 ? productNames.Substring(0, 25) : productNames;
                
                //string total_fee = "1";
                //检测是否给当前页面传递了相关参数

                // 1.先取购买商品的详情返回给用户   并缓存到页面   2.支付成功后跳转到支付结果页并把缓存数据插入到销售记录表
                //若传递了相关参数，则调统一下单接口，获得后续相关接口的入口参数 

                // jsApiPay.openid = openid;

                payInfo.total_fee = Convert.ToInt32((totalFee * 100));
                payInfo.jsonProduct = JsonHandler.GetJsonStrFromObject(keyJsonInfo, false);

                //写入交易中转
                /*
                RedisHelper helper = new RedisHelper(0);
                
                helper.StringSet(JsApi.payInfo.trade_no.Trim(), JsApi.payInfo.jsonProduct, new TimeSpan(0, 10, 30));
                */
                // FileHandler.WriteFile("data/", JsApi.payInfo.trade_no + ".wa", JsApi.payInfo.jsonProduct);

                WxPayData unifiedOrderResult = jsApi.GetUnifiedOrderResult(payInfo,payConfig);
                // Log.Write("GetDataW", "step step");
                string wxJsApiParam = jsApi.GetJsApiParameters(payConfig, payInfo);//获取H5调起JS API参数       
                payState.RequestState = "1";
                payState.ProductJson = JsonHandler.GetJsonStrFromObject(lstProduct, false);
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

      

        //对k进行解码 k格式：{"m":"ABC123456789","t":[{"tid":"1-2","n":3},{"tid":"1-3","n":2}]}
        private KeyJsonModel AnalizeKey(string key)
        {
            KeyJsonModel keyJsonInfo = null;
            try
            {
              keyJsonInfo = JsonHandler.GetObjectFromJson<KeyJsonModel>(key);
            }
            catch(Exception e) {
              keyJsonInfo = JsonHandler.GetObjectFromJson<KeyJsonModel>(System.Text.Encoding.Default.GetString(ByteHelper.strToToHexByte(key)));
            }
            

            return keyJsonInfo;
        }

        private string GeneraterTradeNo()
        {
            Random ran = new Random();
            int RandKey = ran.Next(1000, 9999);
            string out_trade_no = DateTime.Now.ToString("yyyyMMddHHmmssffff") + RandKey.ToString();
            return out_trade_no;
        }

   


        //支付宝支付
        public ResultObj<PayStateModel> GetDataA(string k, bool isPayByTunnel = true)
        {
            PayStateModel payStateModel = new PayStateModel();
        
           
            ////////////////////////////////////////////请求参数////////////////////////////////////////////
           
            //解析k值
            KeyJsonModel keyJsonInfo = AnalizeKey(k);

            if (string.IsNullOrEmpty(keyJsonInfo.m) || keyJsonInfo.t.Count == 0)
            {
                payStateModel.RequestState = "2";
                payStateModel.ProductJson = "";
                payStateModel.RequestData = "";
                return Content(payStateModel);
            }
            RedisHelper redisHelper = new RedisHelper(0);
            if(!redisHelper.KeyExists(keyJsonInfo.m))
            {
                return Content(payStateModel,ResultCode.Success, "机器不在线", new Pagination { });
            }
            //移动支付赋值
            Config config = GenerateConfigModelA(keyJsonInfo.m);
            //生成交易号
            string out_trade_no = GeneraterTradeNo();
            //取商品信息
            IPay _ipay = new PayService();

            decimal totalFee = 0;
            string productNames = string.Empty;

            List<ProductModel> lstProduct = new List<ProductModel>();
            //按货道付款
            if (isPayByTunnel)
            {
                lstProduct = _ipay.GetProducInfo(keyJsonInfo.m, keyJsonInfo.t);
                //遍历商品
                foreach (ProductModel productInfo in lstProduct)
                {
                    var tunnelInfo = (from m in keyJsonInfo.t
                                      where m.tid == productInfo.TunnelId
                                      select m).ToList<KeyTunnelModel>();
                    if (tunnelInfo.Count > 0)
                    {
                        productInfo.Num = string.IsNullOrEmpty(tunnelInfo[0].n) ? "1" : tunnelInfo[0].n;
                        totalFee = totalFee + Convert.ToInt32(productInfo.Num) * Convert.ToDecimal(productInfo.UnitA);
                        productNames = productNames + productInfo.WaresName + ",";
                        productInfo.TradeNo = out_trade_no;
                        tunnelInfo[0].p = productInfo.UnitA;
                        tunnelInfo[0].wid = productInfo.WaresId;
                    }


                }
            }
            else
            {
                lstProduct = _ipay.GetProducInfoByWaresId(keyJsonInfo.m, keyJsonInfo.t);
                //遍历商品
                foreach (ProductModel productInfo in lstProduct)
                {
                    var tunnelInfo = (from m in keyJsonInfo.t
                                      where m.tid == productInfo.WaresId
                                      select m).ToList<KeyTunnelModel>();
                    if (tunnelInfo.Count > 0)
                    {
                        productInfo.Num = string.IsNullOrEmpty(tunnelInfo[0].n) ? "1" : tunnelInfo[0].n;
                        totalFee = totalFee + Convert.ToInt32(productInfo.Num) * Convert.ToDecimal(productInfo.UnitA);
                        productNames = productNames + productInfo.WaresName + ",";
                        productInfo.TradeNo = out_trade_no;
                        tunnelInfo[0].p = productInfo.UnitA;
                        
                        tunnelInfo[0].wid = productInfo.WaresId;
                        tunnelInfo[0].tid = productInfo.TunnelId;
                        
                    }


                }
            }

            


            //订单名称，必填
            string subject = productNames.Length > 25 ? productNames.Substring(0, 25) : productNames;

            //付款金额，必填
            string total_fee = totalFee.ToString();

            //收银台页面上，商品展示的超链接，必填
            string show_url ="";

            //商品描述，可空
            string body = JsonHandler.GetJsonStrFromObject(keyJsonInfo, false);
            //写入交易中转
            //RedisHelper helper = new RedisHelper(0);
            var log = LogManager.GetLogger("FycnApi", "tradeNo");
           
            //helper.StringSet(out_trade_no.Trim(), body,new TimeSpan(0,10,30));
            // FileHandler.WriteFile("data/", out_trade_no + ".wa", body);


            ////////////////////////////////////////////////////////////////////////////////////////////////

            //把请求参数打包成数组
            /*********************老接口注释 2017年11月16日********************/
            /*
            SortedDictionary<string, string> sParaTemp = new SortedDictionary<string, string>();
            sParaTemp.Add("partner", Config.partner);
            sParaTemp.Add("seller_id", Config.seller_id);
            sParaTemp.Add("_input_charset", Config.input_charset.ToLower());
            sParaTemp.Add("service", Config.service);
            sParaTemp.Add("payment_type", Config.payment_type);
            sParaTemp.Add("notify_url", Config.notify_url);
            sParaTemp.Add("return_url", Config.return_url);
            sParaTemp.Add("out_trade_no", out_trade_no);
            sParaTemp.Add("subject", subject);
            sParaTemp.Add("total_fee", total_fee);
            sParaTemp.Add("show_url", show_url);
            sParaTemp.Add("app_pay","Y");//启用此参数可唤起钱包APP支付。
            sParaTemp.Add("body", subject);
            //其他业务参数根据在线开发文档，添加参数.文档地址:https://doc.open.alipay.com/doc2/detail.htm?spm=a219a.7629140.0.0.2Z6TSk&treeId=60&articleId=103693&docType=1
            //如sParaTemp.Add("参数名","参数值");

            //建立请求
            string sHtmlText = Config.GateWay+Submit.BuildRequestParaToString(sParaTemp, Encoding.UTF8);
            
            payStateModel.ProductJson = JsonHandler.GetJsonStrFromObject(lstProduct, false);
            payStateModel.RequestData = sHtmlText;
            payStateModel.RequestState = "1";
           
            return Content(payStateModel);
        */


            /**********************支付宝新接口添加**************************/
            DefaultAopClient client = new DefaultAopClient(config.new_gatewayUrl, config.new_app_id, config.private_key, "json", "1.0", config.new_sign_type, config.alipay_public_key, config.new_charset, false);
            AlipayTradeWapPayModel model = new AlipayTradeWapPayModel();
            model.Body = subject;
            model.Subject = subject;
            model.TotalAmount = total_fee;
            model.OutTradeNo = out_trade_no;
            model.PassbackParams = body;
             log.Info("99999-"+body);
            model.TimeoutExpress="1m";
            model.ProductCode = "";
            model.QuitUrl = "";


            AlipayTradeWapPayRequest request = new AlipayTradeWapPayRequest();
            // 设置支付完成同步回调地址
            request.SetReturnUrl(config.return_url);
            // 设置支付完成异步通知接收地址
            request.SetNotifyUrl(config.notify_url);
            // 将业务model载入到request
            request.SetBizModel(model);

            AlipayTradeWapPayResponse response = null;
            try
            {
                response = client.pageExecute(request, null, "post");
                response.OutTradeNo=out_trade_no;
                
                payStateModel.ProductJson = JsonHandler.GetJsonStrFromObject(lstProduct, false);
                payStateModel.RequestData = response.Body;
                payStateModel.RequestState = "1";
                return Content(payStateModel);
            }
            catch (Exception exp)
            {
                throw exp;
            }

            //return Content(new PayStateModel());
            
        }

        #endregion
     
        public Config GenerateConfigModelA(string machineId)
        {
            IPay ipay = new PayService();
            Config aPayConfig = new Config();
            List<ConfigModel> lstConfig = ipay.GetConfig(machineId);
            if (lstConfig.Count > 0)
            {
                ConfigModel cModel = lstConfig[0];
                aPayConfig.partner = cModel.AliParter;
                aPayConfig.key = cModel.AliKey;
                aPayConfig.seller_id = cModel.AliParter;
                aPayConfig.refund_appid = cModel.AliRefundAppId;
                aPayConfig.rsa_sign = cModel.AliRefundRsaSign;

                //新支付宝接口
                aPayConfig.new_app_id = cModel.AliAppId;
                aPayConfig.private_key = cModel.AliPrivateKey;
                aPayConfig.alipay_public_key = cModel.AliPublicKey;
                if (aPayConfig.private_key.Length > 1000)
                {
                    aPayConfig.new_sign_type = "RSA2";
                }
                else
                {
                    aPayConfig.new_sign_type = "RSA";
                }
               
            }
            return aPayConfig;
        }

        public WxPayConfig GenerateConfigModelW(string machineId)
        {
            WxPayConfig payConfig = new WxPayConfig();
            IPay ipay = new PayService();
            List<ConfigModel> lstConfig = ipay.GetConfig(machineId);
            if (lstConfig.Count > 0)
            {
                ConfigModel cModel = lstConfig[0];
               
                payConfig.APPID = cModel.WxAppId;
                payConfig.MCHID = cModel.WxMchId;
                payConfig.KEY = cModel.WxKey;
                payConfig.APPSECRET = cModel.WxAppSecret;
                payConfig.SSLCERT_PATH = cModel.WxSslcertPath;
                payConfig.SSLCERT_PASSWORD = cModel.WxSslcertPassword;
                
            }
            return payConfig;
        }

        public void AlipayNew()
        {
           
        }
    }
}