using System;

namespace GyroShell.Library.Services.Environment
{
    /// <summary>
    /// Defines a platform-agnostic service interface to get environment information.
    /// </summary>
    public interface IEnvironmentInfoService
    {
        /// <summary>
        /// The system's CPU architecture.
        /// </summary>
        public string SystemArchitecture { get; init; }

        /// <summary>
        /// The application's package version.
        /// </summary>
        public Version AppVersion { get; init; }

        /// <summary>
        /// The build date of the application package.
        /// </summary>
        public DateTime AppBuildDate { get; init; }

        /// <summary>
        /// Checks if the application is running under Windows 11.
        /// </summary>
        public bool IsWindows11 { get; }

        /// <summary>
        /// Gets the principal monitor's width in pixels.
        /// </summary>
        public int MonitorWidth { get; }

        /// <summary>
        /// Gets the principal monitor's height in pixels.
        /// </summary>
        public int MonitorHeight { get; }

        /// <summary>
        /// Gets the handle of the MainWindow.
        /// </summary>
        public IntPtr MainWindowHandle { get; set; }
    }
}
