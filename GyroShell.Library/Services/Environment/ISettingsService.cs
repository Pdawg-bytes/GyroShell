using System;

namespace GyroShell.Library.Services.Environment
{
    /// <summary>
    /// Defines a platform-agnostic service interface to get application settings.
    /// </summary>
    public interface ISettingsService
    {
        /// <summary>Gets a setting value associated with a specific key and casts it to the specified data type.</summary>
        /// <typeparam name="T">The data type to cast the setting value to.</typeparam>
        /// <param name="key">The unique key associated with the setting.</param>
        /// <returns>The setting value cast to the specified data type.</returns>
        /// <remarks>If the key is not found or the type does not match, a default value for the data type is returned.</remarks>
        public T GetSetting<T>(string key);

        /// <summary> Sets a setting with the specified key to the provided value.</summary>
        /// <typeparam name="T">The data type of the setting value.</typeparam>
        /// <param name="key">The unique key associated with the setting.</param>
        /// <param name="value">The value to set for the specified setting key.</param>
        public void SetSetting<T>(string key, T value);

        /// <summary>Removes a setting associated with the specified key.</summary>
        /// <param name="key">The unique key associated with the setting to be removed.</returns>
        /// <returns>True if the setting was successfully removed; otherwise, false if the key is not found.</returns>
        public bool RemoveSetting(string key);

        /// <summary>Adds a new setting with the specified key and value, or updates an existing setting with the same key.</summary>
        /// <typeparam name="T">The data type of the setting value.</typeparam>
        /// <param name="key">The unique key associated with the setting.</param>
        /// <param name="value">The value to set for the specified setting key.</param>
        public void AddSetting<T>(string key, T value);

        /// <summary>An event that is fired when a setting is updated in the local store.</summary>
        /// <remarks>Event payload contains the key of the changed setting.</remarks>
        public event EventHandler<string> SettingUpdated;

        /// <summary>An event that is fired when a new setting is added to the local store.</summary>
        /// <remarks>Event payload contains the key of the added setting.</remarks>
        public event EventHandler<string> SettingAdded;

        /// <summary>
        /// The symbol font style to use.
        /// </summary>
        /// <remarks>Abstraction of GetSetting, SetSetting.</remarks>
        public int IconStyle { get; set; }

        /// <summary>
        /// Enable displaying seconds in the taskbar clock.
        /// </summary>
        /// <remarks>Abstraction of GetSetting, SetSetting.</remarks>
        public bool EnableSeconds { get; set; }

        /// <summary>
        /// Enable displaying 24h time instead of 12h.
        /// </summary>
        /// <remarks>Abstraction of GetSetting, SetSetting.</remarks>
        public bool EnableMilitaryTime { get; set; }

        /// <summary>
        /// The taskbar alignment to use.
        /// </summary>
        /// <remarks>Abstraction of GetSetting, SetSetting.</remarks>
        public int TaskbarAlignment { get; set; }

        /// <summary>
        /// Enable customizing the taskbar background material.
        /// </summary>
        /// <remarks>Abstraction of GetSetting, SetSetting.</remarks>
        public bool EnableCustomTransparency { get; set; }

        /// <summary>
        /// The alpha value of the taskbar background color.
        /// </summary>
        /// <remarks>Abstraction of GetSetting, SetSetting.</remarks>
        public byte AlphaTint { get; set; }

        /// <summary>
        /// The red value of the taskbar background color.
        /// </summary>
        /// <remarks>Abstraction of GetSetting, SetSetting.</remarks>
        public byte RedTint { get; set; }

        /// <summary>
        /// The green value of the taskbar background color.
        /// </summary>
        /// <remarks>Abstraction of GetSetting, SetSetting.</remarks>
        public byte GreenTint { get; set; }

        /// <summary>
        /// The blue value of the taskbar background color.
        /// </summary>
        /// <remarks>Abstraction of GetSetting, SetSetting.</remarks>
        public byte BlueTint { get; set; }

        /// <summary>
        /// The opacity of the luminosity layer in the taskbar background.
        /// </summary>
        /// <remarks>Abstraction of GetSetting, SetSetting.</remarks>
        public float LuminosityOpacity { get; set; }

        /// <summary>
        /// The opacity of the tint layer in the taskbar background.
        /// </summary>
        /// <remarks>Abstraction of GetSetting, SetSetting.</remarks>
        public float TintOpacity { get; set; }

        /// <summary>
        /// The type of material to use in the taskbar background.
        /// </summary>
        /// <remarks>Abstraction of GetSetting, SetSetting.</remarks>
        public int TransparencyType { get; set; }

        /// <summary>
        /// The path to use when looking for modules.
        /// </summary>
        /// <remarks>Abstraction of GetSetting, SetSetting.</remarks>
        public string ModulesFolderPath { get; set; }
    }
}
