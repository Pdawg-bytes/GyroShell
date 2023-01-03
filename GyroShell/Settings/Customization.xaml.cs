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
using GyroShell.ViewModels;

namespace GyroShell.Settings
{
    public sealed partial class Customization : Page
    {
        private readonly CustomizationViewModel _customizationViewModel;

        public static bool TFHourEnabled;
        public static bool SecondsEnabled;
        public Customization(CustomizationViewModel customizationViewModel)
        {
            _customizationViewModel = customizationViewModel;
            this.InitializeComponent();
        }

        public CustomizationViewModel CustomizationViewModel => _customizationViewModel;

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
                MainWindow.timeType = "T";
                SecondsEnabled = true;
            }
            else
            {
                MainWindow.timeType = "t";
                SecondsEnabled = false;
            }
        }

        private void TFHourToggle_Toggled(object sender, RoutedEventArgs e)
        {
            if (TFHourToggle.IsOn)
            {
                MainWindow.timeType = "H:mm";
                TFHourEnabled = true;
            }
            else
            {
                MainWindow.timeType = "t";
                TFHourEnabled = false;
            }
        }
    }
}
