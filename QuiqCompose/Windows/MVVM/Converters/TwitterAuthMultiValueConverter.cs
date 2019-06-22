using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace SDSK.QuiqCompose.WinDesktop.Windows.MVVM.Converters {
    public sealed class TwitterAuthMultiValueConverter : IMultiValueConverter {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
            => new Tuple<string, DependencyObject>(values[0] as string, values[1] as DependencyObject);

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) {
            if(value is Tuple<string, DependencyObject> tupleValue) {
                return new object[] {
                    tupleValue.Item1,
                    tupleValue.Item2
                };
            } else {
                return null;
            }
        }
    }
}
