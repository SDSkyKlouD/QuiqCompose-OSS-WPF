using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using SDSK.QuiqCompose.WinDesktop.Classes;
using SDSK.QuiqCompose.WinDesktop.Classes.Helpers;
using SDSK.QuiqCompose.WinDesktop.Windows.MVVM.ViewModels;

namespace SDSK.QuiqCompose.WinDesktop.Windows.Subclasses {
    internal sealed class PreparationWork {
        public static bool PreparationDone { get; private set; } = false;

        private List<IPreparationWork> _preparationFunctionList = new List<IPreparationWork>();

        public PreparationWork() {
            _preparationFunctionList.Add(new CheckForInternetConnection());
            _preparationFunctionList.Add(new LoadConfigurations());
            _preparationFunctionList.Add(new UpdateAccountInformation());
        }

        public Task DoPreparationAsync()
            => Task.Run(() => {
                bool failed = false;

                foreach(var item in _preparationFunctionList) {
                    try {
                        if((item.ConfigurationName == null
                            || ((bool) ConfigurationManager.ConfigurationInstance?.UIConfigurations[item.ConfigurationName]))
                            && item.Condition()) {
                            ChangeStatusTextInPreparationWindow(item.StatusText);
                            item.Work();
                        }
                    } catch(Exception e) {
                        System.Diagnostics.Debug.WriteLine(e.Message);
                        ChangeStatusTextInPreparationWindow(item.FailText);

                        if(item.IsCritical) {
                            failed = true;
                            return;
                        }
                    }
                }

                if(!failed) {
                    PreparationDone = true;
                } else {
                    return;
                }
            });

        private void ChangeStatusTextInPreparationWindow(string statusText)
            => PreparationWindowViewModel.Instance.PreparationStatusText = statusText;

#region Preparation works
        class CheckForInternetConnection : IPreparationWork {
            public bool IsCritical => true;

            public string StatusText => LocalizeHelper.GetLocalizedString("PreparationWindow_Status_CheckForInternetConnection");

            public string FailText => LocalizeHelper.GetLocalizedString("PreparationWindow_Status_CheckForInternetConnection_Failed");

            public string ConfigurationName => null;

            public void Work() {
                HttpWebRequest request = WebRequest.CreateHttp("https://www.gstatic.com/generate_204");
                request.Timeout = 10 * 1000;  // 10 second

                using(request.GetResponse()) { ; }
            }

            public bool Condition() => true;
        }

        class LoadConfigurations : IPreparationWork {
            public bool IsCritical => false;

            public string StatusText => LocalizeHelper.GetLocalizedString("PreparationWindow_Status_LoadConfigurations");

            public string FailText => LocalizeHelper.GetLocalizedString("PreparationWindow_Status_LoadConfigurations_Failed");

            public string ConfigurationName => null;

            public void Work() {
                try {
                    if(!Directory.Exists(ApplicationData.Instance.GetConfigurationFileRootFolderAbsolutePath())
                        || !File.Exists(ApplicationData.Instance.GetConfigurationFileAbsolutePath())) {
                        ConfigurationManager.CreateConfigurationDirectory();
                        ConfigurationManager.CreateEmptyConfiguration();
                    }

                    ConfigurationManager.Load();
                } catch(ConfigurationInvalidException) {
                    var result = MessageBox.Show(LocalizeHelper.GetLocalizedString("PreparationWindow_Ask_RecreateConfigurationFile"), ApplicationData.Instance.AppVersion, MessageBoxButton.YesNo);

                    if(result == MessageBoxResult.Yes) {
                        ConfigurationManager.CreateEmptyConfiguration();
                        ConfigurationManager.Load();
                    }
                }

                if(ConfigurationManager.IsConfigurationReady()) {
                    LocalizeHelper.SetCurrentUICulture(new System.Globalization.CultureInfo(ConfigurationManager.ConfigurationInstance.UIConfigurations.Language));
                }
            }

            public bool Condition() => true;
        }

        class UpdateAccountInformation : IPreparationWork {
            public bool IsCritical => false;

            public string StatusText => LocalizeHelper.GetLocalizedString("PreparationWindow_Status_UpdateAccountInfo");

            public string FailText => LocalizeHelper.GetLocalizedString("PreparationWindow_Status_UpdateAccountInfo_Failed");

            public string ConfigurationName => null;

            public void Work() {
                if(ConfigurationManager.IsConfigurationReady()
                    && ConfigurationManager.ConfigurationInstance.AccountInformations.Count > 0) {
                    for(int idx = 0, count = ConfigurationManager.ConfigurationInstance.AccountInformations.Count; idx < count; idx++) {
                        var existingAccountInfo = ConfigurationManager.ConfigurationInstance.AccountInformations[idx];
                        var updatedUser = TweetinviHelper.GetAuthedUserById(existingAccountInfo.AccountID);
                        ConfigurationManager.ConfigurationInstance.UpdateAccountInformation(existingAccountInfo.AccountID, updatedUser.Name, updatedUser.ScreenName, updatedUser.ProfileImageUrl400x400);
                    }

                    ConfigurationManager.Save();
                }
            }

            public bool Condition() => ConfigurationManager.IsConfigurationReady() && (ConfigurationManager.ConfigurationInstance.AccountCount() > 0);
        }
#endregion
    }

    interface IPreparationWork {
        bool IsCritical { get; }
        string StatusText { get; }
        string FailText { get; }
        string ConfigurationName { get; }

        void Work();
        bool Condition();
    }
}
