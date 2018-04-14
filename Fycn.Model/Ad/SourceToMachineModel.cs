using System;
using System.Collections.Generic;
using System.Text;

namespace Fycn.Model.Ad
{
    public class SourceToMachineModel
    {
        public string SourceId
        {
            get;
            set;
        }

        public string SourceUrl
        {
            get;
            set;
        }

        public string Sequence
        {
            get;
            set;
        }

        public int AdType
        {
            get;
            set;
        }

        public int PlayTime
        {
            get;
            set;
        }
        public int IsPush
        {
            get;
            set;
        }
    }
}
