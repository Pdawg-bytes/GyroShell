using System;

namespace GyroShell.Library.Services.Hardware
{
    public enum InternetConnection
    {
        Wired,
        Wireless,
        Data,
        Unknown
    }

    public interface INetworkService
    {
        public InternetConnection InternetType { get; }
        public bool IsInternetAvailable { get; }
        public byte SignalStrength { get; }

        public event EventHandler InternetStatusChanged;
    }
}
