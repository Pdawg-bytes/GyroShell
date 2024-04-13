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

using GyroShell.Library.Models.Hardware;
using GyroShell.Library.Services.Hardware;
using System;
using Windows.Devices.Power;
using Windows.System.Power;
using BatteryReport = GyroShell.Library.Models.Hardware.BatteryReport;

namespace GyroShell.Services.Hardware
{
    internal class BatteryService : IBatteryService
    {
        public bool IsBatteryInstalled
        {
            get
            {
                BatteryReport report = GetStatusReport();
                return report.PowerStatus != BatteryPowerStatus.NotInstalled;
            }
        }

        public event EventHandler BatteryStatusChanged;

        public BatteryService()
        {
            Battery.AggregateBattery.ReportUpdated += AggregateBattery_ReportUpdated;
        }

        public BatteryReport GetStatusReport()
        {
            Battery aggBattery = Battery.AggregateBattery;
            Windows.Devices.Power.BatteryReport report = aggBattery.GetReport();

            BatteryReport batteryReport = new BatteryReport()
            {
                ChargeRate = report.ChargeRateInMilliwatts.GetValueOrDefault(0),
                FullCapacity = report.FullChargeCapacityInMilliwattHours.GetValueOrDefault(0),
                RemainingCapacity = report.RemainingCapacityInMilliwattHours.GetValueOrDefault(0)
            };

            batteryReport.PowerStatus = report.Status switch
            {
                BatteryStatus.NotPresent => BatteryPowerStatus.NotInstalled,
                BatteryStatus.Idle => BatteryPowerStatus.Idle,
                BatteryStatus.Charging => BatteryPowerStatus.Charging,
                BatteryStatus.Discharging => BatteryPowerStatus.Draining,
                _ => BatteryPowerStatus.NotInstalled
            };

            if (batteryReport.PowerStatus != BatteryPowerStatus.NotInstalled)
                batteryReport.ChargePercentage = (int)Math.Ceiling((double)(batteryReport.RemainingCapacity / batteryReport.FullCapacity) * 100);
            else
                batteryReport.ChargePercentage = 0;

            return batteryReport;
        }

        private void AggregateBattery_ReportUpdated(Battery sender, object args)
        {
            BatteryStatusChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
