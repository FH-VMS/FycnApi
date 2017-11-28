using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fycn.Model.Sys
{
    public class RemoteServiceEntity
    {
        public string InterfaceNo { get; set; }
        public string ResModule { get; set; }
        public string MethodBehavior { get; set; }
        public string InterfaceRemark { get; set; }
        public string ParmsRemark { get; set; }
        public string ReturnClass { get; set; }
        public string ReturnRemark { get; set; }
        public string InvokeUrl { get; set; }
    }
}
