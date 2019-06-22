using System.ComponentModel;
using SDSK.QuiqCompose.WinDesktop.Classes.Helpers;

namespace SDSK.QuiqCompose.WinDesktop.Windows.MVVM.ViewModels {
    public sealed class PreparationWindowViewModel : INotifyPropertyChanged {
        #region INotifyPropertyChanged interface
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        #endregion

        #region ViewModel instance
        internal static PreparationWindowViewModel Instance { get; private set; } = null;
        #endregion

        #region Bindings
        private string _preparationStatusText = LocalizeHelper.GetLocalizedString("WindowTitle_Preparation");
        public string PreparationStatusText {
            get => _preparationStatusText;
            set {
                _preparationStatusText = value;
                OnPropertyChanged(nameof(PreparationStatusText));
            }
        }
        #endregion

        #region Constructor
        public PreparationWindowViewModel()
            => Instance = this;
        #endregion
    }
}
