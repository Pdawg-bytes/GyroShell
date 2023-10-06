namespace GyroShell.Library.Services.Environment
{
    public interface ISettingsService
    {
        public int? IconStyle { get; set; }

        public bool? EnableSeconds { get; set; }
        public bool? EnableMilitaryTime { get; set; }

        public int? TaskbarAlignment { get; set; }

        public bool? EnableCustomTransparency { get; set; }

        public byte? AlphaTint { get; set; }
        public byte? RedTint { get; set; }
        public byte? GreenTint { get; set; }
        public byte? BlueTint { get; set; }

        public float? LuminosityOpacity { get; set; }
        public float? TintOpacity { get; set; }

        public int? TransparencyType { get; set; }

        public string ModulesFolderPath { get; set; }
    }
}
