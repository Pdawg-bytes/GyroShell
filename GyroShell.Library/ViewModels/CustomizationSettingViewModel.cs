using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GyroShell.Library.Services.Environment;
using System.Diagnostics.Metrics;
using System.Windows.Input;

namespace GyroShell.Library.ViewModels
{
    public sealed class CustomizationSettingViewModel : ObservableObject
    {
        private readonly ISettingsService m_appSettings;
        private readonly ITimeService m_timeService;

        public CustomizationSettingViewModel(ISettingsService appSettings, ITimeService timeService)
        {
            m_appSettings = appSettings;
            m_timeService = timeService;
        }

        public bool EnableMilitaryTime
        {
            get => m_appSettings.EnableMilitaryTime;
            set
            {
                m_appSettings.EnableMilitaryTime = value;
                UpdateTimeSettings();
            }
        }

        public bool EnableSeconds
        {
            get => m_appSettings.EnableSeconds;
            set
            {
                m_appSettings.EnableSeconds = value;
                UpdateTimeSettings();
            }
        }

        private void UpdateTimeSettings() 
        {
            if (EnableMilitaryTime)
            {
                m_timeService.ClockFormat = EnableSeconds ? "H:mm:ss" : "H:mm";
                m_appSettings.EnableMilitaryTime = true;
                m_appSettings.EnableSeconds = EnableSeconds;
            }
            else
            {
                m_timeService.ClockFormat = EnableSeconds ? "T" : "t";
                m_appSettings.EnableMilitaryTime = false;
                m_appSettings.EnableSeconds = EnableSeconds;
            }
        }
    }
}
