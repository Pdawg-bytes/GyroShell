using GyroShell.Library.Services.Hardware;
using GyroShell.Library.Services.Managers;
using GyroShell.Library.Services.Environment;

namespace GyroShell.Library.Models.Plugins
{
    public class PluginServiceContext
    {
        public ISettingsService Settings { get; init; }
        public IInternalLauncher Launcher { get; init; }
        public IDispatcherService Dispatcher { get; init; }
        public IEnvironmentInfoService EnvironmentInfo { get; init; }
        public IShellHookService ShellHook { get; init; }

        public ITimeService Clock { get; init; }
        public INetworkService Network { get; init; }
        public IBatteryService Battery { get; init; }
        public ISoundService Sound { get; init; }

        public INotificationManager Notifications { get; init; }
    }
}