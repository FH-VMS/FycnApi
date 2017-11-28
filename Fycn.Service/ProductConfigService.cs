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
            string userStatus = HttpContextHandler.GetHeaderObj("Sts").ToString();
            var result = new List<ProductConfigModel>();
            var conditions = new List<Condition>();
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

            conditions.AddRange(CreatePaginConditions(productConfigInfo.PageIndex, productConfigInfo.PageSize));

            if (userStatus == "100" || userStatus == "99")
            {
                result = GenerateDal.LoadByConditions<ProductConfigModel>(CommonSqlKey.GetProductConfigAll, conditions);
            }
            else
            {
                conditions.Add(new Condition
                {
                    LeftBrace = "",
                    ParamName = "ClientId",
                    DbColumnName = "",
                    ParamValue = userClientId,
                    Operation = ConditionOperate.None,
                    RightBrace = "",
                    Logic = ""
                });
                result = GenerateDal.LoadByConditions<ProductConfigModel>(CommonSqlKey.GetProductConfig, conditions);
            }





            return result;
        }


        public int GetCount(ProductConfigModel productConfigInfo)
        {
            var result = 0;

            string userClientId = HttpContextHandler.GetHeaderObj("UserClientId").ToString();
            string userStatus = HttpContextHandler.GetHeaderObj("Sts").ToString();
            var conditions = new List<Condition>();
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


            if (userStatus == "100" || userStatus == "99")
            {
                result = GenerateDal.CountByConditions(CommonSqlKey.GetProductConfigAllCount, conditions);
            }
            else
            {
                conditions.Add(new Condition
                {
                    LeftBrace = "",
                    ParamName = "ClientId",
                    DbColumnName = "",
                    ParamValue = userClientId,
                    Operation = ConditionOperate.None,
                    RightBrace = "",
                    Logic = ""
                });
                result = GenerateDal.CountByConditions(CommonSqlKey.GetProductConfigCount, conditions);
            }




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
