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
        public double ChargeRate;

        public double FullCapacity;

        public double RemainingCapacity;

        public int ChargePercentage;

        public BatteryPowerStatus PowerStatus;
    }
}
