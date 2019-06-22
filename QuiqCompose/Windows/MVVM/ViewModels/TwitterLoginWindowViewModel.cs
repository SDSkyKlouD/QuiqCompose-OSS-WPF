using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using SDSK.QuiqCompose.WinDesktop.Classes;
using SDSK.QuiqCompose.WinDesktop.Classes.Helpers;
using SDSK.QuiqCompose.WinDesktop.Windows.MVVM.DataTypes;
using Tweetinvi;
using Tweetinvi.Models;

namespace SDSK.QuiqCompose.WinDesktop.Windows.MVVM.ViewModels {
    public sealed class TwitterLoginWindowViewModel : INotifyPropertyChanged {
        #region INotifyPropertyChanged interface
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        #endregion

        #region ViewModel instance
        internal static TwitterLoginWindowViewModel Instance { get; private set; }
        #endregion

        #region Bindings
        private string _authPinText = string.Empty;
        public string AuthPinText {
            get => _authPinText;
            set {
                _authPinText = value;
                OnPropertyChanged(nameof(AuthPinText));
            }
        }

        public ICommand TwitterAuthCommandBinding { get; private set; }
        public ICommand CloseCommandBinding { get; } = new CommandHelper.CloseWindowCommand();
        public ICommand ReloadCommandBinding { get; } = new CommandHelper.BasicActionCommand(() => {
            Instance.TwitterAuthCommandBinding = Instance.InitTwitterAuthentication();
            Instance.OnPropertyChanged(nameof(Instance.TwitterAuthCommandBinding));
        });
        #endregion

        #region Constructor
        public TwitterLoginWindowViewModel() {
            Instance = this;

            if(!DesignerProperties.GetIsInDesignMode(new DependencyObject())) {
                TwitterAuthCommandBinding = InitTwitterAuthentication();
            }
        }
        #endregion

        #region Private functions
        private ICommand InitTwitterAuthentication() {
            var authContext = AuthFlow.InitAuthentication(ApplicationData.Instance.TwitterApp.GetTwitterCredentials());

            if(authContext != null) {
                ProcessStartHelper.StartProcess(authContext.AuthorizationURL, () => {
                    Clipboard.SetText(authContext.AuthorizationURL);
                    MessageBox.Show("Unable to start system default web browser. Authorization URL was copied to the clipboard.");
                });
            }

            return new TwitterAuthCommand(authContext);
        }
        #endregion

        #region ICommand binding classes
        public class TwitterAuthCommand : ICommand {
#pragma warning disable CS0067
            public event EventHandler CanExecuteChanged;
#pragma warning restore CS0067
            private readonly IAuthenticationContext _authContext;

            public TwitterAuthCommand(IAuthenticationContext authContext)
                => _authContext = authContext;

            public bool CanExecute(object parameter) => true;

            public void Execute(object parameter) {
                var paramTuple = parameter as Tuple<string, DependencyObject>;
                var credsSetupTuple = TweetinviHelper.SetCredentialsWithPin(_authContext, paramTuple.Item1);

                if(credsSetupTuple.Item1 && credsSetupTuple.Item2 is IAuthenticatedUser authedUser) {
                    var cwvmInstance = ComposeWindowViewModel.Instance;
                    
                    if(ConfigurationManager.ConfigurationInstance.FindAccountById(authedUser.Id) == null) {
                        var accountInfo = new CTwitterAccountInformation() {
                            Nickname = authedUser.Name,
                            ScreenName = authedUser.ScreenName,
                            AccountID = authedUser.Id,
                            ProfileImageUrl = authedUser.ProfileImageUrl400x400,
                            TokenInfo = new CTwitterAccountTokenInformation() {
                                AccessToken = authedUser.Credentials.AccessToken,
                                AccessTokenSecret = authedUser.Credentials.AccessTokenSecret
                            }
                        };

                        ConfigurationManager.ConfigurationInstance.AddAccount(accountInfo);
                        ConfigurationManager.Save();

                        var itemToInsert = new ComposeWindowAccountComboBoxItem() {
                            AccountID = accountInfo.AccountID,
                            Nickname = accountInfo.Nickname,
                            ScreenName = accountInfo.ScreenName,
                            ProfileImageUrl = accountInfo.ProfileImageUrl
                        };
                        cwvmInstance.AccountComboBoxItemsSource.Add(itemToInsert);
                        cwvmInstance.AccountComboBoxSelectedItem = itemToInsert;
                    } else {
                        MessageBox.Show(LocalizeHelper.GetLocalizedString("TwitterLoginWindow_Messages_AuthedBefore"));
                    }

                    (paramTuple.Item2 as Window)?.Close();
                } else {
                    MessageBox.Show(LocalizeHelper.GetLocalizedString("TwitterLoginWindow_Messages_Failed"));
                    Instance.AuthPinText = string.Empty;
                    Instance.ReloadCommandBinding.Execute(null);    // Reload authentication
                }
            }
        }
        #endregion
    }
}
