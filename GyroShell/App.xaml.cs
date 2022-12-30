using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Shapes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Diagnostics;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using GyroShell.Settings;

namespace GyroShell
{
    public partial class App : Application
    {
        public App()
        {
            this.InitializeComponent();
        }

        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            m_window = new MainWindow();
            m_window.Activate();

            Package package = Package.Current;
            AboutPage.PackageArch = package.Id.Architecture.ToString();
            package.Id.Version.ToString();
            PackageVersion version = package.Id.Version;
            AboutPage.PackageVer = string.Format("{0}.{1}.{2}.{3}", version.Major, version.Minor, version.Build, version.Revision);
            string mainExecutablePath = System.IO.Path.Combine(Package.Current.InstalledLocation.Path, "GyroShell.exe");
            DateTime buildDate = File.GetLastWriteTime(mainExecutablePath);
            AboutPage.PackageBuild = buildDate.ToString();
        }

        private Window m_window;
    }
}
