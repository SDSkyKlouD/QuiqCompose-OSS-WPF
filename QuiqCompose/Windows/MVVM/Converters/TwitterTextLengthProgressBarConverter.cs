using System;
using System.Globalization;
using System.Windows.Data;

namespace SDSK.QuiqCompose.WinDesktop.Windows.MVVM.Converters {
    public sealed class TwitterTextLengthProgressBarConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if(value is double currentValue) {
                if(currentValue > 260) {
                    return TwitterTextLengthLevels.NearlyLimit;
                } else if(currentValue > 220) {
                    return TwitterTextLengthLevels.Warning;
                } else {
                    return TwitterTextLengthLevels.Normal;
                }
            } else {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new InvalidOperationException("TwitterTextLengthProgressBarConverter doesn't support converting back the value.");
    }

    public enum TwitterTextLengthLevels {
        Normal,
        Warning,
        NearlyLimit
    }
}
