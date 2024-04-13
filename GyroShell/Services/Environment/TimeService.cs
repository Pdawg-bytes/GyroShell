#region Copyright (License GPLv3)
// GyroShell - A modern, extensible, fast, and customizable shell platform.
// Copyright (C) 2022-2024  Pdawg
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
#endregion

using GyroShell.Library.Services.Environment;
using System;
using System.Timers;

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
