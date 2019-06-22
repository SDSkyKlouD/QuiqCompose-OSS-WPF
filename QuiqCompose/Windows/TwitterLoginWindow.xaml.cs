using System.Text.RegularExpressions;
using System.Windows.Input;
using MahApps.Metro.Controls;

namespace SDSK.QuiqCompose.WinDesktop.Windows {
    public partial class TwitterLoginWindow : MetroWindow {
        public TwitterLoginWindow()
            => InitializeComponent();

        private void PINTextBoxAcceptOnlyNumbers(object sender, TextCompositionEventArgs e)
            => e.Handled = new Regex("[^0-9]+").IsMatch(e.Text);
    }
}
