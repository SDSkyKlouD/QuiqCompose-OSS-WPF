using System.Windows;

namespace SDSK.QuiqCompose.WinDesktop.Windows.MVVM.Extensions {
    public sealed class ComposeWindowComposeButtonExtensions {
        #region ComposeWindowComposeButtonExtensions.ComposeStatus
        public static readonly DependencyProperty ComposeStatusProperty =
            DependencyProperty.RegisterAttached("ComposeStatus",
                typeof(ComposeStatus), typeof(ComposeWindowComposeButtonExtensions),
                new PropertyMetadata(ComposeStatus.Ready));

        public static void SetComposeStatus(UIElement element, ComposeStatus value)
            => element.SetValue(ComposeStatusProperty, value);

        public static ComposeStatus GetComposeStatus(UIElement element)
            => (ComposeStatus) element.GetValue(ComposeStatusProperty);
        #endregion
    }

    public enum ComposeStatus {
        Ready,
        Composing,
        Composed,
        Failed
    }
}
