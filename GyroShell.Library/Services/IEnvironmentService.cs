using CoreAudio;
using System;

namespace GyroShell.Library.Services
{
    public interface IEnvironmentService
    {
        public string SystemArchitecture { get; init; }
        public Version AppVersion { get; init; }
        public DateTime AppBuildDate { get; init; }

        public MMDevice AudioDevice { get; init; }
        public MMDeviceEnumerator AudioDeviceEnumerator { get; init; }

        public bool IsWindows11 { get; }

        public int MonitorWidth { get; }
        public int MonitorHeight { get; }
    }
}
