using GyroShell.Library.Services;
using GyroShell.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;

namespace GyroShell.Settings
{
    public sealed partial class AboutPage : Page
    {
        private IEnvironmentService m_envService;
        private ISettingsService m_appSettings;

        public AboutPage()
        {
            this.InitializeComponent();

            m_envService = App.ServiceProvider.GetRequiredService<IEnvironmentService>();
            m_appSettings = App.ServiceProvider.GetRequiredService<ISettingsService>();

            ArchText.Text = m_envService.SystemArchitecture;
            VersionText.Text = m_envService.AppVersion.ToString();
            BDText.Text = m_envService.AppBuildDate.ToString("MMMM dd, yyyy");

            int? iconStyle = m_appSettings.IconStyle;

            if (iconStyle != null)
            {
                switch (iconStyle)
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
}
