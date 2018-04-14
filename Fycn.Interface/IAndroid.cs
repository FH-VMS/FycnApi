using Fycn.Model.Android;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fycn.Interface
{
    public interface IAndroid
    {
        List<AndroidProductModel> GetProductAndroid(AndroidProductModel machineInfo);
        List<AndroidProductTypeModel> GetProductTypeByMachine(string machineId);
    }
}
