using GyroShell.Library.Services.Environment;
using System;
using Windows.Storage;

namespace GyroShell.Services.Environment
{
    public class SettingsService : ISettingsService
    {
        private readonly ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;

        public event EventHandler<string> SettingUpdated;
        public event EventHandler<string> SettingAdded;

        public T GetSetting<T>(string key)
        {
            if (localSettings.Values.TryGetValue(key, out var value) && value is T)
            {
                return (T)value;
            }
            return default;
        }

        public void SetSetting<T>(string key, T value)
        {
            localSettings.Values[key] = value;
            SettingUpdated?.Invoke(this, key);
        }

        public bool RemoveSetting(string key)
        {
            return localSettings.Values.Remove(key);
        }

        public void AddSetting<T>(string key, T value)
        {
            if (!localSettings.Values.ContainsKey(key))
            {
                localSettings.Values.Add(key, value);
                SettingAdded?.Invoke(this, key);
            }
        }

        public int IconStyle
        {
            get => GetSetting<int?>("iconStyle") ?? 0;
            set => SetSetting("iconStyle", value);
        }

        public bool EnableSeconds
        {
            get => GetSetting<bool?>("isSeconds") ?? false;
            set => SetSetting("isSeconds", value);
        }

        public bool EnableMilitaryTime
        {
            get => GetSetting<bool?>("is24HR") ?? false;
            set => SetSetting("is24HR", value);
        }

        public int TaskbarAlignment
        {
            get => GetSetting<int?>("tbAlignment") ?? 0;
            set => SetSetting("tbAlignment", value);
        }

        public bool EnableCustomTransparency
        {
            get => GetSetting<bool?>("isCustomTransparency") ?? false;
            set => SetSetting("isCustomTransparency", value);
        }

        public byte AlphaTint
        {
            get => GetSetting<byte?>("aTint") ?? 255;
            set => SetSetting("aTint", value);
        }

        public byte RedTint
        {
            get => GetSetting<byte?>("rTint") ?? 32; 
            set => SetSetting("rTint", value);
        }

        public byte GreenTint
        {
            get => GetSetting<byte?>("gTint") ?? 32;
            set => SetSetting("gTint", value);
        }

        public byte BlueTint
        {
            get => GetSetting<byte?>("bTint") ?? 32;
            set => SetSetting("bTint", value);
        }

        public float LuminosityOpacity
        {
            get => GetSetting<float?>("luminOpacity") ?? 0.96f;
            set => SetSetting("luminOpacity", value);
        }

        public float TintOpacity
        {
            get => GetSetting<float?>("tintOpacity") ?? 0.50f;
            set => SetSetting("tintOpacity", value);
        }

        public int TransparencyType
        {
            get => GetSetting<int?>("transparencyType") ?? 2;
            set => SetSetting("transparencyType", value);
        }

        public string ModulesFolderPath
        {
            get => GetSetting<string>("modulesFolderPath") ?? string.Empty;
            set => SetSetting("modulesFolderPath", value);
        }
    }
}
