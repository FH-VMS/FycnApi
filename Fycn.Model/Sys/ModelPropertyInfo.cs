using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Fycn.Model.Sys
{
    public class ModelPropertyInfo
    {

        public string PropertyName { get; set; }
        public Type PropertyType { get; set; }
        public string ColumnName { get; set; }
        public bool IsPrimaryColumn { get; set; }
        public bool IsAuto { get; set; }
        public bool IsNotNull { get; set; }

        public PropertyInfo PropertyInfo { get; set; }

        public string XmlName { get; set; }
        public string XmlFormat { get; set; }

        public object DefaultValue { get; set; }
    }
}
