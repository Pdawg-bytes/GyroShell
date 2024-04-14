#region Copyright (BSD 3-Clause License)
/*
 * GyroShell - A modern, extensible, fast, and customizable shell platform.
 * Copyright 2022-2024 Pdawg
 *
 * Licensed under the BSD 3-Clause License.
 * SPDX-License-Identifier: BSD-3-Clause
 */
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