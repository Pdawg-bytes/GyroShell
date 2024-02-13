using GyroShell.Controls;
using GyroShell.Library.Services.Environment;
using GyroShell.Library.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using System;
using System.Diagnostics;
using Windows.System;
using Windows.UI;

namespace GyroShell.Views
{
    public sealed partial class CustomizationSettingView : Page
    {
        public CustomizationSettingView()
        {
            this.InitializeComponent();

            DataContext = App.ServiceProvider.GetRequiredService<CustomizationSettingViewModel>();
        }

        CustomizationSettingViewModel ViewModel => (CustomizationSettingViewModel)this.DataContext;
    }
}
