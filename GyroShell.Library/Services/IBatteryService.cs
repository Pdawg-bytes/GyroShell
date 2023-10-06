using GyroShell.Library.Models.Hardware;
using System;

namespace GyroShell.Library.Services
{
    public interface IBatteryService
    {
        public event EventHandler BatteryStatusChanged;

        public BatteryReport GetStatusReport();
    }
}
