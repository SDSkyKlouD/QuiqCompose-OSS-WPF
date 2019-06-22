using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;
using SDSK.QuiqCompose.WinDesktop.Classes;
using SDSK.QuiqCompose.WinDesktop.Classes.Helpers;
using SDSK.QuiqCompose.WinDesktop.Windows.MVVM.DataTypes;

namespace SDSK.QuiqCompose.WinDesktop.Windows.MVVM.ViewModels {
    public sealed class AppInfoWindowViewModel : INotifyPropertyChanged {
        #region INotifyPropertyChanged interface
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        #endregion

        #region Bindings
        public string VersionText { get; }
            = string.Format(LocalizeHelper.GetLocalizedString("AppInfoWindow_VersionFormat"), ApplicationData.Instance.AppVersion);

        public HashSet<AppInfoWindowOSLItem> OpenSourceLibrariesItemsSource { get; set; } = new HashSet<AppInfoWindowOSLItem>();

        public ICommand OpenBrowserCommandBinding { get; } = new CommandHelper.ProcessStartCommand();
        public ICommand CloseWindowCommandBinding { get; } = new CommandHelper.CloseWindowCommand();
        #endregion

        public AppInfoWindowViewModel() {
            OpenSourceLibrariesItemsSource = new HashSet<AppInfoWindowOSLItem>(JsonHelper.DeserializeEmbeddedFileToList<AppInfoWindowOSLItem>("/Resources/OpenSourceLibraries.json"));
        }
    }
}
