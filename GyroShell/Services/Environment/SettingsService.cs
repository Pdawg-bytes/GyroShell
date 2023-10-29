using GyroShell.Library.Services.Environment;
using Windows.Storage;

namespace GyroShell.Services.Environment
{
    public class SettingsService : ISettingsService
    {
        private readonly ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;

        public int IconStyle
        {
            get => localSettings.Values["iconStyle"] as int? != null ? (int)localSettings.Values["iconStyle"] : 0;
            set => localSettings.Values["iconStyle"] = value;
        }

        public bool EnableSeconds
        {
            get => localSettings.Values["isSeconds"] as bool? != null ? (bool)localSettings.Values["isSeconds"] : false;
            set => localSettings.Values["isSeconds"] = value;
        }

        public bool EnableMilitaryTime
        {
            get => localSettings.Values["is24HR"] as bool? != null ? (bool)localSettings.Values["is24HR"] : false;
            set => localSettings.Values["is24HR"] = value;
        }

        public int TaskbarAlignment
        {
            get => localSettings.Values["tbAlignment"] as int? != null ? (int)localSettings.Values["tbAlignment"] : 0;
            set => localSettings.Values["tbAlignment"] = value;
        }

        public bool EnableCustomTransparency
        {
            get => localSettings.Values["isCustomTransparency"] as bool? != null ? (bool)localSettings.Values["isCustomTransparency"] : false;
            set => localSettings.Values["isCustomTransparency"] = value;
        }

        public byte AlphaTint
        {
            get => localSettings.Values["aTint"] as byte? != null ? (byte)localSettings.Values["aTint"] : (byte)255;
            set => localSettings.Values["aTint"] = value;
        }

        public byte RedTint
        {
            get => localSettings.Values["rTint"] as byte? != null ? (byte)localSettings.Values["rTint"] : (byte)32;
            set => localSettings.Values["rTint"] = value;
        }

        public byte GreenTint
        {
            get => localSettings.Values["gTint"] as byte? != null ? (byte)localSettings.Values["gTint"] : (byte)32;
            set => localSettings.Values["gTint"] = value;
        }

        public byte BlueTint
        {
            get => localSettings.Values["bTint"] as byte? != null ? (byte)localSettings.Values["bTint"] : (byte)32;
            set => localSettings.Values["bTint"] = value;
        }

        public float LuminosityOpacity
        {
            get => localSettings.Values["luminOpacity"] as float? != null ? (float)localSettings.Values["luminOpacity"] : 0.95f;
            set => localSettings.Values["luminOpacity"] = value;
        }

        public float TintOpacity
        {
            get => localSettings.Values["tintOpacity"] as float? != null ? (float)localSettings.Values["tintOpacity"] : 0.0f;
            set => localSettings.Values["tintOpacity"] = value;
        }

        public int TransparencyType
        {
            get => localSettings.Values["transparencyType"] as int? != null ? (int)localSettings.Values["transparencyType"] : 0;
            set => localSettings.Values["transparencyType"] = value;
        }

        public string ModulesFolderPath
        {
            get => localSettings.Values["modulesFolderPath"] as string != null ? (string)localSettings.Values["modulesFolderPath"] : string.Empty;
            set => localSettings.Values["modulesFolderPath"] = value;
        }
    }
}
