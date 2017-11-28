using Fycn.PaymentLib.wx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payment.wx
{
    public class PayModel
    {
         /// <summary>
        /// 保存页面对象，因为要在类的方法中使用Page的Request对象
        /// </summary>

        /// <summary>
        /// openid用于调用统一下单接口
        /// </summary>
        public string openid { get; set; }

        /// <summary>
        /// access_token用于获取收货地址js函数入口参数
        /// </summary>
        public string access_token { get; set; }

        /// <summary>
        /// 商品金额，用于统一下单
        /// </summary>
        public int total_fee { get; set; }

        /// <summary>
        /// 统一下单接口返回结果
        /// </summary>
        public WxPayData unifiedOrderResult { get; set; }

        /// <summary>
        /// 返回的URL
        /// </summary>
        public string redirect_url { get; set; }

        /// <summary>
        /// 机器传过来的编码
        /// </summary>
        public string k { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        public string product_name
        {
            get;
            set;
        }

        //商户订单号
        public string trade_no
        {
            get;
            set;
        }

        //商品列表信息
        public string jsonProduct
        {
            get;
            set;
        }
    }
}
