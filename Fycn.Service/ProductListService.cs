using Fycn.Interface;
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
            string userStatus = HttpContextHandler.GetHeaderObj("Sts").ToString();
            var result = new List<ProductListModel>();
            var conditions = new List<Condition>();
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
                    ParamValue =  productListInfo.WaresTypeId ,
                    Operation = ConditionOperate.Equal,
                    RightBrace = "",
                    Logic = ""
                });
            }

            conditions.AddRange(CreatePaginConditions(productListInfo.PageIndex, productListInfo.PageSize));

            if (userStatus == "100" || userStatus == "99")
            {
                result = GenerateDal.LoadByConditions<ProductListModel>(CommonSqlKey.GetProductAllList, conditions);
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
                result = GenerateDal.LoadByConditions<ProductListModel>(CommonSqlKey.GetProductList, conditions);
            }





            return result;
        }


        public int GetCount(ProductListModel productListInfo)
        {
            var result = 0;

            string userClientId = HttpContextHandler.GetHeaderObj("UserClientId").ToString();
            string userStatus = HttpContextHandler.GetHeaderObj("Sts").ToString();
            var conditions = new List<Condition>();
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

            if (userStatus == "100" || userStatus == "99")
            {
                result = GenerateDal.CountByConditions(CommonSqlKey.GetProductListAllCount, conditions);
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
                result = GenerateDal.CountByConditions(CommonSqlKey.GetProductListCount, conditions);
            }




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
