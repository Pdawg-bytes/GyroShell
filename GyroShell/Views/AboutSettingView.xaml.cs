using GyroShell.Library.Services.Environment;
using GyroShell.Library.ViewModels;
using GyroShell.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;

namespace GyroShell.Views
{
    public sealed partial class AboutSettingView : Page
    {
        public AboutSettingView()
        {
            this.InitializeComponent();

            DataContext = App.ServiceProvider.GetRequiredService<AboutSettingViewModel>();
        }

        public AboutSettingViewModel ViewModel => (AboutSettingViewModel)this.DataContext;
    }
}
