using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Text.Json;
using GyroShell.Controls;
using GyroShell.Helpers;

namespace GyroShell.Settings
{
    public sealed partial class Customization : Page
    {
        public Customization()
        {
            this.InitializeComponent();
        }

        public static int settingTransparencyType;
        public static bool settingSecondsEnabled;
        public static bool settingIs24Hour;
        public static int settingIconType;

        private void TransparencyType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string materialName = e.AddedItems[0].ToString();
            switch (materialName)
            {
                case "Mica Alt":
                    settingTransparencyType = 0;
                    break;
                case "Mica":
                    settingTransparencyType = 1;
                    break;
                case "Acrylic":
                    settingTransparencyType = 2;
                    break;
            }
        }

        #region Clock Settings
        private void SecondsToggle_Toggled(object sender, RoutedEventArgs e)
        {
            if (TFHourToggle.IsOn == true && SecondsToggle.IsOn == false)
            {
                DefaultTaskbar.timeType = "H:mm";
                settingIs24Hour = true;
                settingSecondsEnabled = false;
            }
            else if (TFHourToggle.IsOn == true && SecondsToggle.IsOn == true)
            {
                DefaultTaskbar.timeType = "H:mm:ss";
                settingIs24Hour = true;
                settingSecondsEnabled = true;
            }
            else if (TFHourToggle.IsOn == false && SecondsToggle.IsOn == false)
            {
                DefaultTaskbar.timeType = "t";
                settingIs24Hour = false;
                settingSecondsEnabled = false;
            }
            else if (SecondsToggle.IsOn)
            {
                DefaultTaskbar.timeType = "T";
                settingIs24Hour = true;
                settingSecondsEnabled = false;
            }
        }

        private void TFHourToggle_Toggled(object sender, RoutedEventArgs e)
        {
            if (TFHourToggle.IsOn == true && SecondsToggle.IsOn == true)
            {
                DefaultTaskbar.timeType = "H:mm:ss";
                settingIs24Hour = true;
                settingSecondsEnabled = true;
            }
            else if (TFHourToggle.IsOn)
            {
                DefaultTaskbar.timeType = "H:mm";
                settingIs24Hour = true;
                settingSecondsEnabled = false;
            }
            else if (TFHourToggle.IsOn == false && SecondsToggle.IsOn == true)
            {
                settingIs24Hour = false;
                settingSecondsEnabled = true;
                DefaultTaskbar.timeType = "T";
            }
            else
            {
                settingIs24Hour = false;
                settingSecondsEnabled = false;
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
                        settingIconType = 0;
                        break;
                    case "Icon11":
                        settingIconType = 1;
                        break;
                }
            }
        }
    }
}
