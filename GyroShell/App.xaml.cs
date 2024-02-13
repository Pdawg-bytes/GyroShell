using GyroShell.Controls;
using GyroShell.Library.Services.Environment;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using System;
using System.Threading.Tasks;
using Windows.Storage;

namespace GyroShell
{
    public partial class App : Application
    {
        internal static StartupWindow startupScreen;
        private Window m_window;
        private IntPtr handle;

        public App()
        {
            this.InitializeComponent();
        }

        protected async override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            ConfigureServices();
            PreloadServices();

            await LoadStartupScreenContentAsync();
            m_window = new MainWindow();
            m_window.Activate();
        }

        private async Task LoadStartupScreenContentAsync()
        {
            startupScreen = new StartupWindow();
            startupScreen.Activate();
            await Task.Delay(200);
        }
    }
}
