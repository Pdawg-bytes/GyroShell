using GyroShell.Controls;
using GyroShell.Library.Services.Environment;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using System;
using System.Diagnostics;
using Windows.System;
using Windows.UI;

using GyroShell.Library.ViewModels;

namespace GyroShell.Views
{
    public sealed partial class CustomizationSettingView : Page
    {
        public static bool NotifError;
        
        private ISettingsService m_appSettings;
        private IEnvironmentInfoService m_envService;

        public CustomizationSettingView(ISettingsService appSettings, IEnvironmentInfoService envService)
        {
            //DataContext = App.ServiceProvider.GetRequiredService<CustomizationSettingViewModel>();

            this.InitializeComponent();

            m_appSettings = appSettings;
            m_envService = envService;
        }

        //public CustomizationSettingViewModel ViewModel => (CustomizationSettingViewModel)this.DataContext;

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
            m_appSettings.AlphaTint = 0;
            m_appSettings.RedTint = 0;
            m_appSettings.BlueTint = 0;
            m_appSettings.GreenTint = 0;
            m_appSettings.LuminosityOpacity = 0.3f;
            m_appSettings.TintOpacity = 0.2f;

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
            float luminOpacity = m_appSettings.LuminosityOpacity;
            float tintOpacity = m_appSettings.TintOpacity;

            byte aTint = m_appSettings.AlphaTint;
            byte rTint = m_appSettings.RedTint;
            byte gTint = m_appSettings.GreenTint;
            byte bTint = m_appSettings.BlueTint;

            NotifInfo.IsOpen = NotifError ? true : false;

            // Icon Style radio buttons
            Icon11.IsEnabled = m_envService.IsWindows11 ? true : false;
  
            // Transparency type
            switch (m_appSettings.TransparencyType)
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

            // Transparency settings
            LuminSlider.Value = (int)Math.Round((decimal)luminOpacity * 100, 1);
            TintSlider.Value = (int)Math.Round((decimal)(tintOpacity * 100), 1);
            TintColorPicker.Color = Color.FromArgb((byte)aTint, (byte)rTint, (byte)gTint, (byte)bTint);

            switch (m_appSettings.IconStyle)
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

            // Clock toggles
            TFHourToggle.IsOn = m_appSettings.EnableMilitaryTime;
            SecondsToggle.IsOn = m_appSettings.EnableSeconds;

            // Taskbar Alignment Combo-box
            switch (m_appSettings.TaskbarAlignment)
            {
                case 0:
                default:
                    AlignmentType.SelectedValue = "Left";
                    break;
                case 1:
                    AlignmentType.SelectedValue = "Center";
                    break;
            }

            RestartInfo.IsOpen = false;
        }
    }
}
