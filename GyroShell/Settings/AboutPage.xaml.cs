using GyroShell.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Windows.ApplicationModel;

namespace GyroShell.Settings
{
    public sealed partial class AboutPage : Page
    {
        private EnvironmentService m_envService;

        public AboutPage()
        {
            this.InitializeComponent();

            m_envService = App.ServiceProvider.GetRequiredService<EnvironmentService>();

            PackageVersion version = m_envService.AppVersion;

            ArchText.Text = m_envService.SystemArchitecture;
            VersionText.Text = string.Format("{0}.{1}.{2}.{3}", version.Major, version.Minor, version.Build, version.Revision);
            BDText.Text = m_envService.AppBuildDate.ToString("MMMM dd, yyyy");

            int? iconStyle = App.localSettings.Values["iconStyle"] as int?;

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
