using CommunityToolkit.Mvvm.ComponentModel;
using GyroShell.Library.Services.Environment;
using Microsoft.UI.Xaml.Media;

namespace GyroShell.Library.ViewModels
{
    public partial class SettingsWindowViewModel : ObservableObject
    {
        private readonly ISettingsService m_appSettings;
        
        public SettingsWindowViewModel(ISettingsService appSettings)
        {
            m_appSettings = appSettings;

            m_appSettings.SettingUpdated += AppSettings_SettingUpdated;
        }

        private void AppSettings_SettingUpdated(object sender, string key)
        {
            switch (key)
            {
                case "iconStyle": OnPropertyChanged(nameof(IconFontFamily)); break;
            }
        }

        public FontFamily IconFontFamily => m_appSettings.IconStyle switch
        {
            0 => new FontFamily("Segoe MDL2 Assets"),
            1 => new FontFamily("Segoe Fluent Icons"),
            _ => new FontFamily("Segoe MDL2 Assets")
        };
    }
}
