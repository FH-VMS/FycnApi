using Fycn.Interface;
using Fycn.Model.Machine;
using Fycn.SqlDataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fycn.Service
{
    public class MachineCabinetService : AbstractService, IBase<MachineCabinetModel>
    {
        public List<MachineCabinetModel> GetAll(MachineCabinetModel machineCabinetInfo)
        {
           

            var conditions = new List<Condition>();
            if (!string.IsNullOrEmpty(machineCabinetInfo.CabinetName))
            {
                conditions.Add(new Condition
                {
                    LeftBrace = " AND ",
                    ParamName = "CabinetName",
                    DbColumnName = "cabinet_name",
                    ParamValue = "%" + machineCabinetInfo.CabinetName + "%",
                    Operation = ConditionOperate.Like,
                    RightBrace = "",
                    Logic = ""
                });
            }


            conditions.AddRange(CreatePaginConditions(machineCabinetInfo.PageIndex, machineCabinetInfo.PageSize));

            return GenerateDal.LoadByConditions<MachineCabinetModel>(CommonSqlKey.GetMachineCabinet, conditions);
        }


        public int GetCount(MachineCabinetModel machineCabinetInfo)
        {
            var result = 0;

           

            var conditions = new List<Condition>();
            if (!string.IsNullOrEmpty(machineCabinetInfo.CabinetName))
            {
                conditions.Add(new Condition
                {
                    LeftBrace = " AND ",
                    ParamName = "CabinetName",
                    DbColumnName = "cabinet_name",
                    ParamValue = "%" + machineCabinetInfo.CabinetName + "%",
                    Operation = ConditionOperate.Like,
                    RightBrace = "",
                    Logic = ""
                });
            }



            result = GenerateDal.CountByConditions(CommonSqlKey.GetCabinetCount, conditions);

            return result;
        }


        
        public int PostData(MachineCabinetModel machineCabinetInfo)
        {
            int result;
            machineCabinetInfo.CabinetId = Guid.NewGuid().ToString();
         
            result = GenerateDal.Create(machineCabinetInfo);




            return result;
        }

       
        public int DeleteData(string id)
        {
                MachineCabinetModel machineCabinetInfo = new MachineCabinetModel();
                machineCabinetInfo.CabinetId = id;
                return GenerateDal.Delete<MachineCabinetModel>(CommonSqlKey.DeleteMachineCabinet, machineCabinetInfo);
        }

        public int UpdateData(MachineCabinetModel machineCabinetInfo)
        {
            return GenerateDal.Update(CommonSqlKey.UpdateMachineCabinet, machineCabinetInfo);
        }
    }
}
