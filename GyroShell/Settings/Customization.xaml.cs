using CommunityToolkit.WinUI.Helpers;
using GyroShell.Controls;
using GyroShell.Helpers;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.Windows.AppLifecycle;
using System;
using System.Diagnostics;
using System.Linq;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Core;
using Windows.Storage.Pickers;
using Windows.Storage;
using Windows.System;
using Windows.UI;

using static GyroShell.Helpers.Modules.ModuleManager;
using GyroShell.Services;
using Microsoft.Extensions.DependencyInjection;
using GyroShell.Library.Services;

namespace GyroShell.Settings
{
    public sealed partial class Customization : Page
    {
        bool? transparencyCustom = App.localSettings.Values["isCustomTransparency"] as bool?;
        bool? secondsEnabled = App.localSettings.Values["isSeconds"] as bool?;
        bool? is24HREnabled = App.localSettings.Values["is24HR"] as bool?;
        int? transparencyType = App.localSettings.Values["transparencyType"] as int?;
        int? iconStyle = App.localSettings.Values["iconStyle"] as int?;
        int? tbAlignment = App.localSettings.Values["tbAlignment"] as int?;
        float? luminOpacity = App.localSettings.Values["luminOpacity"] as float?;
        float? tintOpacity = App.localSettings.Values["tintOpacity"] as float?;
        byte? aTint = App.localSettings.Values["aTint"] as byte?;
        byte? rTint = App.localSettings.Values["rTint"] as byte?;
        byte? gTint = App.localSettings.Values["gTint"] as byte?;
        byte? bTint = App.localSettings.Values["bTint"] as byte?;

        public static bool NotifError;

        private IEnvironmentService m_envService;

        public Customization()
        {
            this.InitializeComponent();

            m_envService = App.ServiceProvider.GetRequiredService<IEnvironmentService>();
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

        #region Icon events
        private void Icon_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton irb = sender as RadioButton;

            if (irb != null)
            {
                switch (irb.Name)
                {
                    case "Icon11":
                    default:
                        App.localSettings.Values["iconStyle"] = 1;
                        RestartInfo.IsOpen = true;
                        break;
                    case "Icon10":
                        App.localSettings.Values["iconStyle"] = 0;
                        RestartInfo.IsOpen = true;
                        break;
                }
            }
        }
        #endregion

        #region Transparency events
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

        private void TintColorPicker_ColorChanged(ColorPicker sender, ColorChangedEventArgs args)
        {
            App.localSettings.Values["aTint"] = TintColorPicker.Color.A;
            App.localSettings.Values["rTint"] = TintColorPicker.Color.R;
            App.localSettings.Values["gTint"] = TintColorPicker.Color.G;
            App.localSettings.Values["bTint"] = TintColorPicker.Color.B;
            App.localSettings.Values["isCustomTransparency"] = true;

            RestartInfo.IsOpen = true;
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
        #endregion

        #region Restart InfoBar events
        private async void RestartNowInfo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start(new ProcessStartInfo { FileName = Process.GetCurrentProcess().MainModule.FileName, UseShellExecute = true });
                Application.Current.Exit();
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
        #endregion

        #region Notification InfoBar events
        private async void OpenSettingsInfo_Click(object sender, RoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri("ms-settings:notifications"));
        }
        private void IgnoreInfo_Click(object sender, RoutedEventArgs e)
        {
            NotifInfo.IsOpen = false;
        }
        #endregion

        #region Alignment events
        private void AlignmentType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string alignType = e.AddedItems[0].ToString();
            switch (alignType)
            {
                case "Left":
                default:
                    App.localSettings.Values["tbAlignment"] = 0;
                    RestartInfo.IsOpen = true;
                    break;
                case "Center":
                    App.localSettings.Values["tbAlignment"] = 1;
                    RestartInfo.IsOpen = true;
                    break;
            }
        }
        #endregion

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

            if (!m_envService.IsWindows11)
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
                        FontFamily segoeMDL = new FontFamily("Segoe MDL2 Assets");

                        Icon10.IsChecked = true;
                        Icon11.IsChecked = false;

                        TransparencyIcon.FontFamily = segoeMDL;
                        ClockIcon.FontFamily = segoeMDL;
                        IconHeaderIcon.FontFamily = segoeMDL;
                        TbIcon.FontFamily = segoeMDL;
                        break;
                    case 1:
                    default:
                        if(m_envService.IsWindows11)
                        {
                            FontFamily segoeFluent = new FontFamily("Segoe Fluent Icons");

                            Icon10.IsChecked = false;
                            Icon11.IsChecked = true;

                            TransparencyIcon.FontFamily = segoeFluent;
                            ClockIcon.FontFamily = segoeFluent;
                            IconHeaderIcon.FontFamily = segoeFluent;
                            TbIcon.FontFamily = segoeFluent;
                        }
                        else
                        {
                            FontFamily segoeMDLB = new FontFamily("Segoe MDL2 Assets");

                            Icon10.IsChecked = true;
                            Icon11.IsChecked = false;

                            TransparencyIcon.FontFamily = segoeMDLB;
                            ClockIcon.FontFamily = segoeMDLB;
                            IconHeaderIcon.FontFamily = segoeMDLB;
                            TbIcon.FontFamily = segoeMDLB;
                        }
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

            if (tbAlignment != null)
            {
                switch (tbAlignment)
                {
                    case 0:
                    default:
                        AlignmentType.SelectedValue = "Left";
                        break;
                    case 1:
                        AlignmentType.SelectedValue = "Center";
                        break;
                }
            }

            RestartInfo.IsOpen = false;
        }
    }
}
