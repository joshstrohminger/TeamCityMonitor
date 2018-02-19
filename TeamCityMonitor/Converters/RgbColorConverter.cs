using System;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using BlinkStickUniversal;

namespace TeamCityMonitor.Converters
{
    public class RgbColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is RgbColor c)
            {
                return new Color {R = c.R, G = c.G, B = c.B};
            }
            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value is Color c)
            {
                return new RgbColor(c.R, c.G, c.B);
            }
            return DependencyProperty.UnsetValue;
        }
    }
}
