using Fycn.Interface;
using Fycn.Model.Product;
using Fycn.SqlDataAccess;
using Fycn.Utility;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fycn.Service
{
    public class ProductGroupRelationService : AbstractService, IBase<ProductGroupRelationModel>
    {
        public List<ProductGroupRelationModel> GetAll(ProductGroupRelationModel productListInfo)
        {
           
         
            var result = new List<ProductGroupModel>();
            var conditions = new List<Condition>();
            conditions.Add(new Condition
            {
                LeftBrace = " AND ",
                ParamName = "WaresGroupId",
                DbColumnName = "a.wares_group_id",
                ParamValue = productListInfo.WaresGroupId,
                Operation = ConditionOperate.Equal,
                RightBrace = " ",
                Logic = ""
            });

            return GenerateDal.LoadByConditions<ProductGroupRelationModel>(CommonSqlKey.GetProductGroupRelation, conditions);
        }


        public int GetCount(ProductGroupRelationModel productListInfo)
        {
            
            return 0;
        }


        /// <summary>
        /// 新增/修改会员信息
        /// </summary>
        /// <param name="memberInfo"></param>
        /// <returns></returns>
        public int PostData(ProductGroupRelationModel productListInfo)
        {
            int result;
            result = GenerateDal.Create(productListInfo);




            return result;
        }

        /// <summary>
        /// 删除用户
        /// </summary>
        /// <returns></returns>
        public int DeleteData(string id)
        {


            ProductGroupRelationModel productListInfo = new ProductGroupRelationModel();
            productListInfo.WaresGroupId = id;
            return GenerateDal.Delete<ProductGroupRelationModel>(CommonSqlKey.DeleteProductGroupRelation, productListInfo);


        }

        public int UpdateData(ProductGroupRelationModel productListInfo)
        {
            return 0;
        }
    }
}
