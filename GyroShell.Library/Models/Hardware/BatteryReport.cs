using GyroShell.Library.Services;

namespace GyroShell.Library.Models.Hardware
{
    public enum BatteryPowerStatus
    {
        NotInstalled,
        Draining,
        Idle,
        Charging
    }

    public record BatteryReport
    {
        public int ChargeRate;

        public int FullCapacity;

        public int RemainingCapacity;

        public int ChargePercentage;

        public BatteryPowerStatus PowerStatus;
    }
}
