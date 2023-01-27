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

        public static string settingTransparencyType;
        public static bool settingSecondsEnabled;
        public static bool settingIs24Hour;
        public static string settingIconType;

        private void TransparencyType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string materialName = e.AddedItems[0].ToString();
            switch (materialName)
            {
                case "Mica Alt":
                    break;
                case "Mica":
                    break;
                case "Acrylic":
                    break;
            }
        }

        #region Clock Settings
        private void SecondsToggle_Toggled(object sender, RoutedEventArgs e)
        {
            if (TFHourToggle.IsOn == true && SecondsToggle.IsOn == false)
            {
                DefaultTaskbar.timeType = "H:mm";
            }
            else if (TFHourToggle.IsOn == true && SecondsToggle.IsOn == true)
            {
                DefaultTaskbar.timeType = "H:mm:ss";
            }
            else if (TFHourToggle.IsOn == false && SecondsToggle.IsOn == false)
            {
                DefaultTaskbar.timeType = "t";
            }
            else if (SecondsToggle.IsOn)
            {
                DefaultTaskbar.timeType = "T";
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
                        
                        break;
                    case "Icon11":
                        
                        break;
                }
            }
        }
    }
}
