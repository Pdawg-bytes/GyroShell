using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Windows.System;
using static GyroShell.Helpers.Win32.Win32Interop;

namespace GyroShell.Helpers
{
    public class IconModel
    {
        public string IconName { get; set; }
        public IntPtr Id { get; set; }
    }
}