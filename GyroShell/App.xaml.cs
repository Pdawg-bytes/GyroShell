using GyroShell.Controls;
using Microsoft.UI.Xaml;
using System;
using System.Threading.Tasks;
using Windows.Storage;

namespace GyroShell
{
    public partial class App : Application
    {
        internal static StartupScreen startupScreen;
        private Window m_window;

        public static ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;

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
            startupScreen = new StartupScreen();
            startupScreen.Activate();
            await Task.Delay(200);
        }
    }
}
