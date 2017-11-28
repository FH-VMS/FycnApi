using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fycn.Model.Sys
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ColumnAttribute:Attribute
    {
        public string Name { get; set; }

        public bool IsAuto { get; set; }

        public bool IsPrimaryKey { get; set; }

        public bool IsNotNull { get; set; }

        public ColumnAttribute()
        {

        }

        public ColumnAttribute(string name)
        {
            Name = name;
        }
    }
}
