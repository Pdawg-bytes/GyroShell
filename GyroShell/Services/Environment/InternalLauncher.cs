using GyroShell.Controls;
using GyroShell.Library.Services.Environment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GyroShell.Services.Environment
{
    public class InternalLauncher : IInternalLauncher
    {
        public InternalLauncher() 
        { 
        }

        public void LaunchShellSettings()
        {
            SettingsWindow _settingsWindow = new SettingsWindow();
            _settingsWindow.Activate();
        }
    }
}
