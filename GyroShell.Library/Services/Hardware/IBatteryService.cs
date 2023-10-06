using GyroShell.Library.Models.Hardware;
using System;

namespace GyroShell.Library.Services.Hardware
{
    public interface IBatteryService
    {
        public event EventHandler BatteryStatusChanged;

        public BatteryReport GetStatusReport();
    }
}
