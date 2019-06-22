using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using MahApps.Metro.IconPacks;
using SDSK.QuiqCompose.WinDesktop.Windows.MVVM.Extensions;

namespace SDSK.QuiqCompose.WinDesktop.Windows.MVVM.Converters {
    public sealed class ComposeButtonContentConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if(value is ComposeStatus status) {
                switch(status) {
                    case ComposeStatus.Ready:
                        return new PackIconMaterial {
                            Kind = PackIconMaterialKind.Send
                        };
                    case ComposeStatus.Composing:
                        return new MahApps.Metro.Controls.ProgressRing {
                            Width = 32,
                            Height = 32
                        };
                    case ComposeStatus.Composed:
                        return new PackIconMaterial {
                            Kind = PackIconMaterialKind.Check
                        };
                    case ComposeStatus.Failed:
                        return new PackIconMaterial {
                            Kind = PackIconMaterialKind.Close
                        };
                }
            }

            return new PackIconMaterial {
                Kind = PackIconMaterialKind.Send
            };  // Default status in design mode
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            if(value is FrameworkElement element) {
                if(element is PackIconMaterial materialIconElement) {
                    switch(materialIconElement.Kind) {
                        case PackIconMaterialKind.Send:
                            return ComposeStatus.Ready;
                        case PackIconMaterialKind.Check:
                            return ComposeStatus.Composed;
                        case PackIconMaterialKind.Close:
                            return ComposeStatus.Failed;
                    }
                } else if(element is MahApps.Metro.Controls.ProgressRing) {
                    return ComposeStatus.Composing;
                }
            }

            return null;
        }
    }
}
