using GyroShell.Controls;
using GyroShell.Library.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using System;
using System.Diagnostics;
using Windows.System;
using Windows.UI;

namespace GyroShell.Settings
{
    public sealed partial class Customization : Page
    {
        public static bool NotifError;

        private IEnvironmentInfoService m_envService;
        private ISettingsService m_appSettings;

        public Customization()
        {
            this.InitializeComponent();

            m_envService = App.ServiceProvider.GetRequiredService<IEnvironmentInfoService>();
            m_appSettings = App.ServiceProvider.GetRequiredService<ISettingsService>();
        }

        #region Clock Settings
        private void SecondsToggle_Toggled(object sender, RoutedEventArgs e)
        {
            if (TFHourToggle.IsOn == true && SecondsToggle.IsOn == false)
            {
                DefaultTaskbar.timeType = "H:mm";
                m_appSettings.EnableMilitaryTime = true;
                m_appSettings.EnableSeconds = false;
            }
            else if (TFHourToggle.IsOn == true && SecondsToggle.IsOn == true)
            {
                DefaultTaskbar.timeType = "H:mm:ss";
                m_appSettings.EnableMilitaryTime = true;
                m_appSettings.EnableSeconds = true;
            }
            else if (TFHourToggle.IsOn == false && SecondsToggle.IsOn == false)
            {
                DefaultTaskbar.timeType = "t";
                m_appSettings.EnableMilitaryTime = false;
                m_appSettings.EnableSeconds = false;
            }
            else if (SecondsToggle.IsOn)
            {
                m_appSettings.EnableMilitaryTime = false;
                m_appSettings.EnableSeconds = true;
                DefaultTaskbar.timeType = "T";
            }
        }

        private void TFHourToggle_Toggled(object sender, RoutedEventArgs e)
        {
            if (TFHourToggle.IsOn == true && SecondsToggle.IsOn == true)
            {
                DefaultTaskbar.timeType = "H:mm:ss";
                m_appSettings.EnableMilitaryTime = true;
                m_appSettings.EnableSeconds = true;
            }
            else if (TFHourToggle.IsOn)
            {
                DefaultTaskbar.timeType = "H:mm";
                m_appSettings.EnableMilitaryTime = true;
                m_appSettings.EnableSeconds = false;
            }
            else if (TFHourToggle.IsOn == false && SecondsToggle.IsOn == true)
            {
                DefaultTaskbar.timeType = "T";
                m_appSettings.EnableMilitaryTime = false;
                m_appSettings.EnableSeconds = true;
            }
            else
            {
                m_appSettings.EnableMilitaryTime = false;
                m_appSettings.EnableSeconds = false;
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
                        m_appSettings.IconStyle = 1;
                        RestartInfo.IsOpen = true;
                        break;
                    case "Icon10":
                        m_appSettings.IconStyle = 0;
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
            bool? transparencyCustom = m_appSettings.EnableCustomTransparency;

            switch (materialName)
            {
                case "Mica Alt":
                    m_appSettings.TransparencyType = 0;
                    LuminSlider.Value = 0;
                    LuminSlider.IsEnabled = false;

                    if (transparencyCustom != null && transparencyCustom == false)
                    {
                        DefaultExtern();
                    }

                    RestartInfo.IsOpen = true;
                    break;
                case "Mica":
                    m_appSettings.TransparencyType = 1;
                    LuminSlider.IsEnabled = false;

                    if (transparencyCustom != null && transparencyCustom == false)
                    {
                        DefaultExtern();
                    }

                    RestartInfo.IsOpen = true;
                    break;
                case "Acrylic":
                    m_appSettings.TransparencyType = 2;
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

            m_appSettings.TintOpacity = tintOpacity;
            m_appSettings.EnableCustomTransparency = true;

            RestartInfo.IsOpen = true;
        }

        private void LuminSlider_ValueChanged(object sender, Microsoft.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
        {
            float luminOpacity = (float)e.NewValue / 100;

            m_appSettings.LuminosityOpacity = luminOpacity;
            m_appSettings.EnableCustomTransparency = true;

            RestartInfo.IsOpen = true;
        }

        private void TintColorPicker_ColorChanged(ColorPicker sender, ColorChangedEventArgs args)
        {
            m_appSettings.AlphaTint = TintColorPicker.Color.A;
            m_appSettings.RedTint = TintColorPicker.Color.R;
            m_appSettings.GreenTint = TintColorPicker.Color.G;
            m_appSettings.BlueTint = TintColorPicker.Color.B;
            m_appSettings.EnableCustomTransparency = true;

            RestartInfo.IsOpen = true;
        }

        private void DefaultsButton_Click(object sender, RoutedEventArgs e)
        {
            DefaultExtern();
            RestartInfo.IsOpen = true;
        }
        private void DefaultExtern()
        {
            m_appSettings.AlphaTint = null;
            m_appSettings.RedTint = null;
            m_appSettings.BlueTint = null;
            m_appSettings.GreenTint = null;
            m_appSettings.LuminosityOpacity = null;
            m_appSettings.TintOpacity = null;

            TintSlider.Value = 0;
            LuminSlider.Value = 0;

            m_appSettings.EnableCustomTransparency = false;
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
                    m_appSettings.TaskbarAlignment = 0;
                    RestartInfo.IsOpen = true;
                    break;
                case "Center":
                    m_appSettings.TaskbarAlignment = 1;
                    RestartInfo.IsOpen = true;
                    break;
            }
        }
        #endregion

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            int? transparencyType = m_appSettings.TransparencyType;
            int? iconStyle = m_appSettings.IconStyle;
            int? tbAlignment = m_appSettings.TaskbarAlignment;

            float? luminOpacity = m_appSettings.LuminosityOpacity;
            float? tintOpacity = m_appSettings.TintOpacity;

            byte? aTint = m_appSettings.AlphaTint;
            byte? rTint = m_appSettings.RedTint;
            byte? gTint = m_appSettings.GreenTint;
            byte? bTint = m_appSettings.BlueTint;

            bool? is24HREnabled = m_appSettings.EnableMilitaryTime;
            bool? secondsEnabled = m_appSettings.EnableSeconds;

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
                m_appSettings.EnableCustomTransparency = true;
            }
            else
            {
                LuminSlider.Value = 0;
                m_appSettings.EnableCustomTransparency = false;
            }

            if (tintOpacity != null)
            {
                TintSlider.Value = (int)Math.Round((decimal)(tintOpacity * 100), 1);
                m_appSettings.EnableCustomTransparency = true;
            }
            else
            {
                TintSlider.Value = 0;
                m_appSettings.EnableCustomTransparency = false;
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
                        if (m_envService.IsWindows11)
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
                m_appSettings.EnableCustomTransparency = true;
            }
            else
            {
                m_appSettings.EnableCustomTransparency = false;
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
