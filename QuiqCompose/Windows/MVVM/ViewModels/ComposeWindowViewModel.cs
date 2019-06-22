using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using SDSK.QuiqCompose.Library.TwitterText;
using SDSK.QuiqCompose.WinDesktop.Classes;
using SDSK.QuiqCompose.WinDesktop.Classes.Helpers;
using SDSK.QuiqCompose.WinDesktop.Windows.MVVM.DataTypes;
using SDSK.QuiqCompose.WinDesktop.Windows.MVVM.Extensions;
using Tweetinvi;
using Tweetinvi.Models;

namespace SDSK.QuiqCompose.WinDesktop.Windows.MVVM.ViewModels {
    public sealed class ComposeWindowViewModel : INotifyPropertyChanged {
        #region INotifyPropertyChanged interface
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        #endregion

        #region ViewModel instance
        internal static ComposeWindowViewModel Instance { get; private set; } = null;
        #endregion

        #region Bindings
        private float _windowOpacity
            = ConfigurationManager.IsConfigurationReady()
                ? ConfigurationManager.ConfigurationInstance.UIConfigurations.ComposeWindowOpacity : 1;
        public float WindowOpacity {
            get => _windowOpacity;
            set {
                _windowOpacity = value;
                OnPropertyChanged(nameof(WindowOpacity));
            }
        }

        private bool _topMost
            = ConfigurationManager.IsConfigurationReady()
                ? ConfigurationManager.ConfigurationInstance.UIConfigurations.TopMost : false;
        public bool TopMost {
            get => _topMost;
            set {
                _topMost = value;
                OnPropertyChanged(nameof(TopMost));
            }
        }

        private double _textLength = 0;
        public double TextLength {
            get => _textLength;
            set {
                _textLength = value;
                OnPropertyChanged(nameof(TextLength));
            }
        }

        private string _composeText = string.Empty;
        public string ComposeText {
            get => _composeText;
            set {
                _composeText = value;
                ApplyTextLengthValue(value);
                OnPropertyChanged(nameof(ComposeText));
            }
        }

        private string _accountComboBoxOpenUserWithBrowserToolTipText
            = ConfigurationManager.IsConfigurationReady() ?
                (ConfigurationManager.ConfigurationInstance.UIConfigurations.OpenProfilePageOnAccountClick ?
                    LocalizeHelper.GetLocalizedString("ComposeWindow_OpenAccountWithBrowser_ToolTip") : string.Empty)
                : string.Empty;
        public string AccountComboBoxOpenUserWithBrowserToolTipText {
            get => _accountComboBoxOpenUserWithBrowserToolTipText;
            set {
                _accountComboBoxOpenUserWithBrowserToolTipText = value;
                OnPropertyChanged(nameof(AccountComboBoxOpenUserWithBrowserToolTipText));
            }
        }

        private ComposeWindowAccountComboBoxItem _accountComboBoxSelectedItem = null;
        public ComposeWindowAccountComboBoxItem AccountComboBoxSelectedItem {
            get => _accountComboBoxSelectedItem;
            set {
                _accountComboBoxSelectedItem = value;
                ChangeAccountByCurrentAccountSelection(value);

                OnPropertyChanged(nameof(AccountComboBoxSelectedItem));
            }
        }

        private int _accountComboBoxSelectedIndex = 0;
        public int AccountComboBoxSelectedIndex {
            get => _accountComboBoxSelectedIndex;
            set {
                _accountComboBoxSelectedIndex = value;

                if(ConfigurationManager.IsConfigurationReady()
                    && ConfigurationManager.ConfigurationInstance.UIConfigurations.SaveLastAccountPosition) {
                    ConfigurationManager.ConfigurationInstance.UIConfigurations.LastAccountPosition = value;
                    ConfigurationManager.Save();
                }

                OnPropertyChanged(nameof(AccountComboBoxSelectedIndex));
            }
        }

        private bool _accountComboBoxShowProfileImage
            = ConfigurationManager.IsConfigurationReady()
                ? ConfigurationManager.ConfigurationInstance.UIConfigurations.ShowProfileImage : true;
        public bool AccountComboBoxShowProfileImage {
            get => _accountComboBoxShowProfileImage;
            set {
                _accountComboBoxShowProfileImage = value;
                OnPropertyChanged(nameof(AccountComboBoxShowProfileImage));
            }
        }

        private ushort _composeTextFontSize
            = ConfigurationManager.IsConfigurationReady()
                ? ConfigurationManager.ConfigurationInstance.UIConfigurations.ComposeTextFontSize : (ushort) 15;
        public ushort ComposeTextFontSize {
            get => _composeTextFontSize;
            set {
                _composeTextFontSize = value;
                ConfigurationManager.ConfigurationInstance.UIConfigurations.ComposeTextFontSize = value;
                ConfigurationManager.Save();

                OnPropertyChanged(nameof(ComposeTextFontSize));
            }
        }

        private string _composeTextHint = LocalizeHelper.GetLocalizedString("ComposeWindow_TextBox_Hint" + (new Random().Next(1, 6)).ToString());
        public string ComposeTextHint {
            get => _composeTextHint;
            set {
                _composeTextHint = value;
                OnPropertyChanged(nameof(ComposeTextHint));
            }
        }

        private ComposeStatus _composeButtonComposeStatus = ComposeStatus.Ready;
        public ComposeStatus ComposeButtonComposeStatus {
            get => _composeButtonComposeStatus;
            set {
                _composeButtonComposeStatus = value;
                OnPropertyChanged(nameof(ComposeButtonComposeStatus));
            }
        }

        private bool _composeTextReadOnly = false;
        public bool ComposeTextReadOnly {
            get => _composeTextReadOnly;
            set {
                _composeTextReadOnly = value;
                OnPropertyChanged(nameof(ComposeTextReadOnly));
            }
        }

        private bool _composeTextEnabled = false;
        public bool ComposeTextEnabled {
            get => _composeTextEnabled;
            set {
                _composeTextEnabled = value;
                OnPropertyChanged(nameof(ComposeTextEnabled));
            }
        }

        private bool _composeButtonEnabled = true;
        public bool ComposeButtonEnabled {
            get => _composeButtonEnabled;
            set {
                _composeButtonEnabled = value;
                OnPropertyChanged(nameof(ComposeButtonEnabled));
            }
        }

        public ICommand ComposeCommandBinding {
            get {
                ComposeCommand command = new ComposeCommand();

                command.BeforePublishAction += () => {
                    ComposeSubWindowViewModel.Instance.MediaPanelEnabled = false;

                    ComposeTextReadOnly = true;
                    ComposeButtonEnabled = false;
                    ComposeButtonComposeStatus = ComposeStatus.Composing;
                };
                command.AfterPublishAction += async () => {
                    ComposeSubWindowViewModel.Instance.MediaList.Clear();
                    ComposeSubWindowViewModel.Instance.MediaPanelEnabled = true;

                    ComposeText = "";
                    ComposeTextReadOnly = false;
                    ComposeButtonEnabled = true;
                    ComposeButtonComposeStatus = ComposeStatus.Composed;
                    await Task.Delay(1000);
                    ComposeButtonComposeStatus = ComposeStatus.Ready;
                };
                command.PublishFailedAction += async () => {
                    ComposeSubWindowViewModel.Instance.MediaPanelEnabled = true;

                    ComposeTextReadOnly = false;
                    ComposeButtonEnabled = true;
                    ComposeButtonComposeStatus = ComposeStatus.Failed;
                    await Task.Delay(1000);
                    ComposeButtonComposeStatus = ComposeStatus.Ready;
                };

                return command;
            }
        }

        public ICommand ComposeTextFontSizeZoomInCommandBinding { get; } = new CommandHelper.BasicActionCommand(() => {
            Instance.ComposeTextFontSize++;
        });

        public ICommand ComposeTextFontSizeZoomOutCommandBinding { get; } = new CommandHelper.BasicActionCommand(() => {
            if(Instance.ComposeTextFontSize > 1) {
                Instance.ComposeTextFontSize--;
            }
        });

        public ICommand AccountComboBoxAddAccountButtonCommandBinding { get; } = new CommandHelper.ShowWindowCommand(typeof(TwitterLoginWindow), showWindowAsDialog: true);

        public ICommand AccountComboBoxOpenUserWithBrowserCommandBinding { get; } = new CommandHelper.BasicActionCommand(() => {
            if(ConfigurationManager.ConfigurationInstance.UIConfigurations.OpenProfilePageOnAccountClick) {
                string url = "https://twitter.com/" + Instance.AccountComboBoxSelectedItem.ScreenName;
                ProcessStartHelper.StartProcess(url, () => {
                    Clipboard.SetText(url);
                    MessageBox.Show("Profile URL was copied to the clipboard.");
                });
            }
        });

        public ICommand AccountComboBoxRemoveAccountButtonCommandBinding { get; } = new RemoveAccountCommand();
        public ICommand AppInfoCommandBinding { get; } = new CommandHelper.ShowWindowCommand(typeof(AppInfoWindow));
        public ICommand SettingsCommandBinding { get; } = new CommandHelper.ShowWindowCommand(typeof(SettingsWindow));

        private ObservableCollection<ComposeWindowAccountComboBoxItem> _accountComboBoxItemsSource = null;
        public ObservableCollection<ComposeWindowAccountComboBoxItem> AccountComboBoxItemsSource {
            get => _accountComboBoxItemsSource;
            set {
                var newCollection = value;
                newCollection.CollectionChanged += (object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) => UpdateUiByComposeAvailability();
                _accountComboBoxItemsSource = newCollection;

                OnPropertyChanged(nameof(AccountComboBoxItemsSource));
            }
        }
        #endregion

        #region Constructor
        public ComposeWindowViewModel() {
            Instance = this;
            AccountComboBoxItemsSource = new ObservableCollection<ComposeWindowAccountComboBoxItem>();

            if(ConfigurationManager.IsConfigurationReady()) {
                if(ConfigurationManager.ConfigurationInstance.AccountCount() > 0) {
                    if(AccountComboBoxItemsSource.Count <= 0) {
                        foreach(var item in ConfigurationManager.ConfigurationInstance.AccountInformations) {
                            AccountComboBoxItemsSource.Add(new ComposeWindowAccountComboBoxItem() {
                                AccountID = item.AccountID,
                                Nickname = item.Nickname,
                                ScreenName = item.ScreenName,
                                ProfileImageUrl = item.ProfileImageUrl
                            });
                        }

                        if(ConfigurationManager.ConfigurationInstance.UIConfigurations.SaveLastAccountPosition
                            && ConfigurationManager.ConfigurationInstance.UIConfigurations.LastAccountPosition != -1
                            && AccountComboBoxItemsSource.Count > ConfigurationManager.ConfigurationInstance.UIConfigurations.LastAccountPosition) {
                            AccountComboBoxSelectedItem = AccountComboBoxItemsSource[ConfigurationManager.ConfigurationInstance.UIConfigurations.LastAccountPosition];
                            AccountComboBoxSelectedIndex = ConfigurationManager.ConfigurationInstance.UIConfigurations.LastAccountPosition;
                        } else {
                            AccountComboBoxSelectedItem = AccountComboBoxItemsSource[0];
                            AccountComboBoxSelectedIndex = 0;
                        }
                    }
                }

                UpdateUiByComposeAvailability();
            }
        }
        #endregion

        #region Private functions
        internal void UpdateUiByComposeAvailability() {
            if(ConfigurationManager.IsConfigurationReady() && ConfigurationManager.ConfigurationInstance.AccountCount() > 0) {
                ComposeButtonEnabled = true;
                ComposeTextEnabled = true;
                ComposeButtonComposeStatus = ComposeStatus.Ready;
                RandomizeHintText();

                if(ComposeSubWindowViewModel.Instance != null) {
                    ComposeSubWindowViewModel.Instance.MediaPanelEnabled = true;
                }
            } else {
                ComposeButtonEnabled = false;
                ComposeTextEnabled = false;
                ComposeButtonComposeStatus = ComposeStatus.Failed;
                ComposeTextHint = LocalizeHelper.GetLocalizedString("ComposeWindow_TextBox_Hint_AddAccount");

                if(ComposeSubWindowViewModel.Instance != null) {
                    ComposeSubWindowViewModel.Instance.MediaPanelEnabled = false;
                }
            }
        }

        private void RandomizeHintText()
            => ComposeTextHint = LocalizeHelper.GetLocalizedString("ComposeWindow_TextBox_Hint" + (new Random().Next(1, 6)).ToString());

        private void ApplyTextLengthValue(string text) {
            var result = TwitterTextParser.ParseTweet(text);
            TextLength = result.WeightedLength;

            if(TextLength == 0) {
                RandomizeHintText();
            }
        }

        private void ChangeAccountByCurrentAccountSelection(ComposeWindowAccountComboBoxItem item) {
            if(item != null) {
                var currentItemInfo = ConfigurationManager.ConfigurationInstance.FindAccountById(item.AccountID);
                var decryptedAccessTokens = ConfigurationManager.ConfigurationInstance.DecryptAccessTokens(currentItemInfo);

                TweetinviHelper.SetCredentialsWithTokens(decryptedAccessTokens.Item1, decryptedAccessTokens.Item2);
            }
        }
        #endregion

        #region ICommand binding classes
        class ComposeCommand : ICommand {
#pragma warning disable CS0067
            public event EventHandler CanExecuteChanged;
#pragma warning restore CS0067
            private Action _beforePublishAction;
            private Action _afterPublishAction;
            private Action _publishFailedAction;

            public bool CanExecute(object parameter) => true;

            public async void Execute(object parameter) {
                string tweetText = parameter as string;

                if(TwitterTextParser.ParseTweet(tweetText).IsValid
                    && Tweet.CanBePublished(tweetText)) {
                    _beforePublishAction();

                    List<IMedia> uploadedMedias = new List<IMedia>();
                    foreach(var item in ComposeSubWindowViewModel.Instance.MediaList) {
                        byte[] imageData = File.ReadAllBytes(item.MediaImageSourceUri.LocalPath);
                        if(imageData != null) {
                            uploadedMedias.Add(await TweetinviHelper.UploadImageAsync(imageData));
                        }
                    }

                    var publishParam = Tweet.CreatePublishTweetParameters(tweetText, new Tweetinvi.Parameters.PublishTweetOptionalParameters() {
                        Medias = uploadedMedias
                    });

                    if(await TweetinviHelper.PublishTweetAsync(publishParam)) {
                        _afterPublishAction();
                        return;
                    }
                }

                _publishFailedAction();
            }

            public event Action BeforePublishAction {
                add => _beforePublishAction += value;
                remove => _beforePublishAction -= value;
            }

            public event Action AfterPublishAction {
                add => _afterPublishAction += value;
                remove => _afterPublishAction -= value;
            }

            public event Action PublishFailedAction {
                add => _publishFailedAction += value;
                remove => _publishFailedAction -= value;
            }
        }

        class RemoveAccountCommand : ICommand {
#pragma warning disable CS0067
            public event EventHandler CanExecuteChanged;
#pragma warning restore CS0067

            public bool CanExecute(object parameter) => true;

            public void Execute(object parameter) {
                if(parameter is ComposeWindowAccountComboBoxItem accountItem) {
                    var result = MessageBox.Show(LocalizeHelper.GetLocalizedString("ComposeWindow_RemoveAccount_Message"), LocalizeHelper.GetLocalizedString("Global_Warning"), MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No);

                    if(result == MessageBoxResult.Yes) {
                        ConfigurationManager.ConfigurationInstance.RemoveAccount(ConfigurationManager.ConfigurationInstance.FindAccountById(accountItem.AccountID));
                        Instance.AccountComboBoxItemsSource.Remove(accountItem);
                        Instance.AccountComboBoxSelectedIndex = 0;
                    }
                }
            }
        }
        #endregion
    }
}
