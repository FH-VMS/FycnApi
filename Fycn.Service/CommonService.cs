using Fycn.Interface;
using Fycn.Model.Common;
using Fycn.Model.User;
using Fycn.SqlDataAccess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fycn.Utility;
using Fycn.Model.Sys;

namespace Fycn.Service
{
    public class CommonService:AbstractService,ICommon
    {
        public List<MenuModel> GetMenus(string userAccount)
        {
            string sts = HttpContextHandler.GetHeaderObj("Sts").ToString();
            List<MenuModel> menuList = null;
            if (sts == "100")
            {
                var dic = new Dictionary<string, object>();
                menuList = GenerateDal.Load<MenuModel>();
               
            }
            else
            {
                string userAccessId  = HttpContextHandler.GetHeaderObj("UserAccessId").ToString();
                var dic = new Dictionary<string, object>();
                dic.Add("UserAccessId", userAccessId);
                menuList = GenerateDal.Load<MenuModel>(CommonSqlKey.GetMenuByUser, dic);
            }

            var fatherList = from m in menuList
                             where m.MenuFather == null || m.MenuFather == ""
                             orderby m.MenuId
                             select m;
            foreach (MenuModel item in fatherList)
            {
                var menu = from m in menuList
                           where m.MenuFather == item.MenuId
                           select m;
                item.Menus = menu.ToList<MenuModel>();
            }
            return fatherList.ToList<MenuModel>();
           
        }

        public UserModel PostUser(UserModel userInfo)
        {
            var dic = new Dictionary<string, object>();
            dic.Add("UserAccount", userInfo.UserAccount);
            dic.Add("UserPassword", Md5.md5(userInfo.UserPassword,16));
            var userList = GenerateDal.Load<UserModel>(CommonSqlKey.GetLogin, dic);
            if (userList != null && userList.ToList<UserModel>().Count>0)
            {

                return userList[0];
            }
            else
            {
                return null;
            }
            
        }

        //取字典的方法
        public List<DicModel> GetDic(string id)
        {
            var dic = new Dictionary<string, object>();
            dic.Add("id", id);
            return GenerateDal.Load<DicModel>(CommonSqlKey.GetDic, dic);
        }

        //取客户等级
        public List<DicModel> GetRank(string id)
        {
            string sts = HttpContextHandler.GetHeaderObj("Sts").ToString();
            if (sts == "100")
            {
                var rankList = GetDic(id);
                var result = from m in rankList
                             where m.Value != "0"
                             select m;
                return result.ToList<DicModel>();
            }
            else
            {
                string userAccessId = HttpContextHandler.GetHeaderObj("UerAccessId").ToString(); 
                var dic = new Dictionary<string, object>();
                dic.Add("Id", id);
                dic.Add("DmsId", id);
                return GenerateDal.Load<DicModel>(CommonSqlKey.GetRank, dic);
            }
           
        }

        // 根据权限模板取等级
        //取客户等级
        public int GetRankValue(string id)
        {
            var dic = new Dictionary<string, object>();
            dic.Add("Id", id);
            return Convert.ToInt32(GenerateDal.Load<DicModel>(CommonSqlKey.GetRankValue, dic)[0].Value);
        }

        //取客户树形结构作字典
        public List<CommonDic> GetClientDic()
        {
            string userStatus = HttpContextHandler.GetHeaderObj("Sts").ToString();
            var clientId = HttpContextHandler.GetHeaderObj("UserClientId");
            var conditions = new List<Condition>();
            if (userStatus == "100" || userStatus == "99")
            {
                conditions.Add(new Condition
                {
                    LeftBrace = " AND ",
                    ParamName = "ClientFatherId",
                    DbColumnName = "client_father_id",
                    ParamValue = "self",
                    Operation = ConditionOperate.Equal,
                    RightBrace = "",
                    Logic = ""
                });
            }
            else
            {
                conditions.Add(new Condition
                {
                    LeftBrace = " AND ",
                    ParamName = "ClientFatherId",
                    DbColumnName = "client_father_id",
                    ParamValue = clientId,
                    Operation = ConditionOperate.Equal,
                    RightBrace = "",
                    Logic = ""
                });

             
            }
            return GetCustomersFinalResult(GenerateDal.LoadByConditions<CommonDic>(CommonSqlKey.GetClientDic, conditions));

        }

        private List<CommonDic> GetCustomersFinalResult(List<CommonDic> result)
        {
            if (result != null && result.Count > 0)
            {
                foreach (CommonDic cusInfo in result)
                {
                    var conditions = new List<Condition>();
                    conditions.Add(new Condition
                    {
                        LeftBrace = " AND ",
                        ParamName = "ClientFatherId",
                        DbColumnName = "client_father_id",
                        ParamValue = cusInfo.Id,
                        Operation = ConditionOperate.Equal,
                        RightBrace = "",
                        Logic = ""
                    });
                    cusInfo.children = GenerateDal.LoadByConditions<CommonDic>(CommonSqlKey.GetClientDic, conditions);
                    GetCustomersFinalResult(cusInfo.children);
                }
            }



            return result;
        }

        // 取用户树形结构当字典
        public List<CommonDic> GetUserDic()
        {
            string userStatus = HttpContextHandler.GetHeaderObj("Sts").ToString();
            var clientId = HttpContextHandler.GetHeaderObj("UserClientId");
            var conditions = new List<Condition>();
            if (userStatus == "100" || userStatus == "99")
            {

            }
            else
            {
                conditions.Add(new Condition
                {
                    LeftBrace = " AND ",
                    ParamName = "ClientFatherId",
                    DbColumnName = "client_father_id",
                    ParamValue = clientId,
                    Operation = ConditionOperate.Equal,
                    RightBrace = "",
                    Logic = ""
                });


            }
            return GenerateDal.LoadByConditions<CommonDic>(CommonSqlKey.GetClientDic, conditions);

        }

        // 取权限等级字典
        public List<CommonDic> GetAuthDic()
        {
            string userStatus = HttpContextHandler.GetHeaderObj("Sts").ToString();
            var clientId = HttpContextHandler.GetHeaderObj("UserClientId");
            string accessId = HttpContextHandler.GetHeaderObj("UserAccessId").ToString();
            var conditions = new List<Condition>();
            if (userStatus == "100" || userStatus == "99")
            {

            }
            else
            {
                int rank = GetRankValue(accessId);
                conditions.Add(new Condition
                {
                    LeftBrace = " AND ",
                    ParamName = "Rank",
                    DbColumnName = "a.rank",
                    ParamValue = rank,
                    Operation = ConditionOperate.Greater,
                    RightBrace = "",
                    Logic = ""
                });


            }
            return GenerateDal.LoadByConditions<CommonDic>(CommonSqlKey.GetAuthDic, conditions);

        }

        // 取机型字典模板
        public List<CommonDic> GetMachineTypeDic()
        {
            var conditions = new List<Condition>();
            return GenerateDal.LoadByConditions<CommonDic>(CommonSqlKey.GetMachineTypeDic, conditions);

        }

        // 根据客户取客户的用户们
        public List<CommonDic> GetUserByClientId(string id)
        {
            var conditions = new List<Condition>();
            conditions.Add(new Condition
            {
                LeftBrace = " AND ",
                ParamName = "ClientId",
                DbColumnName = "usr_client_id",
                ParamValue = id,
                Operation = ConditionOperate.Equal,
                RightBrace = "",
                Logic = ""
            });
            return GenerateDal.LoadByConditions<CommonDic>(CommonSqlKey.GetUserByClientId, conditions);
        }

        // 根据客户取对应机器
        public List<CommonDic> GetMachineDic()
        {
            string clientId = HttpContextHandler.GetHeaderObj("UserClientId").ToString();
            var conditions = new List<Condition>();
            conditions.Add(new Condition
            {
                LeftBrace = " ",
                ParamName = "ClientId",
                DbColumnName = "",
                ParamValue = clientId,
                Operation = ConditionOperate.None,
                RightBrace = "",
                Logic = ""
            });
            List<CommonDic> machines = GenerateDal.LoadByConditions<CommonDic>(CommonSqlKey.GetMachineDic, conditions);
            foreach (CommonDic commDic in machines)
            {
                var innerConditions = new List<Condition>();
                innerConditions.Add(new Condition
                {
                    LeftBrace = " AND ",
                    ParamName = "MachineId",
                    DbColumnName = "c.machine_id",
                    ParamValue = commDic.Id,
                    Operation = ConditionOperate.Equal,
                    RightBrace = "",
                    Logic = ""
                });
                commDic.children = GenerateDal.LoadByConditions<CommonDic>(CommonSqlKey.GetCabinetByMachineId, innerConditions);
            }
            return machines;
        }

        //取图片资源字典
        public List<CommonDic> GetPictureDic()
        {
            string userClientId = HttpContextHandler.GetHeaderObj("UserClientId").ToString();
            var conditions = new List<Condition>();
            conditions.Add(new Condition
            {
                LeftBrace = "  ",
                ParamName = "ClientId",
                DbColumnName = "",
                ParamValue = userClientId,
                Operation = ConditionOperate.None,
                RightBrace = "",
                Logic = ""
            });
            conditions.Add(new Condition
            {
                LeftBrace = "  AND ",
                ParamName = "AClientId",
                DbColumnName = "a.client_id",
                ParamValue = userClientId,
                Operation = ConditionOperate.Equal,
                RightBrace = "",
                Logic = ""
            });
            return GenerateDal.LoadByConditions<CommonDic>(CommonSqlKey.GetPictureDic, conditions);
        }

        //取商品作字典
        public List<CommonDic> GetProductDic()
        {
            string userClientId = HttpContextHandler.GetHeaderObj("UserClientId").ToString();
            string sts = HttpContextHandler.GetHeaderObj("Sts").ToString();
            var result = new List<CommonDic>();
            var conditions = new List<Condition>();
            if (sts == "100" || sts == "99")
            {
                result = GenerateDal.LoadByConditions<CommonDic>(CommonSqlKey.GetProductDicAll, conditions);
            }
            else
            {
                conditions.Add(new Condition
                {
                    LeftBrace = "  ",
                    ParamName = "ClientId",
                    DbColumnName = "",
                    ParamValue = userClientId,
                    Operation = ConditionOperate.None,
                    RightBrace = "",
                    Logic = ""
                });
                result = GenerateDal.LoadByConditions<CommonDic>(CommonSqlKey.GetProductDic, conditions);

            }

            return result;
        }

        // 取货柜作字典
        public List<CommonDic> GetCabinetDic()
        {
            var conditions = new List<Condition>();

            return GenerateDal.LoadByConditions<CommonDic>(CommonSqlKey.GetCabinetDic, conditions);
        }

        //修改个人登录密码
        public int UpdatePassword(UserModel userInfo)
        {
            userInfo.UserPassword = Md5.md5(userInfo.UserPassword, 16);
            return GenerateDal.Update(CommonSqlKey.ChangePassword, userInfo);
        }

        /// <summary>
        /// 取机器各个状态数
        /// </summary>
        /// <returns></returns>
        public DataTable GetTotalMachineCount()
        {
            var clientId = HttpContextHandler.GetHeaderObj("UserClientId");
            var conditions = new List<Condition>();
            if (string.IsNullOrEmpty(clientId.ToString()))
            {
                return new DataTable();
            }
            conditions.Add(new Condition
            {
                LeftBrace = "",
                ParamName = "ClientId",
                DbColumnName = "",
                ParamValue = clientId,
                Operation = ConditionOperate.None,
                RightBrace = "",
                Logic = ""
            });
            // 返回的三个数字按顺序分别代表未启用  离线  在线
            return GenerateDal.LoadDataTableByConditions(CommonSqlKey.GetMachineCountWithStatus, conditions);
        }

        /// <summary>
        /// 取机器各个状态数
        /// </summary>
        /// <returns></returns>
        public int CheckMachineId(string machineId)
        {
            var conditions = new List<Condition>();
            conditions.Add(new Condition
            {
                LeftBrace = " AND ",
                ParamName = "DeviceId",
                DbColumnName = "device_id",
                ParamValue = machineId,
                Operation = ConditionOperate.Equal,
                RightBrace = "",
                Logic = ""
            });
            return GenerateDal.CountByConditions(CommonSqlKey.CheckMachineId, conditions);
        }


        public List<CommonDic> GetMachineNameById(string machineId)
        {
            var conditions = new List<Condition>();
            conditions.Add(new Condition
            {
                LeftBrace = " AND ",
                ParamName = "MachineId",
                DbColumnName = "machine_id",
                ParamValue = machineId,
                Operation = ConditionOperate.Equal,
                RightBrace = "",
                Logic = ""
            });
            return GenerateDal.LoadByConditions<CommonDic>(CommonSqlKey.GetMachineNameById, conditions);
        }

        // 取机型字典模板
        public List<CommonDic> GetPayConfigDic(string clientId)
        {
            string clientIds = GetChildAndParentIds(clientId);
            var conditions = new List<Condition>();
            conditions.Add(new Condition
            {
                LeftBrace = " AND ",
                ParamName = "ClientId",
                DbColumnName = "client_id",
                ParamValue = "'"+clientIds.Replace(",","','")+"'",
                Operation = ConditionOperate.INWithNoPara,
                RightBrace = "",
                Logic = " "
            });


            return GenerateDal.LoadByConditions<CommonDic>(CommonSqlKey.GetPayConfigDic, conditions);

        }

        private string GetChildAndParentIds(string clientId)
        {
            var conditions = new List<Condition>();
            conditions.Add(new Condition
            {
                LeftBrace = "",
                ParamName = "ClientId",
                DbColumnName = "",
                ParamValue = clientId,
                Operation = ConditionOperate.None,
                RightBrace = "",
                Logic = ""
            });

            DataTable result = GenerateDal.LoadDataTableByConditions(CommonSqlKey.GetChildAndParentIds, conditions);
            if (result.Rows.Count == 0)
            {
                return string.Empty;
            }
            else
            {
                return result.Rows[0][0].ToString();
            }
        }
    }
}
