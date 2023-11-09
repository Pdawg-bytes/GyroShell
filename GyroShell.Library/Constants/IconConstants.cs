using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GyroShell.Library.Constants
{
    public class IconConstants
    {
        /// <summary>
        /// The set of icons to represent a charging battery.
        /// </summary>
        public static string[] BatteryIconsCharge = { "\uEBAE", "\uEBAC", "\uEBAD", "\uEBAE", "\uEBAF", "\uEBB0", "\uEBB1", "\uEBB2", "\uEBB3", "\uEBB4", "\uEBB5" };

        /// <summary>
        /// The set of icons to represent a discharging battery.
        /// </summary>
        public static string[] BatteryIcons = { "\uEBA0", "\uEBA1", "\uEBA2", "\uEBA3", "\uEBA4", "\uEBA5", "\uEBA6", "\uEBA7", "\uEBA8", "\uEBA9", "\uEBAA" };

        /// <summary>
        /// The set of icons that represent a Wi-Fi connection.
        /// </summary>
        public static string[] WiFiIcons = { "\uE871", "\uE872", "\uE873", "\uE874", "\uE701" };

        /// <summary>
        /// The set of icons that represent a cellular connection.
        /// </summary>
        public static string[] DataIcons = { "\uEC37", "\uEC38", "\uEC39", "\uEC3A", "\uEC3B" };
    }
}
