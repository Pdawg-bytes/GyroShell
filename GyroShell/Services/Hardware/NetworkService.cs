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

using System;
using Windows.Networking.Connectivity;
using GyroShell.Library.Services.Hardware;

namespace GyroShell.Services.Hardware
{
    internal class NetworkService : INetworkService, IDisposable
    {
        public InternetConnection InternetType
        {
            get
            {
                ConnectionProfile profile = NetworkInformation.GetInternetConnectionProfile();

                if (profile == null)
                    return InternetConnection.Unknown;

                switch (profile.NetworkAdapter.IanaInterfaceType)
                {
                    case 6:
                        return InternetConnection.Wired;
                    case 71:
                        return InternetConnection.Wireless;
                    case 243:
                    case 244:
                        return InternetConnection.Data;
                    default:
                        return InternetConnection.Unknown;
                }
            }
        }

        public bool IsInternetAvailable
        {
            get
            {
                ConnectionProfile profile = NetworkInformation.GetInternetConnectionProfile();
                return profile != null && profile.GetNetworkConnectivityLevel() == NetworkConnectivityLevel.InternetAccess;
            }
        }

        public byte SignalStrength
        {
            get
            {
                ConnectionProfile profile = NetworkInformation.GetInternetConnectionProfile();
                return profile.GetSignalBars() ?? 0;
            }
        }

        public event EventHandler InternetStatusChanged;

        public NetworkService()
        {
            NetworkInformation.NetworkStatusChanged += NetworkInformation_NetworkStatusChanged;
        }

        private void NetworkInformation_NetworkStatusChanged(object sender)
        {
            InternetStatusChanged?.Invoke(this, EventArgs.Empty);
        }


        public void Dispose()
        {
            NetworkInformation.NetworkStatusChanged -= NetworkInformation_NetworkStatusChanged;
        }
    }
}