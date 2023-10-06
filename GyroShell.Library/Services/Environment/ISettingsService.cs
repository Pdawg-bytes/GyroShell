namespace GyroShell.Library.Services.Environment
{
    /// <summary>
    /// Defines a platform-agnostic service interface to get application settings.
    /// </summary>
    public interface ISettingsService
    {
        /// <summary>
        /// The symbol font style to use.
        /// </summary>
        public int? IconStyle { get; set; }

        /// <summary>
        /// Enable displaying seconds in the taskbar clock.
        /// </summary>
        public bool? EnableSeconds { get; set; }

        /// <summary>
        /// Enable displaying 24h time instead of 12h.
        /// </summary>
        public bool? EnableMilitaryTime { get; set; }

        /// <summary>
        /// The taskbar alignment to use.
        /// </summary>
        public int? TaskbarAlignment { get; set; }

        /// <summary>
        /// Enable customizing the taskbar background material.
        /// </summary>
        public bool? EnableCustomTransparency { get; set; }

        /// <summary>
        /// The alpha value of the taskbar background color.
        /// </summary>
        public byte? AlphaTint { get; set; }

        /// <summary>
        /// The red value of the taskbar background color.
        /// </summary>
        public byte? RedTint { get; set; }

        /// <summary>
        /// The green value of the taskbar background color.
        /// </summary>
        public byte? GreenTint { get; set; }

        /// <summary>
        /// The blue value of the taskbar background color.
        /// </summary>
        public byte? BlueTint { get; set; }

        /// <summary>
        /// The opacity of the luminosity layer in the taskbar background.
        /// </summary>
        public float? LuminosityOpacity { get; set; }

        /// <summary>
        /// The opacity of the tint layer in the taskbar background.
        /// </summary>
        public float? TintOpacity { get; set; }

        /// <summary>
        /// The type of material to use in the taskbar background.
        /// </summary>
        public int? TransparencyType { get; set; }

        /// <summary>
        /// The path to use when looking for modules.
        /// </summary>
        public string ModulesFolderPath { get; set; }
    }
}
