using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using FycnApi.Base;
using Fycn.Model.Privilege;
using Fycn.Interface;
using Fycn.Service;
using Fycn.Model.Sys;

namespace FycnApi.Controllers
{
    public class ActivityController : ApiBaseController
    {
        private static IBase<ActivityModel> _IBase
        {
            get
            {
                return new ActivityService();
            }
        }

        public ResultObj<List<ActivityModel>> GetData(string name = "", string activityType = "", int pageIndex = 1, int pageSize = 10)
        {
            // IProduct service = new ProductService();
            //List<ProductModel> products = service.GetAllProducts();

            ActivityModel activityInfo = new ActivityModel();
            activityInfo.Name = name;
            activityInfo.ActivityType = activityType;
            activityInfo.PageIndex = pageIndex;
            activityInfo.PageSize = pageSize;
            var data = _IBase.GetAll(activityInfo);
            int totalcount = _IBase.GetCount(activityInfo);

            var pagination = new Pagination { PageSize = pageSize, PageIndex = pageIndex, StartIndex = 0, TotalRows = totalcount, TotalPage = 0 };
            return Content(data, pagination);
        }

        public ResultObj<int> PostData([FromBody]ActivityModel activityInfo)
        {
            return Content(_IBase.PostData(activityInfo));
        }

        public ResultObj<int> PutData([FromBody]ActivityModel activityInfo)
        {
            return Content(_IBase.UpdateData(activityInfo));
        }

        public ResultObj<int> DeleteData(string idList)
        {
            return Content(_IBase.DeleteData(idList));
        }

        public ResultObj<List<ActivityPrivilegeRelationModel>> GetActivityRelationById(string activityId)
        {
            if (string.IsNullOrEmpty(activityId))
            {
                return Content(new List<ActivityPrivilegeRelationModel>());
            }
            IBase<ActivityPrivilegeRelationModel> baseRelation = new ActivityRelationService();
            ActivityPrivilegeRelationModel relationInfo = new ActivityPrivilegeRelationModel();
            relationInfo.ActivityId = activityId;
            return Content(baseRelation.GetAll(relationInfo));
        }
    }
}