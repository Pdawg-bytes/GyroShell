using GyroShell.Library.Services;
using Windows.Storage;

namespace GyroShell.Services
{
    internal class SettingsService : ISettingsService
    {
        private readonly ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;

        public int? IconStyle
        {
            get => localSettings.Values["iconStyle"] as int?;
            set => localSettings.Values["iconStyle"] = value;
        }

        public bool? EnableSeconds
        {
            get => localSettings.Values["isSeconds"] as bool?;
            set => localSettings.Values["isSeconds"] = value;
        }

        public bool? EnableMilitaryTime
        {
            get => localSettings.Values["is24HR"] as bool?;
            set => localSettings.Values["is24HR"] = value;
        }

        public int? TaskbarAlignment
        {
            get => localSettings.Values["tbAlignment"] as int?;
            set => localSettings.Values["tbAlignment"] = value;
        }

        public bool? EnableCustomTransparency
        {
            get => localSettings.Values["isCustomTransparency"] as bool?;
            set => localSettings.Values["isCustomTransparency"] = value;
        }

        public byte? AlphaTint
        {
            get => localSettings.Values["aTint"] as byte?;
            set => localSettings.Values["aTint"] = value;
        }

        public byte? RedTint
        {
            get => localSettings.Values["rTint"] as byte?;
            set => localSettings.Values["rTint"] = value;
        }

        public byte? GreenTint
        {
            get => localSettings.Values["gTint"] as byte?;
            set => localSettings.Values["gTint"] = value;
        }

        public byte? BlueTint
        {
            get => localSettings.Values["bTint"] as byte?;
            set => localSettings.Values["bTint"] = value;
        }

        public float? LuminosityOpacity
        {
            get => localSettings.Values["luminOpacity"] as float?;
            set => localSettings.Values["luminOpacity"] = value;
        }

        public float? TintOpacity
        {
            get => localSettings.Values["tintOpacity"] as float?;
            set => localSettings.Values["tintOpacity"] = value;
        }

        public int? TransparencyType
        {
            get => localSettings.Values["transparencyType"] as int?;
            set => localSettings.Values["transparencyType"] = value;
        }

        public string ModulesFolderPath
        {
            get => localSettings.Values["modulesFolderPath"] as string;
            set => localSettings.Values["modulesFolderPath"] = value;
        }
    }
}
