using Fycn.Model.Ad;
using Fycn.Model.Sys;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fycn.Interface
{
    public interface IAdRelation
    {
        [Remark("根据广告id和类型取关系表", ParmsNote = "实体", ReturnNote = "实体列表")]
        List<AdRelationModel> GetRelationByIdAndType(AdRelationModel adRelationInfo);

        [Remark("取机器对应的广告资源", ParmsNote = "机器id", ReturnNote = "实体列表")]
        List<SourceToMachineModel> GetAdSource(string machineId);
    }
}
