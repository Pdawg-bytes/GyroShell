using CommunityToolkit.WinUI.Connectivity;
using GyroShell.Library.Services;
using System;
using Windows.Networking.Connectivity;

namespace GyroShell.Services
{
    internal class NetworkService : INetworkService
    {
        public InternetConnection InternetType
        {
            get
            {
                ConnectionType type = NetworkHelper.Instance.ConnectionInformation.ConnectionType;

                return type switch
                {
                    ConnectionType.Ethernet => InternetConnection.Wired,
                    ConnectionType.WiFi => InternetConnection.Wireless,
                    ConnectionType.Data => InternetConnection.Data,
                    ConnectionType.Unknown => InternetConnection.Unknown,
                    _ => InternetConnection.Unknown
                };
            }
        }

        public bool IsInternetAvailable =>
            NetworkHelper.Instance.ConnectionInformation.IsInternetAvailable;
        public byte SignalStrength =>
            NetworkHelper.Instance.ConnectionInformation.SignalStrength.GetValueOrDefault(0);

        public event EventHandler InternetStatusChanged;

        public NetworkService()
        {
            NetworkInformation.NetworkStatusChanged += NetworkInformation_NetworkStatusChanged;
        }

        private void NetworkInformation_NetworkStatusChanged(object sender)
        {
            InternetStatusChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
