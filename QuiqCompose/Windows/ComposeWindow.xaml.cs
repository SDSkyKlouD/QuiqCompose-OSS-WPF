using MahApps.Metro.Controls;

namespace SDSK.QuiqCompose.WinDesktop.Windows {
    public partial class ComposeWindow : MetroWindow {
        ComposeSubWindow _subWindow;
        const int _subWindowMargin = 10;

        public ComposeWindow() => InitializeComponent();

        private void StartSubWindow(object sender, System.EventArgs e) {
            _subWindow = new ComposeSubWindow {
                Top = Top + Height + _subWindowMargin,
                Left = Left,
                Owner = this
            };
            _subWindow.Show();
        }

        private void CloseSubWindow(object sender, System.ComponentModel.CancelEventArgs e) {
            _subWindow?.Close();
        }

        private void MoveSubWindow(object sender, System.EventArgs e) {
            if(_subWindow != null) {
                _subWindow.Top = Top + Height + _subWindowMargin;
                _subWindow.Left = Left;
            }
        }

        private void SizeSubWindow(object sender, System.Windows.SizeChangedEventArgs e) {
            MoveSubWindow(sender, e);
        }
    }
}
