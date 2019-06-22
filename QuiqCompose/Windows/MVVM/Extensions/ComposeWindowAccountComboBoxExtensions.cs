using System.Windows;
using System.Windows.Input;

namespace SDSK.QuiqCompose.WinDesktop.Windows.MVVM.Extensions {
    public sealed class ComposeWindowAccountComboBoxExtensions {
        #region ComposeWindowAccountComboBoxExtensions.AddAccountButtonCommand
        public static readonly DependencyProperty AddAccountButtonCommandProperty =
            DependencyProperty.RegisterAttached("AddAccountButtonCommand",
                typeof(ICommand), typeof(ComposeWindowAccountComboBoxExtensions));

        public static void SetAddAccountButtonCommand(UIElement element, ICommand value)
            => element.SetValue(AddAccountButtonCommandProperty, value);

        public static ICommand GetAddAccountButtonCommand(UIElement element)
            => (ICommand) element.GetValue(AddAccountButtonCommandProperty);
        #endregion

        #region ComposeWindowAccountComboBoxExtensions.OpenUserAccountWithBrowserCommand
        public static readonly DependencyProperty OpenUserAccountWithBrowserCommandProperty =
            DependencyProperty.RegisterAttached("OpenUserAccountWithBrowserCommand",
                typeof(ICommand), typeof(ComposeWindowAccountComboBoxExtensions));

        public static void SetOpenUserAccountWithBrowserCommand(UIElement element, ICommand value)
            => element.SetValue(OpenUserAccountWithBrowserCommandProperty, value);

        public static ICommand GetOpenUserAccountWithBrowserCommand(UIElement element)
            => (ICommand) element.GetValue(OpenUserAccountWithBrowserCommandProperty);
        #endregion

        #region ComposeWindowAccountComboBoxExtensions.RemoveAccountButtonCommand
        public static readonly DependencyProperty RemoveAccountButtonCommandProperty =
            DependencyProperty.RegisterAttached("RemoveAccountButtonCommand",
                typeof(ICommand), typeof(ComposeWindowAccountComboBoxExtensions));

        public static void SetRemoveAccountButtonCommand(UIElement element, ICommand value)
            => element.SetValue(RemoveAccountButtonCommandProperty, value);

        public static ICommand GetRemoveAccountButtonCommand(UIElement element)
            => (ICommand) element.GetValue(RemoveAccountButtonCommandProperty);
        #endregion

        #region ComposeWindowAccountComboBoxExtensions.ProfileImageVisibility
        public static readonly DependencyProperty ProfileImageVisibilityProperty =
            DependencyProperty.RegisterAttached("ProfileImageVisibility",
                typeof(Visibility), typeof(ComposeWindowAccountComboBoxExtensions));

        public static void SetProfileImageVisibility(UIElement element, Visibility value)
            => element.SetValue(ProfileImageVisibilityProperty, value);

        public static Visibility GetProfileImageVisibility(UIElement element)
            => (Visibility) element.GetValue(ProfileImageVisibilityProperty);
        #endregion
    }
}
