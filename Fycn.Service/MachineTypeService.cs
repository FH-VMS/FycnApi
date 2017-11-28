using Fycn.Interface;
using Fycn.Model.Machine;
using Fycn.SqlDataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fycn.Service
{
    public class MachineTypeService : AbstractService, IBase<MachineTypeModel>
    {

        public List<MachineTypeModel> GetAll(MachineTypeModel machineTypeInfo)
        {
           
            var conditions = new List<Condition>();
            var conditionAccount = new Condition
            {
                LeftBrace = " AND ",
                ParamName = "TypeName",
                DbColumnName = "type_name",
                ParamValue = machineTypeInfo.TypeName + "%",
                Operation = ConditionOperate.RightLike,
                RightBrace = "",
                Logic = ""

            };
            conditions.Add(conditionAccount);
            if (!string.IsNullOrEmpty(machineTypeInfo.TypeType.ToString()))
            {
                var conditionUserName = new Condition
                {
                    LeftBrace = " AND ",
                    ParamName = "TypeType",
                    DbColumnName = "type_type",
                    ParamValue = machineTypeInfo.TypeType,
                    Operation = ConditionOperate.Equal,
                    RightBrace = "",
                    Logic = ""

                };
                conditions.Add(conditionUserName);
            }

            conditions.AddRange(CreatePaginConditions(machineTypeInfo.PageIndex, machineTypeInfo.PageSize));
            List<MachineTypeModel> result = GenerateDal.LoadByConditions<MachineTypeModel>(CommonSqlKey.GetMachineType, conditions);
            if (result != null && result.Count > 0)
            {
                foreach (MachineTypeModel item in result)
                {
                    item.Cabinets = new CabinetService().GetCabinetByMachineTypeId(item.Id);
                }
            }
            return result;
        }


        public int GetCount(MachineTypeModel machineTypeInfo)
        {
            var result = 0;

            var conditions = new List<Condition>();

            var conditionAccount = new Condition
            {
                LeftBrace = " AND ",
                ParamName = "TypeName",
                DbColumnName = "type_name",
                ParamValue = machineTypeInfo.TypeName + "%",
                Operation = ConditionOperate.RightLike,
                RightBrace = "",
                Logic = ""

            };
            conditions.Add(conditionAccount);
            if (!string.IsNullOrEmpty(machineTypeInfo.TypeType.ToString()))
            {
                var conditionUserName = new Condition
                {
                    LeftBrace = " AND ",
                    ParamName = "TypeType",
                    DbColumnName = "type_type",
                    ParamValue = machineTypeInfo.TypeType,
                    Operation = ConditionOperate.Equal,
                    RightBrace = "",
                    Logic = ""

                };
                conditions.Add(conditionUserName);
            }
            


            result = GenerateDal.CountByConditions(CommonSqlKey.GetMachineTypeCount, conditions);

            return result;
        }


        /// <summary>
        /// 新增/修改会员信息
        /// </summary>
        /// <param name="memberInfo"></param>
        /// <returns></returns>
        public int PostData(MachineTypeModel machineTypeInfo)
        {
            try
            {
                GenerateDal.BeginTransaction();
                machineTypeInfo.Id = Guid.NewGuid().ToString();
                GenerateDal.Create(machineTypeInfo);
                if (machineTypeInfo.Cabinets != null && machineTypeInfo.Cabinets.Count>0)
                {
                    foreach (var item in machineTypeInfo.Cabinets)
                    {
                        var tmpInfo = new MachineTypeAndCabinetModel();
                        tmpInfo.MachineTypeId = machineTypeInfo.Id;
                        tmpInfo.CabinetTypeId = item.CabinetId;
                        new CabinetService().PostCabinetRelationData(tmpInfo);
                    }
                }
                GenerateDal.CommitTransaction();

                return 1;
            }
            catch (Exception e)
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
                MachineTypeModel machineTypeInfo = new MachineTypeModel();
                machineTypeInfo.Id = id;
                GenerateDal.Delete<MachineTypeModel>(CommonSqlKey.DeleteMachineType, machineTypeInfo);
                new CabinetService().DeleteData(machineTypeInfo.Id);
                GenerateDal.CommitTransaction();

                return 1;
            }
            catch (Exception e)
            {
                GenerateDal.RollBack();
                return 0;
            }
            
        }

        public int UpdateData(MachineTypeModel machineTypeInfo)
        {
            try
            {
                GenerateDal.BeginTransaction();
               GenerateDal.Update(CommonSqlKey.UpdateMachineType, machineTypeInfo);
               new CabinetService().DeleteData(machineTypeInfo.Id);
                if (machineTypeInfo.Cabinets != null && machineTypeInfo.Cabinets.Count > 0)
                {
                    foreach (var item in machineTypeInfo.Cabinets)
                    {
                        var tmpInfo = new MachineTypeAndCabinetModel();
                        tmpInfo.MachineTypeId = machineTypeInfo.Id;
                        tmpInfo.CabinetTypeId = item.CabinetId;
                        new CabinetService().PostCabinetRelationData(tmpInfo);
                    }
                }
                GenerateDal.CommitTransaction();

                return 1;
            }
            catch (Exception e)
            {
                GenerateDal.RollBack();
                return 0;
            }
        }
       
    }
}
