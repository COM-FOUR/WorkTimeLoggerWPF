using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Shell;

namespace WorkTimeLogger.Resources
{
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
