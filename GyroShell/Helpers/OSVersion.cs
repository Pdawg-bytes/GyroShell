using System;

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
