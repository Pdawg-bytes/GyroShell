using GyroShell.Library.Services.Environment;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.ObjectModel;
using Windows.Storage.Pickers;

using static GyroShell.Services.Managers.PluginManager;
using GyroShell.Library.Models.InternalData;
using GyroShell.Library.ViewModels;

namespace GyroShell.Views
{
    public sealed partial class PluginsSettingView : Page
    {
        public PluginsSettingView()
        {
            this.InitializeComponent();

            DataContext = App.ServiceProvider.GetRequiredService<PluginSettingViewModel>();
        }

        public PluginSettingViewModel ViewModel => (PluginSettingViewModel)this.DataContext;
    }
}
