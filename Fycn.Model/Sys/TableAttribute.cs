using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fycn.Model.Sys
{
    [AttributeUsage(AttributeTargets.Class)]
    public class TableAttribute:Attribute
    {

        public string Name { get; set; }

        public string Key { get; set; }

        public TableAttribute()
        { }
        public TableAttribute(string name)
        {
            Name = name;
        }
    }
}
