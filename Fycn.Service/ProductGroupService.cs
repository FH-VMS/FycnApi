using Fycn.Interface;
using Fycn.Model.Product;
using Fycn.SqlDataAccess;
using Fycn.Utility;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fycn.Service
{
    public class ProductGroupService : AbstractService, IBase<ProductGroupModel>
    {
        public List<ProductGroupModel> GetAll(ProductGroupModel productListInfo)
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
            var result = new List<ProductGroupModel>();
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
                    ParamValue = productListInfo.WaresTypeId,
                    Operation = ConditionOperate.Equal,
                    RightBrace = "",
                    Logic = ""
                });
            }
            conditions.AddRange(CreatePaginConditions(productListInfo.PageIndex, productListInfo.PageSize));
            result = GenerateDal.LoadByConditions<ProductGroupModel>(CommonSqlKey.GetProductGroupList, conditions);
            





            return result;
        }


        public int GetCount(ProductGroupModel productListInfo)
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
                    ParamValue = productListInfo.WaresTypeId,
                    Operation = ConditionOperate.Equal,
                    RightBrace = "",
                    Logic = ""
                });
            }
            result = GenerateDal.CountByConditions(CommonSqlKey.GetProductGroupListCount, conditions);
            
            return result;
        }


        /// <summary>
        /// 新增/修改会员信息
        /// </summary>
        /// <param name="memberInfo"></param>
        /// <returns></returns>
        public int PostData(ProductGroupModel productListInfo)
        {
            try
            {
                GenerateDal.BeginTransaction();
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
                productListInfo.IsGroup = 1;
                if(productListInfo.lstProductRelation!=null && productListInfo.lstProductRelation.Count>0)
                {
                    foreach(ProductGroupRelationModel relationInfo in productListInfo.lstProductRelation)
                    {
                        relationInfo.WaresGroupId = productListInfo.WaresId;
                        relationInfo.ClientId = userClientId;
                        new ProductGroupRelationService().PostData(relationInfo);
                    }
                }
                GenerateDal.Create(productListInfo);
                GenerateDal.CommitTransaction();
                return 1;
            }
            catch(Exception ex)
            {
                GenerateDal.RollBack();
                return 0;
            }



            
        }

        /// <summary>
        /// 删除用户
        /// </summary>
        /// <returns></returns>
        public int DeleteData(string id)
        {
            try
            {
                GenerateDal.BeginTransaction();
                ProductGroupModel productListInfo = new ProductGroupModel();
                productListInfo.WaresId = id;
                GenerateDal.Delete<ProductGroupModel>(CommonSqlKey.DeleteProductGroupList, productListInfo);
                new ProductGroupRelationService().DeleteData(id);
                GenerateDal.CommitTransaction();
                return 1;
            }
            catch(Exception ex)
            {
                GenerateDal.RollBack();
                return 0;
            }
           


        }

        public int UpdateData(ProductGroupModel productListInfo)
        {
            try
            {
                GenerateDal.BeginTransaction();
                productListInfo.UpdateDate = DateTime.Now;
                string userAccount = HttpContextHandler.GetHeaderObj("UserAccount").ToString();
                productListInfo.Creator = userAccount;
                productListInfo.UpdateDate = DateTime.Now;
               
                new ProductGroupRelationService().DeleteData(productListInfo.WaresId);
                if (productListInfo.lstProductRelation != null && productListInfo.lstProductRelation.Count > 0)
                {
                    foreach (ProductGroupRelationModel relationInfo in productListInfo.lstProductRelation)
                    {
                        relationInfo.WaresGroupId = productListInfo.WaresId;
                        relationInfo.ClientId = productListInfo.ClientId;
                        new ProductGroupRelationService().PostData(relationInfo);
                    }
                }
                GenerateDal.Update(CommonSqlKey.UpdateProductGroupList, productListInfo);
                GenerateDal.CommitTransaction();
                return 1;
            }
            catch(Exception ex)
            {
                GenerateDal.RollBack();
                return 0;
            }
            
        }
    }
}
