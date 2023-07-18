using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;

namespace GyroShell.Settings
{
    public sealed partial class AboutPage : Page
    {
        public static string PackageArch;
        public static string PackageVer;
        public static string PackageBuild;

        public AboutPage()
        {
            this.InitializeComponent();

            ArchText.Text = PackageArch;
            VersionText.Text = PackageVer;
            BDText.Text = PackageBuild;

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
