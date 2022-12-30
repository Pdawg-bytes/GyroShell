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
        }
    }
}
