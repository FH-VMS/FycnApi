using System;
using System.IO;

namespace Fycn.PaymentLib.ali
{
    /// <summary>
    /// 类名：Config
    /// 功能：基础配置类
    /// 详细：设置帐户有关信息及返回路径
    /// 版本：3.4
    /// 修改日期：2016-03-08
    /// 说明：
    /// 以下代码只是为了方便商户测试而提供的样例代码，商户可以根据自己网站的需要，按照技术文档编写,并非一定要使用该代码。
    /// 该代码仅供学习和研究支付宝接口使用，只是提供一个参考。
    /// </summary>
    public class Config
    {
        //↓↓↓↓↓↓↓↓↓↓请在这里配置您的基本信息↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓
        /*
        // 合作身份者ID，签约账号，以2088开头由16位纯数字组成的字符串，查看地址：https://b.alipay.com/order/pidAndKey.htm
        public string partner = "2088621838347114";

        // 收款支付宝账号，以2088开头由16位纯数字组成的字符串，一般情况下收款账号就是签约账号
        public string seller_id = partner;

        // MD5密钥，安全检验码，由数字和字母组成的32位字符串，查看地址：https://b.alipay.com/order/pidAndKey.htm
        public string key = "7bvs5ke7to0m6064mt7tkcm1gafo9qjf";
        */
        // 合作身份者ID，签约账号，以2088开头由16位纯数字组成的字符串，查看地址：https://b.alipay.com/order/pidAndKey.htm
        public string partner = "";

        // 收款支付宝账号，以2088开头由16位纯数字组成的字符串，一般情况下收款账号就是签约账号
        public string seller_id = "";

        // MD5密钥，安全检验码，由数字和字母组成的32位字符串，查看地址：https://b.alipay.com/order/pidAndKey.htm
        public string key = "";


        // 服务器异步通知页面路径，需http://格式的完整路径，不能加?id=123这类自定义参数,必须外网可以正常访问
        public string notify_url = PathConfig.NotidyAddr+"/Machine/PostPayResultA";//Path.PathConfig + "/m.html#/payresult";

        // 页面跳转同步通知页面路径，需http://格式的完整路径，不能加?id=123这类自定义参数，必须外网可以正常访问
        public string return_url = PathConfig.DomainConfig + "/m.html#/payresult";

        // 签名方式
        public string sign_type = "MD5";

        // 调试用，创建TXT日志文件夹路径，见AlipayCore.cs类中的LogResult(string sWord)打印方法。
        public string log_path = Directory.GetCurrentDirectory() + "log/";

        // 字符编码格式 目前支持utf-8
        public string input_charset = "utf-8";

        // 支付类型 ，无需修改
        public string payment_type = "1";

        // 调用的接口名，无需修改
        public string service = "alipay.wap.create.direct.pay.by.user";

        // 调用的接口名，无需修改
        public string GateWay = "https://mapi.alipay.com/gateway.do?";

        //↑↑↑↑↑↑↑↑↑↑请在这里配置您的基本信息↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑

        /*************************************退款接口配置************************************/
        //实现异步http://www.cnblogs.com/wintersun/archive/2013/01/11/2856541.html

        /*****************************************退款配置**********************************************/
        public string refund_appid = "2017042106871270";

        public string rsa_sign = "MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQDGPgmxK5YV0/fH939lHFxg6lgTqByC9Hom3re4gnphpYFtWKLTvTo8b3YTUMKlmRUsF63MtUrrFfjND9s43XzIiEKj7R12qye8X76/RZfbmvYHF7JVE14KnucBOnQ+hlqf7PskJbQa7+1n3IjknY8iRBHrnGKSDW9iTmCjeHiexwIDAQAB";

        public string refund_notify_url = PathConfig.NotidyAddr + "/Refund/PostRefundResultA"; //退款通知

        // 退款日期 时间格式 yyyy-MM-dd HH:mm:ss
        public string refund_date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        // 调试用，创建TXT日志文件夹路径，见AlipayCore.cs类中的LogResult(string sWord)打印方法。
        public string refund_log_path = Directory.GetCurrentDirectory() + "log/";

        // 字符编码格式 目前支持 gbk 或 utf-8
        public string refund_input_charset = "utf-8";

        public string refund_sign_type = "RSA";

        // 调用的接口名，无需修改
        public string refund_service = "refund_fastpay_by_platform_nopwd";



        /**************************************新的api字段************************************/
        // 应用ID,您的APPID
        public string new_app_id = "2017042106871270";
        // 支付宝网关
        public string new_gatewayUrl = "https://openapi.alipay.com/gateway.do";

        // 商户私钥，您的原始格式RSA私钥
        public string private_key = "MIICXAIBAAKBgQDGPgmxK5YV0/fH939lHFxg6lgTqByC9Hom3re4gnphpYFtWKLTvTo8b3YTUMKlmRUsF63MtUrrFfjND9s43XzIiEKj7R12qye8X76/RZfbmvYHF7JVE14KnucBOnQ+hlqf7PskJbQa7+1n3IjknY8iRBHrnGKSDW9iTmCjeHiexwIDAQABAoGBAIguPcIzStqbze7UGfN/VAZPdUmrhkp/XxosjNB28VL6uro+1TvXFZZGizohlFTloCG18nJZZ6muYkebyOB7Zie1wqnFHPWxptjptwBZbS149gXl721glKpV5bI4PtmMuyHXLOBuIzjKs8am1vs+T7Gprm7AIvPT9MGJCBr9B7qBAkEA5Yk22pCaHd1/a0aWqNzlLu3po5X1Ocfz200+a3hhKI6f7NpVuzxQQemMSyP8UFDPgjOmbGPssJppLC0FAaqJkQJBAN0ZL+NznvFAzpFvqbCxMDS9igph6UFREAoiUu91ULAtUCpCkBODmkoLrKbm7mm0PF4yeLD5dc7f0HRfIYRSttcCQFI42MFqUwqnsWEIJCfRGPe6mZrTuMg97Ah+nwF4WbVhgcAiZdtwO3+g3XR9K4DJsct+HPtuv/ZzGYGNjuGN6UECQG0q7YlJ4nXOgPAwiUG0C1BPMeR0eb6Fbv0B+58+dqu2g/mJyifIeBsNbp8uMRPCKXh9RThkw/V0bzG2cw8p5BsCQBpG9GAOD59gJp+5BEJxpZa9oCxFfIyIGGYfyRxPiCJFwNUz+MyaHjq1DgFJY8WiicZoKg37Vzalff6MTixDVIc=";

        // 支付宝公钥,查看地址：https://openhome.alipay.com/platform/keyManage.htm 对应APPID下的支付宝公钥。
        public string alipay_public_key = "MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQDGPgmxK5YV0/fH939lHFxg6lgTqByC9Hom3re4gnphpYFtWKLTvTo8b3YTUMKlmRUsF63MtUrrFfjND9s43XzIiEKj7R12qye8X76/RZfbmvYHF7JVE14KnucBOnQ+hlqf7PskJbQa7+1n3IjknY8iRBHrnGKSDW9iTmCjeHiexwIDAQAB";

        // 签名方式
        public string new_sign_type = "RSA";

        // 编码格式
        public string new_charset = "UTF-8";


    }
}