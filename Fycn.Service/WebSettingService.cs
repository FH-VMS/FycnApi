using Fycn.Interface;
using Fycn.Model.Wechat;
using Fycn.SqlDataAccess;
using Fycn.Utility;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fycn.Service
{
    public class WebSettingService : AbstractService, IBase<WebSettingModel>
    {
        public List<WebSettingModel> GetAll(WebSettingModel webSettingInfo)
        {
            string userClientId = string.Empty;
            if (string.IsNullOrEmpty(webSettingInfo.ClientId))
            {
                userClientId = HttpContextHandler.GetHeaderObj("UserClientId").ToString();
            }
            else
            {
                userClientId = webSettingInfo.ClientId;
            }
            if (string.IsNullOrEmpty(userClientId))
            {
                return null;
            }
            var conditions = new List<Condition>();
            conditions.Add(new Condition
            {
                LeftBrace = " AND ",
                ParamName = "ClientId",
                DbColumnName = "client_id",
                ParamValue = userClientId,
                Operation = ConditionOperate.Equal,
                RightBrace = " ",
                Logic = ""
            });
            
            return GenerateDal.LoadByConditions<WebSettingModel>(CommonSqlKey.GetWechatInfo, conditions);
        }

        public int GetCount(WebSettingModel webSettingInfo)
        {
            if (string.IsNullOrEmpty(webSettingInfo.Id))
            {
                return 0;
            }
            var conditions = new List<Condition>();
            
            conditions.Add(new Condition
            {
                LeftBrace = " AND ",
                ParamName = "Id",
                DbColumnName = "id",
                ParamValue = webSettingInfo.Id,
                Operation = ConditionOperate.Equal,
                RightBrace = " ",
                Logic = ""
            });
            return GenerateDal.CountByConditions(CommonSqlKey.IsExistWechat, conditions);
        }

        public int PostData(WebSettingModel webSettingInfo)
        {
            if (string.IsNullOrEmpty(webSettingInfo.ClientId))
            {
                webSettingInfo.ClientId = HttpContextHandler.GetHeaderObj("UserClientId").ToString();

            }
            webSettingInfo.Id = Guid.NewGuid().ToString();
            return GenerateDal.Create<WebSettingModel>(webSettingInfo);
        }

        public int UpdateData(WebSettingModel webSettingInfo)
        {
            if (string.IsNullOrEmpty(webSettingInfo.ClientId))
            {
                webSettingInfo.ClientId = HttpContextHandler.GetHeaderObj("UserClientId").ToString();

            }
            return GenerateDal.Update(CommonSqlKey.UpdateWechatInfo, webSettingInfo);
        }

        public int DeleteData(string id)
        {
            return 0;
        }
    }
}
