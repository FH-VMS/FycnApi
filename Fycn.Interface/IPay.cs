using Fycn.Model.Pay;
using Fycn.Model.Sys;
using Fycn.PaymentLib.ali;
using Fycn.PaymentLib.wx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fycn.Interface
{
    public interface IPay
    {
        [Remark("获取商品信息", ParmsNote = "机器编号,货道编号逗号隔开", ReturnNote = "商品信息列表")]
        List<ProductModel> GetProducInfo(string machineId, List<KeyTunnelModel> lstTunnels);

        [Remark("获取商品信息", ParmsNote = "机器编号,商品编号逗号隔开", ReturnNote = "商品信息列表")]
        List<ProductModel> GetProducInfoByWaresId(string machineId, List<KeyTunnelModel> lstTunnels);

        [Remark("获取移动支付配置", ParmsNote = "机器编号", ReturnNote = "移动支付配置实体")]
        List<ConfigModel> GetConfig(string machindId);

        [Remark("获取支付宝配置", ParmsNote = "机器编号", ReturnNote = "支付宝配置实体")]
        Config GenerateConfigModelA(string machineId);

        [Remark("获取微信配置", ParmsNote = "机器编号", ReturnNote = "微信配置实体")]
        WxPayConfig GenerateConfigModelW(string machineId);

        [Remark("获取微信配置", ParmsNote = "客户ID", ReturnNote = "微信配置实体")]
        WxPayConfig GenerateConfigModelWByClientId(string clientId);
    }
}
