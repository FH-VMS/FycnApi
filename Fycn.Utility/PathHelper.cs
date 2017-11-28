using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Fycn.Utility
{
    public class PathHelper
    {
        public static string GetPhysicalApplicationPath()
        {
            string rootdir = AppContext.BaseDirectory;
            DirectoryInfo Dir = Directory.GetParent(rootdir);
            string root = Dir.Parent.Parent.FullName;
            return root;
        }
    }
}
