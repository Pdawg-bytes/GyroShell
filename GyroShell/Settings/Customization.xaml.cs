using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using GyroShell.Controls;
using CommunityToolkit.WinUI.Helpers;
using System;
using System.Diagnostics;
using Microsoft.UI.Composition.SystemBackdrops;

namespace GyroShell.Settings
{
    public sealed partial class Customization : Page
    {
        bool? secondsEnabled = App.localSettings.Values["isSeconds"] as bool?;
        bool? is24HREnabled = App.localSettings.Values["is24HR"] as bool?;
        int? transparencyType = App.localSettings.Values["transparencyType"] as int?;
        int? iconStyle = App.localSettings.Values["iconStyle"] as int?;
        public Customization()
        {
            this.InitializeComponent();
        }

        private void TransparencyType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string materialName = e.AddedItems[0].ToString();
            switch (materialName)
            {
                case "Mica Alt":
                    App.localSettings.Values["transparencyType"] = 0;
                    LuminSlider.IsEnabled = false;
                    RestartInfo.IsOpen = true;
                    break;
                case "Mica":
                    App.localSettings.Values["transparencyType"] = 1;
                    LuminSlider.IsEnabled = false;
                    RestartInfo.IsOpen = true;
                    break;
                case "Acrylic":
                    App.localSettings.Values["transparencyType"] = 2;
                    LuminSlider.IsEnabled = true;
                    RestartInfo.IsOpen = true;
                    break;
            }
        }

        #region Clock Settings
        private void SecondsToggle_Toggled(object sender, RoutedEventArgs e)
        {
            if (TFHourToggle.IsOn == true && SecondsToggle.IsOn == false)
            {
                DefaultTaskbar.timeType = "H:mm";
                App.localSettings.Values["is24HR"] = true;
                App.localSettings.Values["isSeconds"] = false;
            }
            else if (TFHourToggle.IsOn == true && SecondsToggle.IsOn == true)
            {
                DefaultTaskbar.timeType = "H:mm:ss";
                App.localSettings.Values["is24HR"] = true;
                App.localSettings.Values["isSeconds"] = true;
            }
            else if (TFHourToggle.IsOn == false && SecondsToggle.IsOn == false)
            {
                DefaultTaskbar.timeType = "t";
                App.localSettings.Values["is24HR"] = false;
                App.localSettings.Values["isSeconds"] = false;
            }
            else if (SecondsToggle.IsOn)
            {
                App.localSettings.Values["is24HR"] = false;
                App.localSettings.Values["isSeconds"] = true;
                DefaultTaskbar.timeType = "T";
            }
        }

        private void TFHourToggle_Toggled(object sender, RoutedEventArgs e)
        {
            if (TFHourToggle.IsOn == true && SecondsToggle.IsOn == true)
            {
                DefaultTaskbar.timeType = "H:mm:ss";
                App.localSettings.Values["is24HR"] = true;
                App.localSettings.Values["isSeconds"] = true;
            }
            else if (TFHourToggle.IsOn)
            {
                DefaultTaskbar.timeType = "H:mm";
                App.localSettings.Values["is24HR"] = true;
                App.localSettings.Values["isSeconds"] = false;
            }
            else if (TFHourToggle.IsOn == false && SecondsToggle.IsOn == true)
            {
                DefaultTaskbar.timeType = "T";
                App.localSettings.Values["is24HR"] = false;
                App.localSettings.Values["isSeconds"] = true;
            }
            else
            {
                App.localSettings.Values["is24HR"] = false;
                App.localSettings.Values["isSeconds"] = false;
                DefaultTaskbar.timeType = "t";
            }
        }
        #endregion

        private void Icon_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton irb = sender as RadioButton;
            if (irb != null)
            {
                switch (irb.Name)
                {
                    case "Icon10":
                    default:
                        App.localSettings.Values["iconStyle"] = 0;
                        RestartInfo.IsOpen = true;
                        break;
                    case "Icon11":
                        App.localSettings.Values["iconStyle"] = 1;
                        RestartInfo.IsOpen = true;
                        break;
                }
            }
        }

        private void TintSlider_ValueChanged(object sender, Microsoft.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
        {
            float tintOpacity = (float)e.NewValue / 100;
            RestartInfo.IsOpen = true;
        }

        private void LuminSlider_ValueChanged(object sender, Microsoft.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
        {
            float luminOpacity = (float)e.NewValue / 100;
            RestartInfo.IsOpen = true;
        }

        private void RestartNowInfo_Click(object sender, RoutedEventArgs e)
        {

        }

        private void RestartLaterInfo_Click(object sender, RoutedEventArgs e)
        {
            RestartInfo.IsOpen = false;
        }

        private void TintColorPicker_ColorChanged(ColorPicker sender, ColorChangedEventArgs args)
        {
            App.localSettings.Values["aTint"] = (int)TintColorPicker.Color.A;
            App.localSettings.Values["rTint"] = (int)TintColorPicker.Color.R;
            App.localSettings.Values["gTint"] = (int)TintColorPicker.Color.G;
            App.localSettings.Values["bTint"] = (int)TintColorPicker.Color.B;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (transparencyType != null)
            {
                switch (transparencyType)
                {
                    case 0:
                    default:
                        TransparencyType.SelectedValue = "Mica Alt";
                        break;
                    case 1:
                        TransparencyType.SelectedValue = "Mica";
                        break;
                    case 2:
                        TransparencyType.SelectedValue = "Acrylic";
                        break;
                }
            }

            if (iconStyle != null)
            {
                switch (iconStyle)
                {
                    case 0:
                    default:
                        Icon10.IsChecked = true;
                        Icon11.IsChecked = false;
                        break;
                    case 1:
                        Icon10.IsChecked = false;
                        Icon11.IsChecked = true;
                        break;
                }
            }

            if (is24HREnabled != null)
            {
                TFHourToggle.IsOn = (bool)is24HREnabled;
            }
            if (secondsEnabled != null)
            {
                SecondsToggle.IsOn = (bool)secondsEnabled;
            }
        }
    }
}
