using System;
using System.Windows;
using System.Globalization;
using System.Windows.Data;

namespace CF.WPF.Converters
{
    /// <summary>
    /// converter from boolean to FontWeight, for binding purposes
    /// </summary>
    public class BoolToFontWeightConverter : DependencyObject, IValueConverter
    {
        public object Convert(object value, Type targetType,
                              object parameter, CultureInfo culture)
        {
            return ((bool)value) ? FontWeights.Bold : FontWeights.Normal;
        }

        public object ConvertBack(object value, Type targetType,
                                  object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
