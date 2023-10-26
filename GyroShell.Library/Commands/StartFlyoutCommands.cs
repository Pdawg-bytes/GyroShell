using CommunityToolkit.Mvvm.Input;
using GyroShell.Library.Services.Environment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GyroShell.Library.Commands
{
    public partial class StartFlyoutCommands
    {
        private readonly IEnvironmentInfoService m_envService;
        private readonly IInternalLauncher m_internalLauncher;

        public StartFlyoutCommands(IEnvironmentInfoService envService, IInternalLauncher internalLauncher)
        {
            m_envService = envService;
            m_internalLauncher = internalLauncher;
        }

        [RelayCommand]
        public void OpenShellSettings()
        {
            m_envService.SettingsInstances++;

            if (m_envService.SettingsInstances <= 1)
            {
                m_internalLauncher.LaunchShellSettings();
            }
        }
    }
}
