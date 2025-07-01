#region Copyright (BSD 3-Clause License)
/*
 * GyroShell - A modern, extensible, fast, and customizable shell platform.
 * Copyright 2022-2024 Pdawg
 *
 * Licensed under the BSD 3-Clause License.
 * SPDX-License-Identifier: BSD-3-Clause
 */
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
        private DateTime currentTime;

        private readonly ISettingsService _appSettings;

        public string ClockFormat { get; set; }
        public string DateFormat { get; set; }

        public TimeService(ISettingsService appSettings)
        {
            _appSettings = appSettings;
            ClockFormat = _appSettings.EnableSeconds ? (_appSettings.EnableMilitaryTime ? "H:mm:ss" : "T") : (_appSettings.EnableMilitaryTime ? "H:mm" : "t");
            DateFormat = "M/d/yyyy";

            updateCheck = new Timer();
            updateCheck.Interval = 10;
            updateCheck.Elapsed += UpdateCheck_Elapsed;

            updateCheck.Start();

            _appSettings.SettingUpdated += AppSettings_SettingUpdated;
        }

        private void UpdateCheck_Elapsed(object sender, ElapsedEventArgs e)
        {
            currentTime = DateTime.Now;
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
                    ClockFormat = _appSettings.EnableSeconds ? (_appSettings.EnableMilitaryTime ? "H:mm:ss" : "T") : (_appSettings.EnableMilitaryTime ? "H:mm" : "t");
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
