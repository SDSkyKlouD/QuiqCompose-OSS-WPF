using MahApps.Metro.Controls;

namespace SDSK.QuiqCompose.WinDesktop.Windows {
    public partial class SettingsWindow : MetroWindow {
        public SettingsWindow()
            => InitializeComponent();
        
        private void MouseDownDragMove(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            if(e.LeftButton == System.Windows.Input.MouseButtonState.Pressed) {
                DragMove();
            }
        }
    }
}
