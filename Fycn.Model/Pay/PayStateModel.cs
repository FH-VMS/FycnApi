using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fycn.Model.Pay
{
    public class PayStateModel
    {
        public string ProductJson
        {
            get;
            set;
        }

        /// <summary>
        /// 0 表示code为空  要url重定向取得code
        /// </summary>
        public string RequestState  
        {
            get;
            set;
        }

        public string RequestData
        {
            get;
            set;
        }

        public string PrivilegeJson
        {
            get;
            set;
        }

        public decimal TotalMoney
        {
            get;
            set;
        }
    }


}
