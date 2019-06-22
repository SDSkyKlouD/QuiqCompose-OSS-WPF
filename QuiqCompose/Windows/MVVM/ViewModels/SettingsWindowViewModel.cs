using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using SDSK.QuiqCompose.WinDesktop.Classes;
using SDSK.QuiqCompose.WinDesktop.Classes.Helpers;
using WPFLocalizeExtension.Providers;

namespace SDSK.QuiqCompose.WinDesktop.Windows.MVVM.ViewModels {
    public sealed class SettingsWindowViewModel : INotifyPropertyChanged {
        #region INotifyPropertyChanged interface
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        #endregion

        #region Bindings
        #region UI behaviors
        public ICommand CloseWindowCommandBinding { get; } = new CommandHelper.CloseWindowCommand();
        #endregion

        #region Settings
        private bool _topMostChecked =
            ConfigurationManager.IsConfigurationReady() ?
                ConfigurationManager.ConfigurationInstance.UIConfigurations.TopMost : false;
        public bool TopMostChecked {
            get => _topMostChecked;
            set {
                _topMostChecked = value;
                ComposeWindowViewModel.Instance.TopMost = value;

                OnPropertyChanged(nameof(TopMostChecked));
            }
        }

        private bool _openProfilePageOnAccountClickChecked =
            ConfigurationManager.IsConfigurationReady() ?
                ConfigurationManager.ConfigurationInstance.UIConfigurations.OpenProfilePageOnAccountClick : false;
        public bool OpenProfilePageOnAccountClickChecked {
            get => _openProfilePageOnAccountClickChecked;
            set {
                _openProfilePageOnAccountClickChecked = value;
                ComposeWindowViewModel.Instance.AccountComboBoxOpenUserWithBrowserToolTipText = value ? LocalizeHelper.GetLocalizedString("ComposeWindow_OpenAccountWithBrowser_ToolTip") : string.Empty;

                OnPropertyChanged(nameof(OpenProfilePageOnAccountClickChecked));
            }
        }

        private bool _showProfileImageChecked =
            ConfigurationManager.IsConfigurationReady() ?
                ConfigurationManager.ConfigurationInstance.UIConfigurations.ShowProfileImage : true;
        public bool ShowProfileImageChecked {
            get => _showProfileImageChecked;
            set {
                _showProfileImageChecked = value;
                ComposeWindowViewModel.Instance.AccountComboBoxShowProfileImage = value;

                OnPropertyChanged(nameof(ShowProfileImageChecked));
            }
        }

        private int _composeWindowOpacityValue =
            ConfigurationManager.IsConfigurationReady() ?
                (int) (ConfigurationManager.ConfigurationInstance.UIConfigurations.ComposeWindowOpacity * 100) : 100;
        public int ComposeWindowOpacityValue {
            get => _composeWindowOpacityValue;
            set {
                float normalizedValue = (float) value / 100;

                _composeWindowOpacityValue = value;

                ComposeWindowViewModel.Instance.WindowOpacity = normalizedValue;
                ComposeSubWindowViewModel.Instance.WindowOpacity = normalizedValue;
                ConfigurationManager.ConfigurationInstance.UIConfigurations.ComposeWindowOpacity = normalizedValue;
                ConfigurationManager.Save();

                OnPropertyChanged(nameof(ComposeWindowOpacityValue));
            }
        }

        private int _languagesSelectedIndex = -1;
        public int LanguagesSelectedIndex {
            get => _languagesSelectedIndex;
            set {
                _languagesSelectedIndex = value;
                LocalizeHelper.SetCurrentUICulture(LanguagesItemsSource[value].ItemCultureInfo);
                ComposeWindowViewModel.Instance.UpdateUiByComposeAvailability();
                ConfigurationManager.ConfigurationInstance.UIConfigurations.Language = LanguagesItemsSource[value].ItemCultureInfo.Name;
                ConfigurationManager.Save();

                OnPropertyChanged(nameof(LanguagesSelectedIndex));
            }
        }

        private bool _saveLastAccountPositionChecked =
            ConfigurationManager.IsConfigurationReady() ?
                ConfigurationManager.ConfigurationInstance.UIConfigurations.SaveLastAccountPosition : true;
        public bool SaveLastAccountPositionChecked {
            get => _saveLastAccountPositionChecked;
            set {
                _saveLastAccountPositionChecked = value;
                
                if(value) {
                    if(ComposeWindowViewModel.Instance != null) {
                        ConfigurationManager.ConfigurationInstance.UIConfigurations.LastAccountPosition =
                            ComposeWindowViewModel.Instance.AccountComboBoxSelectedIndex;
                    } else {
                        ConfigurationManager.ConfigurationInstance.UIConfigurations.LastAccountPosition = 0;
                    }
                } else {
                    ConfigurationManager.ConfigurationInstance.UIConfigurations.LastAccountPosition = -1;
                }

                OnPropertyChanged(nameof(SaveLastAccountPositionChecked));
            }
        }

        public ObservableCollection<LanguageListItem> LanguagesItemsSource { get; } = new ObservableCollection<LanguageListItem>();

        public ICommand TopMostCommand { get; } = new CommandHelper.UiConfigurationBoolCommand("TopMost");
        public ICommand OpenProfilePageOnAccountClickCommand { get; } = new CommandHelper.UiConfigurationBoolCommand("OpenProfilePageOnAccountClick");
        public ICommand ShowProfileImageCommand { get; } = new CommandHelper.UiConfigurationBoolCommand("ShowProfileImage");
        public ICommand SaveLastAccountPositionCommand { get; } = new CommandHelper.UiConfigurationBoolCommand("SaveLastAccountPosition");

        public ICommand ResetConfigurationCommandBinding { get; } = new CommandHelper.BasicActionCommand(async () => {
            var confirmMessageBoxResult
                = MessageBox.Show(LocalizeHelper.GetLocalizedString("SettingsWindow_BottomBar_Reset_Message"),
                    LocalizeHelper.GetLocalizedString("SettingsWindow_BottomBar_Reset"),
                    MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No);

            if(confirmMessageBoxResult == MessageBoxResult.Yes) {
                ConfigurationManager.ConfigurationInstance.UIConfigurations = new CUserInterfaceSettings();
                await ConfigurationManager.SaveAsync();

                ProcessStartHelper.StartProcess(Application.ResourceAssembly.Location);
                Application.Current.Shutdown();
            }
        });
        #endregion
        #endregion

        #region Constructor
        public SettingsWindowViewModel() {
            foreach(var culture in ResxLocalizationProvider.Instance.AvailableCultures) {
                if(culture.LCID == 0x7F) {
                    LanguagesItemsSource.Add(new LanguageListItem(new CultureInfo("en")));
                } else {
                    LanguagesItemsSource.Add(new LanguageListItem(culture));
                }
            }

            CultureInfo currentCulture = new CultureInfo(Thread.CurrentThread.CurrentUICulture.Name);
            if(ConfigurationManager.IsConfigurationReady()) {
                currentCulture = new CultureInfo(ConfigurationManager.ConfigurationInstance.UIConfigurations.Language);
            }

            foreach(var item in LanguagesItemsSource) {
                if(item.ItemCultureInfo.Equals(currentCulture) || currentCulture.Parent.Equals(item)
                    || item.ItemCultureInfo.ThreeLetterWindowsLanguageName.Equals(currentCulture.ThreeLetterWindowsLanguageName)) {
                    _languagesSelectedIndex = LanguagesItemsSource.IndexOf(item);
                }
            }

            if(_languagesSelectedIndex == -1) {
                _languagesSelectedIndex = 0;
            }
        }
        #endregion

        #region Internal classes
        public sealed class LanguageListItem {
            public CultureInfo ItemCultureInfo { get; }
            public string CultureNativeName { get; }
            public string CultureEnglishName { get; }

            public LanguageListItem(CultureInfo cultureInfo) {
                ItemCultureInfo = cultureInfo;
                CultureNativeName = cultureInfo.NativeName;
                CultureEnglishName = cultureInfo.EnglishName;
            }

            public override string ToString() => ItemCultureInfo.NativeName;
        }
        #endregion
    }
}
