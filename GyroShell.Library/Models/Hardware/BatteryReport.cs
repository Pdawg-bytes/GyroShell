#region Copyright (BSD 3-Clause License)
/*
 * GyroShell - A modern, extensible, fast, and customizable shell platform.
 * Copyright 2022-2024 Pdawg
 *
 * Licensed under the BSD 3-Clause License.
 * SPDX-License-Identifier: BSD-3-Clause
 */
#endregion

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
