using GyroShell.Library.Services.Environment;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.ObjectModel;
using Windows.Storage.Pickers;

using static GyroShell.Services.Managers.ModuleManager;
using GyroShell.Library.Models.InternalData;
using GyroShell.Library.ViewModels;

namespace GyroShell.Views
{
    public sealed partial class ModulesSettingView : Page
    {
        public ModulesSettingView()
        {
            this.InitializeComponent();

            DataContext = App.ServiceProvider.GetRequiredService<ModulesSettingViewModel>();
        }

        public ModulesSettingViewModel ViewModel => (ModulesSettingViewModel)this.DataContext;
    }
}
