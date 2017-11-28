using Fycn.Interface;
using Fycn.Model.Pay;
using Fycn.SqlDataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fycn.Service
{
    public class PayService : AbstractService, IPay
    {
        public List<ProductModel> GetProducInfo(string machineId,  List<KeyTunnelModel> lstTunnels)
        {
            var conditions = new List<Condition>();

            if (!string.IsNullOrEmpty(machineId))
            {
                conditions.Add(new Condition
                {
                    LeftBrace = " AND ",
                    ParamName = "MachineId",
                    DbColumnName = "a.machine_id",
                    ParamValue = machineId,
                    Operation = ConditionOperate.Equal,
                    RightBrace = "  ",
                    Logic = ""
                });
            }
            if(lstTunnels.Count>0) {
               for(int i=0;i<lstTunnels.Count;i++)
               {
                   if(i==0) {
                       if (lstTunnels.Count == 1)
                       {
                           conditions.Add(new Condition
                           {
                               LeftBrace = " AND (",
                               ParamName = "TunnelId" + i,
                               DbColumnName = "a.tunnel_id",
                               ParamValue = lstTunnels[i].tid,
                               Operation = ConditionOperate.Equal,
                               RightBrace = " )",
                               Logic = ""
                           });
                       }
                       else
                       {
                           conditions.Add(new Condition
                           {
                               LeftBrace = " AND (",
                               ParamName = "TunnelId" + i,
                               DbColumnName = "a.tunnel_id",
                               ParamValue = lstTunnels[i].tid,
                               Operation = ConditionOperate.Equal,
                               RightBrace = "",
                               Logic = ""
                           });
                       }
                        
                   } else if(i==lstTunnels.Count-1) {
                       conditions.Add(new Condition
                        {
                            LeftBrace = " OR ",
                            ParamName = "TunnelId" + i,
                            DbColumnName = "a.tunnel_id",
                            ParamValue = lstTunnels[i].tid,
                            Operation = ConditionOperate.Equal,
                            RightBrace = ")",
                            Logic = ""
                        });
                   } else {
                        conditions.Add(new Condition
                        {
                            LeftBrace = " OR ",
                            ParamName = "TunnelId" + i,
                            DbColumnName = "a.tunnel_id",
                            ParamValue = lstTunnels[i].tid,
                            Operation = ConditionOperate.Equal,
                            RightBrace = "",
                            Logic = ""
                        });
                   }
                   
                }
            }

            return GenerateDal.LoadByConditions<ProductModel>(CommonSqlKey.GetProductInfo, conditions);


        }


        public List<ProductModel> GetProducInfoByWaresId(string machineId, List<KeyTunnelModel> lstTunnels)
        {
            var conditions = new List<Condition>();

            if (!string.IsNullOrEmpty(machineId))
            {
                conditions.Add(new Condition
                {
                    LeftBrace = " AND ",
                    ParamName = "MachineId",
                    DbColumnName = "a.machine_id",
                    ParamValue = machineId,
                    Operation = ConditionOperate.Equal,
                    RightBrace = "  ",
                    Logic = ""
                });
            }
            if (lstTunnels.Count > 0)
            {
                for (int i = 0; i < lstTunnels.Count; i++)
                {
                    if (i == 0)
                    {
                        if (lstTunnels.Count == 1)
                        {
                            conditions.Add(new Condition
                            {
                                LeftBrace = " AND (",
                                ParamName = "WaresId" + i,
                                DbColumnName = "a.wares_id",
                                ParamValue = lstTunnels[i].tid,
                                Operation = ConditionOperate.Equal,
                                RightBrace = " )",
                                Logic = ""
                            });
                        }
                        else
                        {
                            conditions.Add(new Condition
                            {
                                LeftBrace = " AND (",
                                ParamName = "WaresId" + i,
                                DbColumnName = "a.wares_id",
                                ParamValue = lstTunnels[i].tid,
                                Operation = ConditionOperate.Equal,
                                RightBrace = "",
                                Logic = ""
                            });
                        }

                    }
                    else if (i == lstTunnels.Count - 1)
                    {
                        conditions.Add(new Condition
                        {
                            LeftBrace = " OR ",
                            ParamName = "WaresId" + i,
                            DbColumnName = "a.wares_id",
                            ParamValue = lstTunnels[i].tid,
                            Operation = ConditionOperate.Equal,
                            RightBrace = ")",
                            Logic = ""
                        });
                    }
                    else
                    {
                        conditions.Add(new Condition
                        {
                            LeftBrace = " OR ",
                            ParamName = "WaresId" + i,
                            DbColumnName = "a.wares_id",
                            ParamValue = lstTunnels[i].tid,
                            Operation = ConditionOperate.Equal,
                            RightBrace = "",
                            Logic = ""
                        });
                    }

                }
            }

            conditions.Add(new Condition
            {
                LeftBrace = "  ",
                ParamName = "",
                DbColumnName = "a.wares_id",
                ParamValue = "",
                Operation = ConditionOperate.GroupBy,
                RightBrace = "",
                Logic = ""
            });

            return GenerateDal.LoadByConditions<ProductModel>(CommonSqlKey.GetProductInfoByWaresId, conditions);


        }


        //取商品列表
        public List<ConfigModel> GetConfig(string machindId)
        {
            var conditions = new List<Condition>();

            if (string.IsNullOrEmpty(machindId))
            {
                return null;
            }

            conditions.Add(new Condition
            {
                LeftBrace = "  AND ",
                ParamName = "MachineId",
                DbColumnName = "b.machine_id",
                ParamValue = machindId,
                Operation = ConditionOperate.Equal,
                RightBrace = "",
                Logic = ""
            });

            return GenerateDal.LoadByConditions<ConfigModel>(CommonSqlKey.GetPayConfig, conditions);


        }
    }
}
