using GyroShell.Library.Services.Environment;
using Microsoft.UI.Xaml.Media;

namespace GyroShell.Library.ViewModels
{
    public sealed class SettingsWindowViewModel
    {
        private readonly ISettingsService m_appSettings;
        
        public SettingsWindowViewModel(ISettingsService appSettings)
        {
            m_appSettings = appSettings;
        }

        public FontFamily IconFontFamily => m_appSettings.IconStyle switch
        {
            0 => new FontFamily("Segoe MDL2 Assets"),
            1 => new FontFamily("Segoe Fluent Icons"),
            _ => new FontFamily("Segoe MDL2 Assets")
        };
    }
}
