using Microsoft.UI.Xaml;
using System;
using System.IO;
using Windows.ApplicationModel;
using GyroShell.Settings;
using static GyroShell.Helpers.Win32Interop;
using Windows.Storage;
using GyroShell.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace GyroShell
{
    public partial class App : Application
    {
        public App()
        {
            this.InitializeComponent();
        }

        public static StorageFile settingsFile;

        protected async override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            m_window = new MainWindow();
            m_window.Activate();

            try
            {
                settingsFile = await KnownFolders.DocumentsLibrary.CreateFileAsync("gshell_settings.json", CreationCollisionOption.ReplaceExisting);
                if (Package.Current.InstalledLocation != null)
                {
                    Package package = Package.Current;
                    AboutPage.PackageArch = package.Id.Architecture.ToString();
                    package.Id.Version.ToString();
                    PackageVersion version = package.Id.Version;
                    AboutPage.PackageVer = string.Format("{0}.{1}.{2}.{3}", version.Major, version.Minor, version.Build, version.Revision);
                    string mainExecutablePath = System.IO.Path.Combine(Package.Current.InstalledLocation.Path, "GyroShell.exe");
                    DateTime buildDate = File.GetLastWriteTime(mainExecutablePath);
                    AboutPage.PackageBuild = buildDate.ToString("MMMM dd, yyyy");
                }
            }
            catch
            {
                SYSTEM_INFO sysInfo = new SYSTEM_INFO();
                GetNativeSystemInfo(out sysInfo);
                var arch = sysInfo.wProcessorArchitecture;
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
                AboutPage.PackageBuild = "Janurary, 2023";
            }
        }

        private Window m_window;

        public static void SerializeSettings()
        {
            var model = new SettingsDataModel
            {
                IsSeconds = Customization.settingSecondsEnabled,
                Is24HR = Customization.settingIs24Hour,
                TransparencyType = Customization.settingTransparencyType,
                IconType = Customization.settingIconType,
            };

            DefaultContractResolver contractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            };

            var json = JsonConvert.SerializeObject(model, Formatting.Indented, new JsonSerializerSettings
            {
                ContractResolver = contractResolver,
                NullValueHandling = NullValueHandling.Ignore
            });

            System.Diagnostics.Debug.WriteLine(json);
        }
    }
}
