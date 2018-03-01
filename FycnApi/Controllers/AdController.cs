using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using FycnApi.Base;
using Fycn.Model.Ad;
using Fycn.Interface;
using Fycn.Model.Sys;
using Fycn.Service;

namespace FycnApi.Controllers
{
    public class AdController : ApiBaseController
    {
        private static IBase<AdModel> _IBase
        {
            get
            {
                return new AdService();
            }
        }

        public ResultObj<List<AdModel>> GetData()
        {
            AdModel adInfo = new AdModel();
            var ads = _IBase.GetAll(adInfo);
            return Content(ads);
        }

        public ResultObj<int> PostData([FromBody]AdModel adInfo)
        {
            return Content(_IBase.PostData(adInfo));
        }

        public ResultObj<int> PutData([FromBody]AdModel adInfo)
        {
            return Content(_IBase.UpdateData(adInfo));
        }

        public ResultObj<int> DeleteData(string idList)
        {
            return Content(_IBase.DeleteData(idList));
        }

        public ResultObj<List<AdRelationModel>> GetRelationByIdAndType(int adId, int adType=0)
        {
            AdRelationModel adRelationInfo = new AdRelationModel();
            adRelationInfo.AdId = adId;
            adRelationInfo.AdType = adType;
            IAdRelation _iRelation = new AdRelationService();
            var adRelation = _iRelation.GetRelationByIdAndType(adRelationInfo);
            return Content(adRelation);
        }
    }
}