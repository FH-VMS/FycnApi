using Fycn.Interface;
using Fycn.Model.Machine;
using Fycn.Model.Product;
using Fycn.SqlDataAccess;
using Fycn.Utility;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fycn.Service
{
    public class ProductTypeService : AbstractService, IBase<ProductTypeModel>
    {
        public List<ProductTypeModel> GetAll(ProductTypeModel productTypeInfo)
        {
            string userClientId = HttpContextHandler.GetHeaderObj("UserClientId").ToString();
            if (string.IsNullOrEmpty(userClientId))
            {
                return null;
            }
            var conditions = new List<Condition>();
            string clientIds = new CommonService().GetClientIds(userClientId);
            if (clientIds.Contains("self"))
            {
                clientIds = "'" + clientIds.Replace(",", "','") + "'";
            }
            conditions.Add(new Condition
            {
                LeftBrace = " AND ",
                ParamName = "ClientId",
                DbColumnName = "a.client_id",
                ParamValue = clientIds,
                Operation = ConditionOperate.INWithNoPara,
                RightBrace = "",
                Logic = ""
            });
            if (!string.IsNullOrEmpty(productTypeInfo.WaresTypeName))
            {
                conditions.Add(new Condition
                {
                    LeftBrace = " AND ",
                    ParamName = "WaresTypeName",
                    DbColumnName = "a.wares_type_name",
                    ParamValue = "%" + productTypeInfo.WaresTypeName + "%",
                    Operation = ConditionOperate.Like,
                    RightBrace = "",
                    Logic = ""
                });
            }

            if (!string.IsNullOrEmpty(productTypeInfo.ClientName))
            {
                conditions.Add(new Condition
                {
                    LeftBrace = " AND ",
                    ParamName = "ClientName",
                    DbColumnName = "b.client_name",
                    ParamValue = "%" + productTypeInfo.ClientName + "%",
                    Operation = ConditionOperate.Like,
                    RightBrace = "",
                    Logic = ""
                });
            }
            
            conditions.Add(new Condition
            {
                LeftBrace = "  ",
                ParamName = "Sequence",
                DbColumnName = "a.sequence",
                ParamValue = "asc",
                Operation = ConditionOperate.OrderBy,
                RightBrace = "",
                Logic = ""
            });

            conditions.AddRange(CreatePaginConditions(productTypeInfo.PageIndex, productTypeInfo.PageSize));

            return GenerateDal.LoadByConditions<ProductTypeModel>(CommonSqlKey.GetProductType, conditions);
        }


        public int GetCount(ProductTypeModel productTypeInfo)
        {
            var result = 0;

            string userClientId = HttpContextHandler.GetHeaderObj("UserClientId").ToString();
            if (string.IsNullOrEmpty(userClientId))
            {
                return 0;
            }

            var conditions = new List<Condition>();
            string clientIds = new CommonService().GetClientIds(userClientId);
            if (clientIds.Contains("self"))
            {
                clientIds = "'" + clientIds.Replace(",", "','") + "'";
            }
            conditions.Add(new Condition
            {
                LeftBrace = " AND ",
                ParamName = "ClientId",
                DbColumnName = "a.client_id",
                ParamValue = clientIds,
                Operation = ConditionOperate.INWithNoPara,
                RightBrace = "",
                Logic = ""
            });
            if (!string.IsNullOrEmpty(productTypeInfo.WaresTypeName))
            {
                conditions.Add(new Condition
                {
                    LeftBrace = " AND ",
                    ParamName = "WaresTypeName",
                    DbColumnName = "a.wares_type_name",
                    ParamValue = "%" + productTypeInfo.WaresTypeName + "%",
                    Operation = ConditionOperate.Like,
                    RightBrace = "",
                    Logic = ""
                });
            }

            if (!string.IsNullOrEmpty(productTypeInfo.ClientName))
            {
                conditions.Add(new Condition
                {
                    LeftBrace = " AND ",
                    ParamName = "ClientName",
                    DbColumnName = "b.client_name",
                    ParamValue = "%" + productTypeInfo.ClientName + "%",
                    Operation = ConditionOperate.Like,
                    RightBrace = "",
                    Logic = ""
                });
            }



            result = GenerateDal.CountByConditions(CommonSqlKey.GetProductTypeCount, conditions);

            return result;
        }


        /// <summary>
        /// 新增/修改会员信息
        /// </summary>
        /// <param name="memberInfo"></param>
        /// <returns></returns>
        public int PostData(ProductTypeModel productTypeInfo)
        {
            int result;

            string userClientId = HttpContextHandler.GetHeaderObj("UserClientId").ToString();
            productTypeInfo.WaresTypeId = Guid.NewGuid().ToString();
            productTypeInfo.ClientId = userClientId;
            result = GenerateDal.Create(productTypeInfo);

            return result;
        }

        /// <summary>
        /// 删除用户
        /// </summary>
        /// <returns></returns>
        public int DeleteData(string id)
        {

            ProductTypeModel productTypeInfo = new ProductTypeModel();
            productTypeInfo.WaresTypeId = id;
            return GenerateDal.Delete<ProductTypeModel>(CommonSqlKey.DeleteProductType, productTypeInfo);
        }

        public int UpdateData(ProductTypeModel productTypeInfo)
        {
            return GenerateDal.Update(CommonSqlKey.UpdateProductType, productTypeInfo);
        }
    }
}
