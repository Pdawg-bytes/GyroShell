namespace GyroShell.Library.Services
{
    public interface IEnvironmentService
    {
        public string SystemArchitecture { get; init; }
        public Version AppVersion { get; init; }
        public DateTime AppBuildDate { get; init; }

        public bool IsWindows11 { get; }

        public int MonitorWidth { get; }
        public int MonitorHeight { get; }
    }
}
