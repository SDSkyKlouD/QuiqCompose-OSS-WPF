using System.Windows;

namespace SDSK.QuiqCompose.WinDesktop.Windows.MVVM.Extensions {
    public sealed class ToolTipExtensions {
        #region ToolTipExtensions.ToolTipText
        public static readonly DependencyProperty ToolTipTextProperty =
            DependencyProperty.RegisterAttached("ToolTipText", typeof(string), typeof(ToolTipExtensions),
                new FrameworkPropertyMetadata(new PropertyChangedCallback(ToolTipTextPropertyChanged)));

        public static void SetToolTipText(UIElement element, string value)
            => element.SetValue(ToolTipTextProperty, value);

        public static string GetToolTipText(UIElement element)
            => (string) element.GetValue(ToolTipTextProperty);

        public static void ToolTipTextPropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e) {
            if((o != null)
               && (e.NewValue is string newText)
               && (o is FrameworkElement ofe)) {
                if(!string.IsNullOrWhiteSpace(newText.ToString())) {
                    ofe.SetValue(FrameworkElement.ToolTipProperty, newText);
                } else {
                    ofe.ClearValue(FrameworkElement.ToolTipProperty);
                }
            }
        }
        #endregion
    }
}
