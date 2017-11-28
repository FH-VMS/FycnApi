using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fycn.Model.Sys
{
    public class RemarkAttribute:Attribute
    {
        public string No { get; set; }
        public string Notes { get; set; }
        public string ParmsNote { get; set; }
        public string ReturnNote { get; set; }
        public RemarkAttribute(string msg)
        {
            Notes = msg;
        }
    }
}
