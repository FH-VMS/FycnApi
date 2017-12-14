using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using FycnApi.Base;
using Fycn.Model.Common;
using Fycn.Model.Sys;
using Fycn.Service;
using Fycn.Interface;

namespace FycnApi.Controllers
{
    public class MachineOperationController : ApiBaseController
    {
        public ResultObj<List<CommonDic>> GetMachines(string name = "", int pageIndex = 1, int pageSize = 10)
        {
            IMachineOperation imachine = new MachineOperationService();
            CommonDic dic = new CommonDic();
            dic.Name = name;
            int totalcount = imachine.GetMachinesCount(dic, pageIndex, pageSize);

            var pagination = new Pagination { PageSize = pageSize, PageIndex = pageIndex, StartIndex = 0, TotalRows = totalcount, TotalPage = 0 };
            return Content(imachine.GetMachines(dic, pageIndex, pageSize), pagination);
        }
    }
}