using GyroShell.Library.Services.Environment;
using System;
using CommunityToolkit.Mvvm;
using CommunityToolkit.Mvvm.ComponentModel;

namespace GyroShell.Library.ViewModels
{
    public sealed class StartupScreenViewModel
    {
        private readonly IEnvironmentInfoService _environmentInfoService;
        public StartupScreenViewModel(IEnvironmentInfoService environmentInfoService) 
        {
            _environmentInfoService = environmentInfoService;
        }

        public Version AppVersion
        {
            get => _environmentInfoService.AppVersion;
        }
    }
}
