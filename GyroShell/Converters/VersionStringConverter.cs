using Microsoft.UI.Xaml.Data;
using System;

namespace GyroShell.Converters
{
    internal class VersionStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is Version version)
            {
                return $"Version: {version}";
            }

            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException("Converting back to string from Version is not implemented.");
        }
    }
}