using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GyroShell.Helpers
{
    public static class OSVersion
    {
        public static bool IsWin11()
        {
            return Environment.OSVersion.Version.Build >= 22000;
        }
    }
}
