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
using Microsoft.UI.Composition.SystemBackdrops;
using GyroShell.Controls;

namespace GyroShell.Settings
{
    public sealed partial class Customization : Page
    {
        DefaultTaskbar defaultTaskbar;
        public static bool SecondsEnabled;
        public static bool TFHourEnabled;
        public Customization()
        {
            this.InitializeComponent();
            defaultTaskbar = new DefaultTaskbar();
        }

        private void TransparencyType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string materialName = e.AddedItems[0].ToString();
            switch (materialName)
            {
                case "Mica Alt (Default)":
                default:
                    MainWindow.micaKind = MicaKind.BaseAlt;
                    MainWindow.useAcrylic = false;
                    break;
                case "Mica":
                    MainWindow.micaKind = MicaKind.Base;
                    MainWindow.useAcrylic = false;
                    break;
                case "Acrylic":
                    MainWindow.useAcrylic = true;
                    break;
            }
        }

        private void SecondsToggle_Toggled(object sender, RoutedEventArgs e)
        {
            if (SecondsToggle.IsOn)
            {
                DefaultTaskbar.timeType = "T";
            }
            else
            {
                DefaultTaskbar.timeType = "t";
            }
        }

        private void TFHourToggle_Toggled(object sender, RoutedEventArgs e)
        {
            if (TFHourToggle.IsOn == true && SecondsToggle.IsOn == true)
            {
                DefaultTaskbar.timeType = "H:mm:ss";
            }
            else if (TFHourToggle.IsOn)
            {
                DefaultTaskbar.timeType = "H:mm";
            }
            else if (TFHourToggle.IsOn == false && SecondsToggle.IsOn == true)
            {
                DefaultTaskbar.timeType = "T";
            }
            else
            {
                DefaultTaskbar.timeType = "t";
            }
        }

        private void Icon_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton irb = sender as RadioButton;
            if (irb != null)
            {
                switch (irb.Name)
                {
                    case "Icon10":
                    default:
                        defaultTaskbar.UpdateIconService(true);
                        break;
                    case "Icon11":
                        defaultTaskbar.UpdateIconService(false);
                        break;
                }
            }
        }
    }
}
