using Fycn.Interface;
using Fycn.Model.Common;
using Fycn.Model.User;
using Fycn.SqlDataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fycn.Service
{
    public class AuthService : AbstractService, IBase<AuthModel>,IAuth
    {
        public List<AuthModel> GetAll(AuthModel authInfo)
        {
            //var x = HttpContextHandler.GetHeaderObj("UserAccount");
                var conditions = new List<Condition>();

                conditions.AddRange(CreatePaginConditions(authInfo.PageIndex, authInfo.PageSize));
                return GenerateDal.LoadByConditions<AuthModel>(CommonSqlKey.GetAuth, conditions);
            
           
        }

        // 取权限模板做字典
        public List<AuthModel> GetAuthDic()
        {

            return GenerateDal.Load<AuthModel>(CommonSqlKey.GetAuth, new Dictionary<string, object>());


        }

        public int GetCount(AuthModel authInfo)
        {
            var result = 0;

            var conditions = new List<Condition>();


            result = GenerateDal.CountByConditions(CommonSqlKey.GetAuthCount, conditions);

            return result;
        }


        /// <summary>
        /// 新增/修改会员信息
        /// </summary>
        /// <param name="memberInfo"></param>
        /// <returns></returns>
        public int PostData(AuthModel authInfo)
        {
            int result;


            authInfo.Id = Guid.NewGuid().ToString();

            result = GenerateDal.Create(authInfo);




            return result;
        }

        public int PostAuthTemplate(string name, int rank, List<MenuModel> lstAuthModel)
        {
           
                try
                {

                    GenerateDal.BeginTransaction();
                   
                    AuthModel authModel = new AuthModel();
                    authModel.Id=Guid.NewGuid().ToString();
                    authModel.DmsName = name;
                    authModel.Rank=rank;
                    GenerateDal.Create<AuthModel>(authModel);
                    foreach(var menuModel in lstAuthModel)
                    {
                        var authRelateModel = new AuthRelateModel();
                        authRelateModel.Id = Guid.NewGuid().ToString();
                        authRelateModel.CorrMenuId = menuModel.MenuId;
                        authRelateModel.CorrDmsId = authModel.Id;
                        authRelateModel.CorrAdd = menuModel.Add;
                        authRelateModel.CorrDel = menuModel.Del;
                        authRelateModel.CorrModify = menuModel.Mod;
                        authRelateModel.CorrSearch = menuModel.Sear;
                        GenerateDal.Create<AuthRelateModel>(authRelateModel);
                    }

                    GenerateDal.CommitTransaction();
                    return 1;
                }
                catch (Exception ee)
                {
                    GenerateDal.RollBack();
                    return 0;
                }
        }

        public int UpdaetAuthTemplate(string id,string name, int rank, List<MenuModel> lstAuthModel)
        {

            try
            {

                GenerateDal.BeginTransaction();
                //删除模板
                AuthModel updateAuth = new AuthModel();
                updateAuth.Id = id;
                updateAuth.DmsName = name;
                updateAuth.Rank = rank;
                GenerateDal.Update<AuthModel>(CommonSqlKey.UpdateAuth, updateAuth);
                //删除权限对应关系
                AuthRelateModel delAuthReate = new AuthRelateModel();
                delAuthReate.CorrDmsId = id;
                GenerateDal.Delete<AuthRelateModel>(CommonSqlKey.DeleteAuthRelate, delAuthReate);
               
                foreach (var menuModel in lstAuthModel)
                {
                    var authRelateModel = new AuthRelateModel();
                    authRelateModel.Id = Guid.NewGuid().ToString();
                    authRelateModel.CorrMenuId = menuModel.MenuId;
                    authRelateModel.CorrDmsId = updateAuth.Id;
                    authRelateModel.CorrAdd = menuModel.Add;
                    authRelateModel.CorrDel = menuModel.Del;
                    authRelateModel.CorrModify = menuModel.Mod;
                    authRelateModel.CorrSearch = menuModel.Sear;
                    GenerateDal.Create<AuthRelateModel>(authRelateModel);
                }

                GenerateDal.CommitTransaction();
                return 1;
            }
            catch (Exception ee)
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
                //删除模板
                AuthModel delAuth = new AuthModel();
                delAuth.Id = id;
                GenerateDal.Delete<AuthModel>(CommonSqlKey.DeleteAuth, delAuth);
                //删除权限对应关系
                AuthRelateModel delAuthReate = new AuthRelateModel();
                delAuthReate.CorrDmsId = id;
                GenerateDal.Delete<AuthRelateModel>(CommonSqlKey.DeleteAuthRelate, delAuthReate);

                GenerateDal.CommitTransaction();
                return 1;
            }
            catch (Exception ee)
            {
                GenerateDal.RollBack();
                return 0;
            }
        }

        public int UpdateData(AuthModel authInfo)
        {
            return GenerateDal.Update(CommonSqlKey.UpdateAuth, authInfo);
        }

        public List<MenuModel> GetAuthRelateByDmsId(string id)
        {
            var dic = new Dictionary<string, object>();
            dic.Add("CorrDmsId", id);
            var authRelateList = GenerateDal.Load<MenuModel>(CommonSqlKey.GetAuthByDmsId, dic);

            var fatherList = from m in authRelateList
                             where m.MenuFather == null || m.MenuFather == ""
                             orderby m.MenuId
                             select m;
            foreach (MenuModel item in fatherList)
            {
                var menu = from m in authRelateList
                           where m.MenuFather == item.MenuId
                           select m;
                item.Menus = menu.ToList<MenuModel>();
            }
            return fatherList.ToList<MenuModel>();
        }
    }
}
