using GyroShell.Controls;
using GyroShell.Helpers;
using GyroShell.Library.Services.Environment;
using Microsoft.UI.Xaml;
using System;
using System.Diagnostics;

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

        public void LaunchNewShellInstance()
        {
            try
            {
                Process.Start(new ProcessStartInfo { FileName = Process.GetCurrentProcess().MainModule.FileName, UseShellExecute = true });
                Application.Current.Exit();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        public void LaunchProcess(string procName, bool createNoWindow, bool useShellEx)
        {
            Process.Start(ProcessStart.ProcessStartEx(procName, createNoWindow, useShellEx));
        }
    }
}
