using GyroShell.Library.Services.Environment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using System.Threading.Tasks;

namespace GyroShell.Services.Environment
{
    internal class TimeService : ITimeService, IDisposable
    {
        private Timer updateCheck;
        private int lastUpdateSecond;

        private readonly ISettingsService m_appSettings;

        public string ClockFormat { get; set; }
        public string DateFormat { get; set; }

        public TimeService(ISettingsService appSettings)
        {
            m_appSettings = appSettings;
            ClockFormat = m_appSettings.EnableSeconds ? (m_appSettings.EnableMilitaryTime ? "H:mm:ss" : "T") : (m_appSettings.EnableMilitaryTime ? "H:mm" : "t");
            DateFormat = "M/d/yyyy";

            updateCheck = new Timer();
            updateCheck.Interval = 10;
            updateCheck.Elapsed += UpdateCheck_Elapsed;

            updateCheck.Start();

            m_appSettings.SettingUpdated += AppSettings_SettingUpdated;
        }

        private void UpdateCheck_Elapsed(object sender, ElapsedEventArgs e)
        {
            DateTime currentTime = DateTime.Now;
            if (currentTime.Second != lastUpdateSecond)
            {
                lastUpdateSecond = currentTime.Second;
                UpdateClockBinding?.Invoke(this, EventArgs.Empty);
            }
        }

        public event EventHandler UpdateClockBinding;

        private void AppSettings_SettingUpdated(object sender, string key)
        {
            switch (key)
            {
                case "isSeconds":
                case "is24HR":
                    ClockFormat = m_appSettings.EnableSeconds ? (m_appSettings.EnableMilitaryTime ? "H:mm:ss" : "T") : (m_appSettings.EnableMilitaryTime ? "H:mm" : "t");
                    DateFormat = "M/d/yyyy";
                    break;
            }
        }


        public void Dispose()
        {
            updateCheck.Elapsed -= UpdateCheck_Elapsed;
        }
    }
}
