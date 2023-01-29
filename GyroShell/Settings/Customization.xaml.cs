using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using GyroShell.Controls;
using CommunityToolkit.WinUI.Helpers;
using System;
using System.Diagnostics;

namespace GyroShell.Settings
{
    public sealed partial class Customization : Page
    {
        public Customization()
        {
            this.InitializeComponent();
        }

        private async void TransparencyType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string materialName = e.AddedItems[0].ToString();
            switch (materialName)
            {
                case "Mica Alt":
                    App.localSettings.Values["transparencyType"] = 0;
                    try
                    {
                        ContentDialog dialog = new RestartDialog();
                        dialog.XamlRoot = this.XamlRoot;
                        await dialog.ShowAsync();
                    }
                    catch(Exception ex)
                    {
                        Debug.Write(ex.ToString());
                    }
                    break;
                case "Mica":
                    App.localSettings.Values["transparencyType"] = 1;
                    break;
                case "Acrylic":
                    App.localSettings.Values["transparencyType"] = 2;
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
                        break;
                    case "Icon11":
                        App.localSettings.Values["iconStyle"] = 1;
                        break;
                }
            }
        }
    }
}
