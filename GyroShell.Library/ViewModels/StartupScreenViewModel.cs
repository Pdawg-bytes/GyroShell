using GyroShell.Library.Services.Environment;
using System;
using CommunityToolkit.Mvvm;
using CommunityToolkit.Mvvm.ComponentModel;

namespace GyroShell.Library.ViewModels
{
    public sealed class StartupScreenViewModel
    {
        private readonly IEnvironmentInfoService m_envService;

        public StartupScreenViewModel(IEnvironmentInfoService envService) 
        {
            m_envService = envService;
        }

        public Version AppVersion
        {
            get => m_envService.AppVersion;
        }
    }
}
