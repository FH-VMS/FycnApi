using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fycn.SqlDataAccess
{
    public enum CommonSqlKey
    {
        Null,
        Sql,
        GetUser,
        GetUserCount,
        UpdateUser,
        DeleteUser,
        GetLogin,
        GetClientDic,
        GetAuthDic,
        CheckUserExist,

        //用户设置
        ChangePassword,

        UpdateCustomer,
        DeleteCustomer,
        GetCustomer,
        GetCustomerCount,
        GetCustomerForSys,
        GetCustomerCountForSys,
        UpdateChildCustomer,

        //权限
        GetAuthCount,
        DeleteAuth,
        UpdateAuth,
        GetAuth,
        GetAuthByDmsId,
        GetMenuByUser,
        DeleteAuthRelate,
        GetRankValue,

        //公用模块
        GetDic,
        GetRank,
        GetUserByClientId,
        GetMachineDic,
        GetMachineDicCount,
        GetCabinetByMachineId,
        GetPictureDic,
        GetProductDic,
        GetCabinetDic,
        GetProductDicAll,
        CheckMachineId,
        GetMachineNameById,
        GetPayConfigDic,
        GetAdDic,
        GetProductTypeDic,

        //机型
        GetMachineType,
        GetMachineTypeCount,
        DeleteMachineType,
        UpdateMachineType,
        GetCabinetByMachineTypeId,
        DeleteMachineTypeAndCabinet,
        //机柜
        GetMachineCabinet, 
        GetCabinetCount,
        DeleteMachineCabinet,
        UpdateMachineCabinet,

        //机器列表
        GetMachineList,
        GetMachineListCount,
        DeleteMachineList,
        UpdateMachineList,
        GetMachineTypeDic,
        GetMachineCountWithStatus,

        //机器配置
        GetMachineConfig,
        GetMachineConfigCount,
        DeleteMachineConfig,
        UpdateMachineConfig,

        //货道配置
        GetTunnelConfigCount,
        GetTunnelConfig,
        GetCabinetConfig,
        DeleteTunnelConfig,
        UpdateTunnelConfig,
        GetPriceByWaresId,
        UpdateTunnelPrice,
        GetWaresByTunnel,

        //商品列表
        GetProductList,
        GetProductListCount,
        DeleteProductList,
        UpdateProductList,
        GetProductAllList,
        GetProductListAllCount,
        GetProductNameByWaresId,

        //商品类型
        GetProductType,
        GetProductTypeCount,
        DeleteProductType,
        UpdateProductType,
        GetProdcutTypeByMachine,

        //商品配置
        GetProductConfigAll,
        GetProductConfig,
        GetProductConfigAllCount,
        GetProductConfigCount,
        DeleteProductConfig,
        UpdateProductConfig,

        //图片资源
        GetPictureList,
        GetPictureListCount,
        UpdatePictureList,
        DeletePictureList,
        GetAdSource,

        //销售
        UpdateSale,
        GetPayResultById,
        DeleteSaleList,
        GetSaleAllList,
        GetSaleList,
        GetSaleListAllCount,
        GetSaleListCount,
        GetClientIds,
        GetChildAndParentIds,
        GetClientParentIds,
        GetRefundDetail,

        //现金销售
        GetCashSaleList,
        GetCashSaleCount,

        //投币器
        GetCashEquipmentList,
        GetCashEquipmentCount,
        UpdateCashStatus,
        UpdateCashStock,
        UpdateCoinStatus,
        UpdateCoinStock,
        DeleteCashSale,
        IsExistEquipmentInfo,

        //货道信息
        GetTunnelInfoCount,
        GetTunnelInfo,
        DeleteTunnelInfo,
        UpdateTunnelInfo,
        UpdateTunnelConfigStatus,
        GenerateFullfilBill,
        GetFullfilCount,
        UpdateTunnelCurrStock,
        ExportByTunnel,
        ExportByProduct,

        //机器对应接口
        GetProductByMachine,
        GetProductByMachineCount,
        GetProductAndroid, //安卓取商品列表  没有货道和库存限制
        GetProductAndroidCount,
        GetCountByTradeNo,
        UpdatePayResult,
        DeleteTunnelStatusByMachine,
        FullfilGoodsOneKey,
        IsExistTunnelInfo,
        UpdateFullfilGoodsOneKey,
        DeleteTunnelStatusByMachineAndTunnel,
        GetBeepHeart,
        DeleteToMachine,
        ToMachinePrice,
        ToMachineStock,
        PostPriceAndMaxStock,
        GetMachineSetting,
        UpdateCurrStock,
        UpdateAddCurrStock,
        UpdateMaxPuts,
        UpdatePrice,
        UpdateMachineInlineTime,
        UpdateMachineInlineTimeAndIpv4,
        GetCabinetId,
        GetSalesByNo,
        UpdateCashPrice,
        //支付对应接口
        GetProductInfo,
        GetProductInfoByWaresId,
        GetRefundData,
        UpdateRefundResult,
        UpdateOrderStatusForAli,
        IsRefundSucceed,
        GetMachineByMachineId,
        GetIpByMachineId,
        GetPayConfig,
        GetPayConfigByClientId,

        //总额及提现记录
        GetTotalMoneyByClient,
        //统计
        GetSalesAmountByMachine,
        GetSalesAmountByMachineCount,
        GetStatisticSalesMoneyByDate,
        GetPayNumbers,
        GetGroupSalesMoney,
        GetGroupProduct,
        GetGroupMoneyByMachine,
        GetMobilePayStatistic,
        GetProductStatistic,
        GetProductStatisticCount,

        //支付配置
        GetPayConfigList,
        GetPayConfigListCount,
        DeletePayConfig,
        UpdatePayConfig,
        UpdateWxCert,
        GetWxConfigByMchId,

        //资源
        GetResourceById,
        GetAd,
        DeleteAdRelation,
        DeleteAd,
        UpdateAd,
        GetRelationByIdAndType,

        //复制机器
        GetTunnelConfigById,
        GetTunnelStatusById,
        GetCopyMachineById,
        GetMachineConfigById,

        //公众号
        IsExistMember
    }
}
