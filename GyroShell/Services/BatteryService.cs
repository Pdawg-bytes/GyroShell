using GyroShell.Library.Models.Hardware;
using GyroShell.Library.Services;
using System;
using Windows.Devices.Power;
using Windows.System.Power;
using BatteryReport = GyroShell.Library.Models.Hardware.BatteryReport;

namespace GyroShell.Services
{
    internal class BatteryService : IBatteryService
    {
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

            batteryReport.ChargePercentage = (int)Math.Ceiling((double)(batteryReport.RemainingCapacity / batteryReport.FullCapacity) * 100);
            batteryReport.PowerStatus = report.Status switch
            {
                BatteryStatus.NotPresent => BatteryPowerStatus.NotInstalled,
                BatteryStatus.Idle => BatteryPowerStatus.Idle,
                BatteryStatus.Charging => BatteryPowerStatus.Charging,
                BatteryStatus.Discharging => BatteryPowerStatus.Draining,
                _ => BatteryPowerStatus.NotInstalled
            };

            return batteryReport;
        }

        private void AggregateBattery_ReportUpdated(Battery sender, object args)
        {
            BatteryStatusChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
