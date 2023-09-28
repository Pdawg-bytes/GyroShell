using GyroShell.Controls;
using GyroShell.Settings;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using System;
using System.IO;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Storage;
using Windows.UI.Core;
using static GyroShell.Helpers.Win32.Win32Interop;

namespace GyroShell
{
    public partial class App : Application
    {
        internal static StartupScreen startupScreen;
        private Window m_window;

        public static ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;

        public App()
        {
            this.InitializeComponent();
        }

        protected async override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            ConfigureServices();
            PreloadServices();

            await LoadStartupScreenContentAsync();
            m_window = new MainWindow();
            m_window.Activate();

            try
            {
                if (Package.Current.InstalledLocation != null)
                {
                    Package package = Package.Current;
                    AboutPage.PackageArch = package.Id.Architecture.ToString();

                    PackageVersion version = package.Id.Version;
                    AboutPage.PackageVer = string.Format("{0}.{1}.{2}.{3}", version.Major, version.Minor, version.Build, version.Revision);

                    string mainExecutablePath = Path.Combine(Package.Current.InstalledLocation.Path, "GyroShell.exe");
                    DateTime buildDate = File.GetLastWriteTime(mainExecutablePath);
                    AboutPage.PackageBuild = buildDate.ToString("MMMM dd, yyyy");
                }
            }
            catch
            {
                SYSTEM_INFO sysInfo = new SYSTEM_INFO();
                GetNativeSystemInfo(out sysInfo);

                ushort arch = sysInfo.wProcessorArchitecture;

                switch (arch)
                {
                    case 0:
                        AboutPage.PackageArch = "X86";
                        break;
                    case 5:
                        AboutPage.PackageArch = "ARM";
                        break;
                    case 6:
                        AboutPage.PackageArch = "ARM64";
                        break;
                    case 9:
                        AboutPage.PackageArch = "X64";
                        break;
                    default:
                        AboutPage.PackageArch = "Unknown Processor";
                        break;
                }

                AboutPage.PackageVer = "0.0.0.0";
                AboutPage.PackageBuild = "February, 2023";
            }
        }

        private async Task LoadStartupScreenContentAsync()
        {
            startupScreen = new StartupScreen();
            startupScreen.Activate();
            await Task.Delay(200);
        }
    }
}
