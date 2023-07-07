using GyroShell.Controls;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using System;
using System.Diagnostics;
using Windows.ApplicationModel.Core;
using Windows.System;
using Windows.UI;

namespace GyroShell.Settings
{
    public sealed partial class Customization : Page
    {
        bool? transparencyCustom = App.localSettings.Values["isCustomTransparency"] as bool?;
        bool? secondsEnabled = App.localSettings.Values["isSeconds"] as bool?;
        bool? is24HREnabled = App.localSettings.Values["is24HR"] as bool?;
        int? transparencyType = App.localSettings.Values["transparencyType"] as int?;
        int? iconStyle = App.localSettings.Values["iconStyle"] as int?;
        float? luminOpacity = App.localSettings.Values["luminOpacity"] as float?;
        float? tintOpacity = App.localSettings.Values["tintOpacity"] as float?;
        byte? aTint = App.localSettings.Values["aTint"] as byte?;
        byte? rTint = App.localSettings.Values["rTint"] as byte?;
        byte? gTint = App.localSettings.Values["gTint"] as byte?;
        byte? bTint = App.localSettings.Values["bTint"] as byte?;

        public static bool NotifError;

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
                    LuminSlider.Value = 0;
                    LuminSlider.IsEnabled = false;
                    if (transparencyCustom != null && transparencyCustom == false)
                    {
                        DefaultExtern();
                    }
                    RestartInfo.IsOpen = true;
                    break;
                case "Mica":
                    App.localSettings.Values["transparencyType"] = 1;
                    LuminSlider.IsEnabled = false;
                    if (transparencyCustom != null && transparencyCustom == false)
                    {
                        DefaultExtern();
                    }
                    RestartInfo.IsOpen = true;
                    break;
                case "Acrylic":
                    App.localSettings.Values["transparencyType"] = 2;
                    LuminSlider.IsEnabled = true;
                    if (transparencyCustom != null && transparencyCustom == false)
                    {
                        DefaultExtern();
                    }
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

            App.localSettings.Values["tintOpacity"] = tintOpacity;
            App.localSettings.Values["isCustomTransparency"] = true;

            RestartInfo.IsOpen = true;
        }

        private void LuminSlider_ValueChanged(object sender, Microsoft.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
        {
            float luminOpacity = (float)e.NewValue / 100;

            App.localSettings.Values["luminOpacity"] = luminOpacity;
            App.localSettings.Values["isCustomTransparency"] = true;

            RestartInfo.IsOpen = true;
        }

        private void RestartNowInfo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                AppRestartFailureReason ret = Microsoft.Windows.AppLifecycle.AppInstance.Restart("");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        private void RestartLaterInfo_Click(object sender, RoutedEventArgs e)
        {
            RestartInfo.IsOpen = false;
        }

        private void TintColorPicker_ColorChanged(ColorPicker sender, ColorChangedEventArgs args)
        {
            App.localSettings.Values["aTint"] = TintColorPicker.Color.A;
            App.localSettings.Values["rTint"] = TintColorPicker.Color.R;
            App.localSettings.Values["gTint"] = TintColorPicker.Color.G;
            App.localSettings.Values["bTint"] = TintColorPicker.Color.B;
            App.localSettings.Values["isCustomTransparency"] = true;

            RestartInfo.IsOpen = true;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (NotifError)
            {
                NotifInfo.IsOpen = true;
            }
            else
            {
                NotifInfo.IsOpen = false;
            }

            if (!Helpers.OSVersion.IsWin11())
            {
                Icon11.IsEnabled = false;
                Icon11.IsChecked = false;
            }

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

            if (luminOpacity != null)
            {
                LuminSlider.Value = (int)Math.Round((decimal)luminOpacity * 100, 1);
                App.localSettings.Values["isCustomTransparency"] = true;
            }
            else
            {
                LuminSlider.Value = 0;
                App.localSettings.Values["isCustomTransparency"] = false;
            }

            if (tintOpacity != null)
            {
                TintSlider.Value = (int)Math.Round((decimal)(tintOpacity * 100), 1);
                App.localSettings.Values["isCustomTransparency"] = true;
            }
            else
            {
                TintSlider.Value = 0;
                App.localSettings.Values["isCustomTransparency"] = false;
            }

            if (iconStyle != null)
            {
                switch (iconStyle)
                {
                    case 0:
                    default:
                        Icon10.IsChecked = true;
                        Icon11.IsChecked = false;

                        TransparencyIcon.FontFamily = new FontFamily("Segoe MDL2 Assets");
                        ClockIcon.FontFamily = new FontFamily("Segoe MDL2 Assets");
                        IconHeaderIcon.FontFamily = new FontFamily("Segoe MDL2 Assets");
                        break;
                    case 1:
                        Icon10.IsChecked = false;
                        Icon11.IsChecked = true;

                        TransparencyIcon.FontFamily = new FontFamily("Segoe Fluent Icons");
                        ClockIcon.FontFamily = new FontFamily("Segoe Fluent Icons");
                        IconHeaderIcon.FontFamily = new FontFamily("Segoe Fluent Icons");
                        break;
                }
            }

            if (aTint != null && rTint != null && gTint != null && bTint != null)
            {
                TintColorPicker.Color = Color.FromArgb((byte)aTint, (byte)rTint, (byte)gTint, (byte)bTint);

                App.localSettings.Values["isCustomTransparency"] = true;
            }
            else
            {
                App.localSettings.Values["isCustomTransparency"] = false;
            }

            if (is24HREnabled != null)
            {
                TFHourToggle.IsOn = (bool)is24HREnabled;
            }
            if (secondsEnabled != null)
            {
                SecondsToggle.IsOn = (bool)secondsEnabled;
            }

            RestartInfo.IsOpen = false;
        }

        private void DefaultsButton_Click(object sender, RoutedEventArgs e)
        {
            DefaultExtern();

            RestartInfo.IsOpen = true;
        }
        private void DefaultExtern()
        {
            aTint = null;
            rTint = null;
            bTint = null;
            gTint = null;

            luminOpacity = null;
            tintOpacity = null;

            TintSlider.Value = 0;
            LuminSlider.Value = 0;

            App.localSettings.Values["isCustomTransparency"] = false;
        }

        private async void OpenSettingsInfo_Click(object sender, RoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri("ms-settings:notifications"));
        }

        private void IgnoreInfo_Click(object sender, RoutedEventArgs e)
        {
            NotifInfo.IsOpen = false;
        }
    }
}
