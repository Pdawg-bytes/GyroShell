using GyroShell.Library.Services;
using System;
using System.IO;
using Windows.ApplicationModel;
using static GyroShell.Helpers.Win32.Win32Interop;

namespace GyroShell.Services
{
    internal class EnvironmentService : IEnvironmentService
    {
        public string SystemArchitecture { get; init; }
        public Version AppVersion { get; init; }
        public DateTime AppBuildDate { get; init; }

        public bool IsWindows11
        {
            get => Environment.OSVersion.Version.Build >= 22000;
        }

        public int MonitorWidth
        {
            get => GetSystemMetrics(SM_CXSCREEN);
        }

        public int MonitorHeight
        {
            get => GetSystemMetrics(SM_CYSCREEN);
        }

        public EnvironmentService()
        {
            if (Package.Current.InstalledLocation != null)
            {
                Package package = Package.Current;
                this.SystemArchitecture = package.Id.Architecture.ToString();

                PackageVersion version = package.Id.Version;
                this.AppVersion = new Version(version.Major, version.Minor, version.Build, version.Revision);

                string mainExecutablePath = Path.Combine(Package.Current.InstalledLocation.Path, "GyroShell.exe");
                this.AppBuildDate = File.GetLastWriteTime(mainExecutablePath);
            }
            else
            {
                SYSTEM_INFO sysInfo = new SYSTEM_INFO();
                GetNativeSystemInfo(out sysInfo);

                ushort arch = sysInfo.wProcessorArchitecture;

                this.SystemArchitecture = arch switch
                {
                    0 => "X86",
                    5 => "ARM",
                    6 => "ARM64",
                    9 => "X64",
                    _ => "Unknown Processor",
                };

                this.AppVersion = new Version(0, 0, 0, 0);
                this.AppBuildDate = new DateTime(2023, 9, 28);
            }
        }
    }
}
