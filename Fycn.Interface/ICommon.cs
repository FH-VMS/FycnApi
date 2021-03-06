﻿using Fycn.Model.Common;
using Fycn.Model.Resource;
using Fycn.Model.Sys;
using Fycn.Model.User;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fycn.Interface
{
    public interface ICommon
    {
        [Remark("取数据列表", ParmsNote = "当前页,每页显示行数", ReturnNote = "返回返页列表")]
        List<MenuModel> GetMenus(string userAccount);

        [Remark("登录", ParmsNote = "当前页,每页显示行数", ReturnNote = "返回返页列表")]
        UserModel PostUser(UserModel userInfo);

        [Remark("取字典方法", ParmsNote = "字典对应的key值", ReturnNote = "实体列表")]
        List<DicModel> GetDic(string id);


        [Remark("取客户等级方法", ParmsNote = "字典对应的key值", ReturnNote = "实体列表")]
        List<DicModel> GetRank(string id);

        [Remark("取客户当字典", ParmsNote = "", ReturnNote = "字典实体列表")]
        List<CommonDic> GetClientDic();

        [Remark("取权限模板当字典", ParmsNote = "", ReturnNote = "字典实体列表")]
        List<CommonDic> GetAuthDic();

        [Remark("取机型模板当字典", ParmsNote = "", ReturnNote = "字典实体列表")]
        List<CommonDic> GetMachineTypeDic();

        [Remark("根据客户ID取他的用户们", ParmsNote = "客户ID", ReturnNote = "字典实体列表")]
        List<CommonDic> GetUserByClientId(string id);

        [Remark("取机器做字典", ParmsNote = "", ReturnNote = "字典实体列表")]
        List<CommonDic> GetMachineDic(string name, int pageIndex, int pageSize);

        [Remark("取图片做字典", ParmsNote = "", ReturnNote = "字典实体列表")]
        List<CommonDic> GetPictureDic(string clinetId,string typ);

        [Remark("取商品做字典", ParmsNote = "", ReturnNote = "字典实体列表")]
        List<CommonDic> GetProductDic(string clinetId);

        [Remark("取货柜做字典", ParmsNote = "", ReturnNote = "字典实体列表")]
        List<CommonDic> GetCabinetDic();

        [Remark("修改密码", ParmsNote = "用户实体", ReturnNote = "int")]
        int UpdatePassword(UserModel userInfo);

        [Remark("取机器状态数", ParmsNote = "用户实体", ReturnNote = "string")]
        DataTable GetTotalMachineCount();

         [Remark("检查机器 是否存在", ParmsNote = "机器ID", ReturnNote = "string")]
        int CheckMachineId(string machineId, string deviceId);

         [Remark("根据机器ID取名称", ParmsNote = "机器ID", ReturnNote = "string")]
         List<CommonDic> GetMachineNameById(string machineId);

         [Remark("取支付配置字典", ParmsNote = "", ReturnNote = "字典实体列表")]
         List<CommonDic> GetPayConfigDic(string clientId);

        [Remark("取广告模板字典", ParmsNote = "", ReturnNote = "字典实体列表")]
        List<CommonDic> GetAdDic(string clientId);

        [Remark("取商品类型字典", ParmsNote = "", ReturnNote = "字典实体列表")]
        List<CommonDic> GetProductTypeDic(string clientId);

        [Remark("取名下机器数量", ParmsNote = "", ReturnNote = "int")]
        int GetMachineCountByClientId(string clientId);

        [Remark("取未过期优惠券作字典", ParmsNote = "", ReturnNote = "int")]
        List<CommonDic> GetNotExpirePrivilegeDic(string clientId);

        [Remark("重置密码", ParmsNote = "", ReturnNote = "int")]
        int ResetPassword(UserModel userInfo);

        [Remark("根据商品id取图片路径", ParmsNote = "", ReturnNote = "图片列表实体")]
        List<PictureModel> GetPicPathByWaresId(string waresId);

    }
}
