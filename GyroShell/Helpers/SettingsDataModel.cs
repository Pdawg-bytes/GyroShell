using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GyroShell.Helpers
{
    public class SettingsDataModel
    {
        public bool IsSeconds { get; set; }
        public bool Is24HR { get; set; }
        public string TransparencyType { get; set; }
        public string IconType { get; set; }
    }
}
