using GyroShell.Library.Services.Environment;
using GyroShell.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;

namespace GyroShell.Views
{
    public sealed partial class AboutSettingView : Page
    {
        private IEnvironmentInfoService m_envService;
        private ISettingsService m_appSettings;

        public AboutSettingView()
        {
            this.InitializeComponent();

            m_envService = App.ServiceProvider.GetRequiredService<IEnvironmentInfoService>();
            m_appSettings = App.ServiceProvider.GetRequiredService<ISettingsService>();

            ArchText.Text = m_envService.SystemArchitecture;
            VersionText.Text = m_envService.AppVersion.ToString();
            BDText.Text = m_envService.AppBuildDate.ToString("MMMM dd, yyyy");

            switch (m_appSettings.IconStyle)
            {
                case 0:
                default:
                    InfoIcon.FontFamily = new FontFamily("Segoe MDL2 Assets");
                    LinksIcon.FontFamily = new FontFamily("Segoe MDL2 Assets");
                    LicenseIcon.FontFamily = new FontFamily("Segoe MDL2 Assets");
                    break;
                case 1:
                    InfoIcon.FontFamily = new FontFamily("Segoe Fluent Icons");
                    LinksIcon.FontFamily = new FontFamily("Segoe Fluent Icons");
                    LicenseIcon.FontFamily = new FontFamily("Segoe Fluent Icons");
                    break;
            }
        }
    }
}