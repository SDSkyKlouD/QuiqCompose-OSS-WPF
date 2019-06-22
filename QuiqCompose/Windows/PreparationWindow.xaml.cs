using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using SDSK.QuiqCompose.WinDesktop.Windows.MVVM.ViewModels;
using SDSK.QuiqCompose.WinDesktop.Windows.Subclasses;

namespace SDSK.QuiqCompose.WinDesktop.Windows {
    public partial class PreparationWindow : Window {
        public PreparationWindow() {
            InitializeComponent();
            ProcessPreparation();

            /* Below is for testing some window */
            // new UpdateFoundWindow(new Classes.API.VersionChecker.VersionInformationItem("39.39.39.39", "2039-08-31", "https://sdsk.one/", "https://sdsk.one/", "testing")).Show();
        }

        private void Window_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
            => DragMove();

        public async void ProcessPreparation() {
            if(!DesignerProperties.GetIsInDesignMode(new DependencyObject())) {
                await (new PreparationWork()).DoPreparationAsync();

                if(PreparationWork.PreparationDone) {
                    await Task.Run(() => {
                        System.Threading.Thread.Sleep(100);
                    });

                    Dispatcher.Invoke(() => {
                        ComposeWindow window = new ComposeWindow();
                        window.Top = Top - ((window.Height - Height) / 2);
                        window.Left = Left - ((window.Width - Width) / 2);
                        window.Show();
                        Application.Current.MainWindow = window;

                        Close();
                    });
                } else {
                    await Task.Run(() => {
                        System.Threading.Thread.Sleep(3000);

                        Dispatcher.Invoke(() => {
                            Close();
                        });
                    });
                }
            }
        }

        private void ChangeStatusText(string statusText)
            => PreparationWindowViewModel.Instance.PreparationStatusText = statusText;
    }
}
