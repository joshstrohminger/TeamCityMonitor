using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace TeamCityMonitor.Converters
{
    class HideIfNullConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is string s && string.IsNullOrWhiteSpace(s)) value = null;
            return value == null ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}
