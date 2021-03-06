﻿using Fycn.Interface;
using Fycn.Model.Product;
using Fycn.Model.Sale;
using Fycn.SqlDataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fycn.Utility;

namespace Fycn.Service
{
    public class ProductListService : AbstractService, IBase<ProductListModel>
    {

        public List<ProductListModel> GetAll(ProductListModel productListInfo)
        {
            string userClientId = HttpContextHandler.GetHeaderObj("UserClientId").ToString();
            if (string.IsNullOrEmpty(userClientId))
            {
                return null;
            }
            string clientIds = new CommonService().GetClientIds(userClientId);
            if (clientIds.Contains("self"))
            {
                clientIds = "'" + clientIds.Replace(",", "','") + "'";
            }
            var result = new List<ProductListModel>();
            var conditions = new List<Condition>();
            conditions.Add(new Condition
            {
                LeftBrace = " AND ",
                ParamName = "ClientId",
                DbColumnName = "a.client_id",
                ParamValue = clientIds,
                Operation = ConditionOperate.INWithNoPara,
                RightBrace = " ",
                Logic = ""
            });
            if (!string.IsNullOrEmpty(productListInfo.WaresName))
            {
                conditions.Add(new Condition
                {
                    LeftBrace = " AND ",
                    ParamName = "WaresName",
                    DbColumnName = "a.wares_name",
                    ParamValue = "%" + productListInfo.WaresName + "%",
                    Operation = ConditionOperate.Like,
                    RightBrace = "",
                    Logic = ""
                });
            }

            if (!string.IsNullOrEmpty(productListInfo.WaresTypeId))
            {
                conditions.Add(new Condition
                {
                    LeftBrace = " AND ",
                    ParamName = "WaresTypeId",
                    DbColumnName = "a.wares_type_id",
                    ParamValue =  productListInfo.WaresTypeId ,
                    Operation = ConditionOperate.Equal,
                    RightBrace = "",
                    Logic = ""
                });
            }

            conditions.AddRange(CreatePaginConditions(productListInfo.PageIndex, productListInfo.PageSize));

           
           result = GenerateDal.LoadByConditions<ProductListModel>(CommonSqlKey.GetProductAllList, conditions);
           





            return result;
        }


        public int GetCount(ProductListModel productListInfo)
        {
            var result = 0;

            string userClientId = HttpContextHandler.GetHeaderObj("UserClientId").ToString();
            if (string.IsNullOrEmpty(userClientId))
            {
                return 0;
            }
            string clientIds = new CommonService().GetClientIds(userClientId);
            if (clientIds.Contains("self"))
            {
                clientIds = "'" + clientIds.Replace(",", "','") + "'";
            }
            var conditions = new List<Condition>();
            conditions.Add(new Condition
            {
                LeftBrace = " AND ",
                ParamName = "ClientId",
                DbColumnName = "client_id",
                ParamValue = clientIds,
                Operation = ConditionOperate.INWithNoPara,
                RightBrace = " ",
                Logic = ""
            });
            if (!string.IsNullOrEmpty(productListInfo.WaresName))
            {
                conditions.Add(new Condition
                {
                    LeftBrace = " AND ",
                    ParamName = "WaresName",
                    DbColumnName = "wares_name",
                    ParamValue = "%" + productListInfo.WaresName + "%",
                    Operation = ConditionOperate.Like,
                    RightBrace = "",
                    Logic = ""
                });
            }

            if (!string.IsNullOrEmpty(productListInfo.WaresTypeId))
            {
                conditions.Add(new Condition
                {
                    LeftBrace = " AND ",
                    ParamName = "WaresTypeId",
                    DbColumnName = "wares_type_id",
                    ParamValue = productListInfo.WaresTypeId,
                    Operation = ConditionOperate.Equal,
                    RightBrace = "",
                    Logic = ""
                });
            }
            
            result = GenerateDal.CountByConditions(CommonSqlKey.GetProductListAllCount, conditions);

            return result;
        }


        /// <summary>
        /// 新增/修改会员信息
        /// </summary>
        /// <param name="memberInfo"></param>
        /// <returns></returns>
        public int PostData(ProductListModel productListInfo)
        {
            int result;
            string userClientId = HttpContextHandler.GetHeaderObj("UserClientId").ToString();
            if (string.IsNullOrEmpty(userClientId))
            {
                return 0;
            }
            string userAccount = HttpContextHandler.GetHeaderObj("UserAccount").ToString();
            productListInfo.WaresId = Guid.NewGuid().ToString();
            productListInfo.Creator = userAccount;
            productListInfo.UpdateDate = DateTime.Now;
            productListInfo.ClientId = userClientId;
            result = GenerateDal.Create(productListInfo);




            return result;
        }

        /// <summary>
        /// 删除用户
        /// </summary>
        /// <returns></returns>
        public int DeleteData(string id)
        {

           
                ProductListModel productListInfo = new ProductListModel();
                productListInfo.WaresId = id;
                return GenerateDal.Delete<ProductListModel>(CommonSqlKey.DeleteProductList, productListInfo);

            
        }

        public int UpdateData(ProductListModel productListInfo)
        {
            productListInfo.UpdateDate = DateTime.Now;
            string userAccount = HttpContextHandler.GetHeaderObj("UserAccount").ToString();
            productListInfo.Creator = userAccount;
            productListInfo.UpdateDate = DateTime.Now;
            return GenerateDal.Update(CommonSqlKey.UpdateProductList, productListInfo);
        }

       
    }
}
