using Fycn.Interface;
using Fycn.Model.Product;
using Fycn.SqlDataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fycn.Utility;

namespace Fycn.Service
{
    public class ProductConfigService : AbstractService, IBase<ProductConfigModel>
    {
        public List<ProductConfigModel> GetAll(ProductConfigModel productConfigInfo)
        {
            string userClientId = HttpContextHandler.GetHeaderObj("UserClientId").ToString();
            if (string.IsNullOrEmpty(userClientId))
            {
                return null;
            }
           
            var result = new List<ProductConfigModel>();
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
            if (!string.IsNullOrEmpty(productConfigInfo.WaresConfigName))
            {
                conditions.Add(new Condition
                {
                    LeftBrace = " AND ",
                    ParamName = "WaresConfigName",
                    DbColumnName = "b.wares_config_name",
                    ParamValue = "%" + productConfigInfo.WaresConfigName + "%",
                    Operation = ConditionOperate.Like,
                    RightBrace = "",
                    Logic = ""
                });
            }

            conditions.AddRange(CreatePaginConditions(productConfigInfo.PageIndex, productConfigInfo.PageSize));
            result = GenerateDal.LoadByConditions<ProductConfigModel>(CommonSqlKey.GetProductConfigAll, conditions);
            return result;
        }


        public int GetCount(ProductConfigModel productConfigInfo)
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
                DbColumnName = "client_id",
                ParamValue = clientIds,
                Operation = ConditionOperate.INWithNoPara,
                RightBrace = "",
                Logic = ""
            });
            if (!string.IsNullOrEmpty(productConfigInfo.WaresConfigName))
            {
                conditions.Add(new Condition
                {
                    LeftBrace = " AND ",
                    ParamName = "WaresConfigName",
                    DbColumnName = "wares_config_name",
                    ParamValue = "%" + productConfigInfo.WaresConfigName + "%",
                    Operation = ConditionOperate.Like,
                    RightBrace = "",
                    Logic = ""
                });
            }


          
           result = GenerateDal.CountByConditions(CommonSqlKey.GetProductConfigAllCount, conditions);
            




            return result;
        }


        /// <summary>
        /// 新增/修改会员信息
        /// </summary>
        /// <param name="memberInfo"></param>
        /// <returns></returns>
        public int PostData(ProductConfigModel productConfigInfo)
        {
            int result;
            productConfigInfo.WaresConfigId = Guid.NewGuid().ToString();
            productConfigInfo.UpdateDate = DateTime.Now;
            result = GenerateDal.Create(productConfigInfo);


            return result;
        }

        /// <summary>
        /// 删除用户
        /// </summary>
        /// <returns></returns>
        public int DeleteData(string id)
        {
            ProductConfigModel productConfigInfo = new ProductConfigModel();
            productConfigInfo.WaresConfigId = id;
            return GenerateDal.Delete<ProductConfigModel>(CommonSqlKey.DeleteProductConfig, productConfigInfo);
        }

        public int UpdateData(ProductConfigModel productConfigInfo)
        {
            productConfigInfo.UpdateDate = DateTime.Now;
            productConfigInfo.UpdateDate = DateTime.Now;
            return GenerateDal.Update(CommonSqlKey.UpdateProductConfig, productConfigInfo);
        }

    }
}
