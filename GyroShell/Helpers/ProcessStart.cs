using System;
using System.Diagnostics;

namespace GyroShell.Helpers
{
    public class ProcessStart
    {
        
        public static ProcessStartInfo ProcessStartEx(string procName, bool createNoWindow, bool useShellEx)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();

            if (procName != null)
            {
                startInfo.CreateNoWindow = createNoWindow;
                startInfo.UseShellExecute = useShellEx;
                startInfo.FileName = System.IO.Path.Combine(Environment.SystemDirectory, procName);

                return startInfo;
            }
            else
            {
                return null;
            }
        }
    }
}
