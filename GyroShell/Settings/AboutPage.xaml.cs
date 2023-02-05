using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

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
            if (iconStyle != null )
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
