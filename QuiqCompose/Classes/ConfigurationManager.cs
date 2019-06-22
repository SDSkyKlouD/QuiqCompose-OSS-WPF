using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Security;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using SDSK.QuiqCompose.WinDesktop.Classes.Helpers.Security;

namespace SDSK.QuiqCompose.WinDesktop.Classes {
    /// <summary>
    /// Application configuration management class
    /// 프로그램 설정 관리 클래스
    /// </summary>
    internal static class ConfigurationManager {
        internal static ConfigurationsRoot ConfigurationInstance { get; private set; } = null;
        internal static readonly XmlSerializer Serializer = new XmlSerializer(typeof(ConfigurationsRoot));
        internal static bool IsConfigurationLoaded { get; private set; } = false;

        /// <summary>
        /// Load the configurations from pre-defined configuration file path.
        /// After load, the configurations can be accessed via ConfigurationManager.ConfigurationInstance.
        /// 미리 지정된 설정 파일 경로에서 설정 파일을 불러들입니다.
        /// 설정이 불러들여지면, ConfigurationManager.ConfigurationInstance를 통해 설정에 접근할 수 있습니다.
        /// </summary>
        /// <returns>
        /// `true` if the configuration loaded successfully.
        /// 설정 파일이 성공적으로 불러들여진 경우 `true`를 반환합니다.
        /// </returns>
        /// <exception cref="ConfigurationInvalidException" />
        /// <exception cref="IOException" />
        /// <exception cref="UnauthorizedAccessException" />
        /// <exception cref="ArgumentException" />
        /// <exception cref="ArgumentNullException" />
        /// <exception cref="PathTooLongException" />
        /// <exception cref="DirectoryNotFoundException" />
        /// <exception cref="NotSupportedException" />
        /// <exception cref="FileNotFoundException" />
        /// <exception cref="SecurityException" />
        /// <exception cref="UnauthorizedAccessException" />
        /// <exception cref="ArgumentOutOfRangeException" />
        internal static bool Load() {
            IsConfigurationLoaded = false;

            /* Step 1. Create configuration directory tree if not exists */
            CreateConfigurationDirectory();

            /* Step 2. Throw an exception when invalid configuration file found */
            if(Directory.Exists(ApplicationData.Instance.GetConfigurationFileRootFolderAbsolutePath())) {
                if(File.Exists(ApplicationData.Instance.GetConfigurationFileAbsolutePath())) {
                    using(FileStream confFileStream = new FileStream(ApplicationData.Instance.GetConfigurationFileAbsolutePath(), FileMode.Open, FileAccess.Read, FileShare.None)) {
                        if(((new FileInfo(ApplicationData.Instance.GetConfigurationFileAbsolutePath())).Length < 500)
                            || (!Serializer.CanDeserialize(new XmlTextReader(confFileStream)))) {
                            throw new ConfigurationInvalidException("The configuration file isn't deserializable.");
                        }
                    }
                }
            }

            /* Step 3. Deserialize configuration file */
            if(Directory.Exists(ApplicationData.Instance.GetConfigurationFileRootFolderAbsolutePath()) && File.Exists(ApplicationData.Instance.GetConfigurationFileAbsolutePath())) {
                using(FileStream confFileStream = new FileStream(ApplicationData.Instance.GetConfigurationFileAbsolutePath(), FileMode.Open, FileAccess.Read, FileShare.None)) {
                    using(XmlTextReader reader = new XmlTextReader(confFileStream)) {
                        if(Serializer.CanDeserialize(reader)) {
                            try {
                                ConfigurationInstance = (ConfigurationsRoot) Serializer.Deserialize(reader);
                            } catch(InvalidOperationException e) {
                                throw new ConfigurationInvalidException("An exception thrown during deserializing configurations.", e);
                            }
                        } else {
                            throw new ConfigurationInvalidException("The configuration file can't be deserialized.");
                        }
                    }
                }
            }

            /* Step 4. Check deserialized configuration instance */
            if(ConfigurationInstance != null
                && !string.IsNullOrWhiteSpace(ConfigurationInstance.AppVersion)
                && new Regex(@"(?:\d+\.\d+\.\d+\.\d+)|(?:\d{8})").IsMatch(ConfigurationInstance.AppVersion)
                && ConfigurationInstance.AccountInformations != null
                && ConfigurationInstance.UIConfigurations != null) {
                IsConfigurationLoaded = true;
                return true;
            } else {
                throw new ConfigurationInvalidException("Checking for deserialized configuration was failed.");
            }
        }

        /// <summary>
        ///     Load the configuration from pre-defined configuration file path, in asynchronous way.
        ///     미리 지정된 설정 파일 경로에서 설정 파일을 비동기적으로 불러들입니다.
        /// </summary>
        /// <returns>
        ///     `true` if the configuration loaded successfully.
        ///     설정 파일이 성공적으로 불러들여진 경우 `true`를 반환합니다.
        /// </returns>
        /// <exception cref="ConfigurationInvalidException" />
        /// <exception cref="IOException" />
        /// <exception cref="UnauthorizedAccessException" />
        /// <exception cref="ArgumentException" />
        /// <exception cref="ArgumentNullException" />
        /// <exception cref="ArgumentOutOfRangeException" />
        /// <exception cref="PathTooLongException" />
        /// <exception cref="DirectoryNotFoundException" />
        /// <exception cref="NotSupportedException" />
        /// <exception cref="FileNotFoundException" />
        /// <exception cref="SecurityException" />
        /// <exception cref="UnauthorizedAccessException" />
        internal static Task<bool> LoadAsync()
            => Task.Run(() => Load());

        /// <summary>
        ///     Save the configurations to pre-defined configuration file path.
        ///     미리 지정된 설정 파일 경로로 설정을 저장합니다.
        /// </summary>
        /// <exception cref="ArgumentException" />
        /// <exception cref="ArgumentNullException" />
        /// <exception cref="ArgumentOutOfRangeException" />
        /// <exception cref="NotSupportedException" />
        /// <exception cref="FileNotFoundException" />
        /// <exception cref="IOException" />
        /// <exception cref="SecurityException" />
        /// <exception cref="DirectoryNotFoundException" />
        /// <exception cref="UnauthorizedAccessException" />
        /// <exception cref="PathTooLongException" />
        /// <exception cref="InvalidOperationException" />
        internal static void Save() {
            using(FileStream confFileStream = new FileStream(ApplicationData.Instance.GetConfigurationFileAbsolutePath(), FileMode.Create, FileAccess.Write, FileShare.None)) {
                if(IsConfigurationReady()) {
                    ConfigurationInstance.AppVersion = ApplicationData.Instance.AppVersion;
                }

                using(XmlTextWriter writer = new XmlTextWriter(confFileStream, System.Text.Encoding.Unicode)) {
                    writer.Formatting = Formatting.Indented;

                    Serializer.Serialize(writer, ConfigurationInstance);
                }
            }
        }

        /// <summary>
        ///     Save the configurations to pre-defined configuration file path, in asynchronous way.
        ///     미리 지정된 설정 파일 경로로 설정을 비동기적으로 저장합니다.
        /// </summary>
        /// <exception cref="ArgumentException" />
        /// <exception cref="ArgumentNullException" />
        /// <exception cref="ArgumentOutOfRangeException" />
        /// <exception cref="NotSupportedException" />
        /// <exception cref="FileNotFoundException" />
        /// <exception cref="IOException" />
        /// <exception cref="SecurityException" />
        /// <exception cref="DirectoryNotFoundException" />
        /// <exception cref="UnauthorizedAccessException" />
        /// <exception cref="PathTooLongException" />
        /// <exception cref="InvalidOperationException" />
        internal static Task SaveAsync()
            => Task.Run(() => Save());

        internal static void CreateEmptyConfiguration() {
            ConfigurationInstance = new ConfigurationsRoot();
            Save();
        }

        internal static bool IsConfigurationReady()
            => ConfigurationInstance != null &&
                ConfigurationInstance.AccountInformations != null &&
                ConfigurationInstance.UIConfigurations != null &&
                IsConfigurationLoaded;

        internal static bool CreateConfigurationDirectory() {
            Directory.CreateDirectory(ApplicationData.Instance.GetConfigurationFileRootFolderAbsolutePath());

            if(!Directory.Exists(ApplicationData.Instance.GetConfigurationFileRootFolderAbsolutePath()) && !((new DirectoryInfo(ApplicationData.Instance.GetConfigurationFileRootFolderAbsolutePath())).Attributes.HasFlag(FileAttributes.ReadOnly))) {
                return true;
            }

            return false;
        }
    }

    #region Configuration classes
    [XmlRoot(ElementName = "Configurations", IsNullable = false)]
    public class ConfigurationsRoot {
        public string AppVersion { get; set; } = ApplicationData.Instance.AppVersion;
        [XmlArray(), XmlArrayItem(Type = typeof(CTwitterAccountInformation), ElementName = "TwitterAccountInfo")]
        public List<CTwitterAccountInformation> AccountInformations { get; set; } = new List<CTwitterAccountInformation>();
        public CUserInterfaceSettings UIConfigurations { get; set; } = new CUserInterfaceSettings();

        #region Functions
        public CTwitterAccountInformation FindAccountById(long accountId) {
            CTwitterAccountInformation item = null;

            AccountInformations.ForEach(i => {
                if(i.AccountID == accountId) {
                    item = i;
                }
            });

            return item;
        }

        public int AccountCount()
            => AccountInformations.Count;

        public void UpdateAccountInformation(long accountId, string nickname = null,
            string screenname = null, string profileImageUrl = null) {
            if(FindAccountById(accountId) is CTwitterAccountInformation account) {
                if(!account.Nickname.Equals(nickname) || !account.ScreenName.Equals(screenname) || !account.ProfileImageUrl.Equals(profileImageUrl)) {
                    var tempDecryptedAccessTokens = DecryptAccessTokens(account);
                    var refreshedAccount = new CTwitterAccountInformation() {
                        AccountID = accountId,
                        Nickname = nickname,
                        ScreenName = screenname,
                        ProfileImageUrl = profileImageUrl,
                        TokenInfo = new CTwitterAccountTokenInformation() {
                            AccessToken = tempDecryptedAccessTokens.Item1.ReadAsString(),
                            AccessTokenSecret = tempDecryptedAccessTokens.Item2.ReadAsString()
                        }
                    };
                    EncryptAccessTokens(refreshedAccount);
                    ReplaceAccount(accountId, refreshedAccount);
                } else {
                    account.Nickname = nickname;
                    account.ScreenName = screenname;
                    account.ProfileImageUrl = profileImageUrl;
                }
            }
        }

        public bool ReplaceAccount(long accountId, CTwitterAccountInformation accountInformation) {
            int currentIndex = GetAccountIndex(accountId);

            if(currentIndex != -1) {
                RemoveAccount(accountId);
                AddAccount(currentIndex, accountInformation);

                if(AccountInformations[currentIndex] == accountInformation) {
                    return true;
                }
            }

            return false;
        }

        public int GetAccountIndex(long accountId) {
            int index = -1;

            AccountInformations.ForEach(i => {
                if(i.AccountID == accountId) {
                    index = AccountInformations.IndexOf(i);
                }
            });

            return index;
        }
        public int GetAccountIndex(CTwitterAccountInformation accountInformation)
            => AccountInformations.IndexOf(accountInformation);

        public void AddAccount(CTwitterAccountInformation accountInformation)
            => AddAccount(AccountInformations.Count, accountInformation);
        public void AddAccount(int index, CTwitterAccountInformation accountInformation) {
            if(!accountInformation.TokenInfo.Secured) {
                EncryptAccessTokens(accountInformation);
            }

            AccountInformations.Insert(index, accountInformation);
        }

        public void RemoveAccount(long accountId)
            => AccountInformations.Remove(FindAccountById(accountId));
        public void RemoveAccount(CTwitterAccountInformation accountInformation)
            => AccountInformations.Remove(accountInformation);

        internal Tuple<SecureString, SecureString> DecryptAccessTokens(long accountId)
            => DecryptAccessTokens(FindAccountById(accountId));
        internal Tuple<SecureString, SecureString> DecryptAccessTokens(CTwitterAccountInformation accountInformation) {
            if(accountInformation.TokenInfo.Secured) {
                return new Tuple<SecureString, SecureString>(
                    accountInformation.TokenInfo.AccessToken.Decrypt(
                        accountInformation.GetPW(accountInformation.Nickname.Substring(
                            accountInformation.Nickname.Length - 1 - (accountInformation.Nickname.Length < 4 ? 0 : 1), 1)).ReadAsString(),
                        accountInformation.GetSALT(nameof(accountInformation.TokenInfo.AccessToken)).ReadAsString(),
                        accountInformation.GetIV(@"CoLA").ReadAsString()),
                    accountInformation.TokenInfo.AccessTokenSecret.Decrypt(
                        accountInformation.GetPW(accountInformation.Nickname.Substring(
                            accountInformation.Nickname.Length - 1 - (accountInformation.Nickname.Length < 4 ? 0 : 3), 1)).ReadAsString(),
                        accountInformation.GetSALT(nameof(accountInformation.TokenInfo.AccessTokenSecret)).ReadAsString(),
                        accountInformation.GetIV(@"CH!K").ReadAsString()));
            } else {
                return new Tuple<SecureString, SecureString>(
                    accountInformation.TokenInfo.AccessToken.ToSecureString(),
                    accountInformation.TokenInfo.AccessTokenSecret.ToSecureString());
            }
        }

        internal void EncryptAccessTokens(long accountId)
            => EncryptAccessTokens(FindAccountById(accountId));
        internal void EncryptAccessTokens(CTwitterAccountInformation accountInformation) {
            if(!accountInformation.TokenInfo.Secured) {
                accountInformation.TokenInfo.AccessToken
                    = accountInformation.TokenInfo.AccessToken.Encrypt(
                        accountInformation.GetPW(accountInformation.Nickname.Substring(
                            accountInformation.Nickname.Length - 1 - (accountInformation.Nickname.Length < 4 ? 0 : 1), 1)).ReadAsString(),
                        accountInformation.GetSALT(nameof(accountInformation.TokenInfo.AccessToken)).ReadAsString(),
                        accountInformation.GetIV(@"CoLA").ReadAsString()).ReadAsString();
                accountInformation.TokenInfo.AccessTokenSecret
                    = accountInformation.TokenInfo.AccessTokenSecret.Encrypt(
                        accountInformation.GetPW(accountInformation.Nickname.Substring(
                            accountInformation.Nickname.Length - 1 - (accountInformation.Nickname.Length < 4 ? 0 : 3), 1)).ReadAsString(),
                        accountInformation.GetSALT(nameof(accountInformation.TokenInfo.AccessTokenSecret)).ReadAsString(),
                        accountInformation.GetIV(@"CH!K").ReadAsString()).ReadAsString();

                accountInformation.TokenInfo.Secured = true;
            }
        }
        #endregion
    }

    [XmlRoot(IsNullable = false)]
    public class CTwitterAccountInformation {
        [XmlAttribute()]
        public long AccountID { get; set; }
        public string Nickname { get; set; }
        public string ScreenName { get; set; }
        public string ProfileImageUrl { get; set; }
        public CTwitterAccountTokenInformation TokenInfo { get; set; }

        #region Security functions
        internal SecureString GetPW(string additional = "")
            => $"ACC_MIKU_TOKPW!{ScreenName}SDSK{additional}QC{AccountID}".ToSecureString();
        internal SecureString GetSALT(string additional = "")
            => $"QuIqCoMpOsE!@!{new string("CoNfIgUrAtIoN".ToCharArray().Select(c => (char) (c << 1)).ToArray())}*&*SaLT^#^{additional}".ToSecureString();
        internal SecureString GetIV(string specified4B = "QC39")
            => ($"{ScreenName.Substring(0, 2)}{ScreenName.Substring(ScreenName.Length - 1 - 2, 2)}"
                + $"{AccountID.ToString().Substring(0, 3)}{AccountID.ToString().Substring(AccountID.ToString().Length - 1 - 3, 3)}"
                + $"{new string(specified4B.ToCharArray().Select(c => (char) (c >> 1)).ToArray())}"
                + @"Q#C_3*9"
                + $"{ProfileImageUrl.Substring(ProfileImageUrl.Length - 1 - 5, 5)}"
                + @"P!z2a@").ToSecureString();
        #endregion
    }

    [XmlRoot(IsNullable = false)]
    public class CTwitterAccountTokenInformation {
        public string AccessToken { get; set; }
        public string AccessTokenSecret { get; set; }
        public bool Secured { get; set; } = false;
    }

    [XmlRoot(IsNullable = false)]
    public class CUserInterfaceSettings {
        public string Language { get; set; } = CultureInfo.InstalledUICulture.Name;
        public float ComposeWindowOpacity { get; set; } = 1;
        public bool TopMost { get; set; } = false;
        public bool OpenProfilePageOnAccountClick { get; set; } = false;
        public bool ShowProfileImage { get; set; } = true;
        public bool SaveLastAccountPosition { get; set; } = true;
        public ushort ComposeTextFontSize { get; set; } = 15;
        public int LastAccountPosition { get; set; } = -1;

        [XmlIgnore]
        public object this[string propertyName] {
            get => GetType().GetProperty(propertyName).GetValue(this, null);
            set => GetType().GetProperty(propertyName).SetValue(this, value, null);
        }
    }
    #endregion

    public sealed class ConfigurationInvalidException : Exception {
        public ConfigurationInvalidException() { }
        public ConfigurationInvalidException(string message) : base(message) { }
        public ConfigurationInvalidException(string message, Exception inner) : base(message, inner) { }
        public ConfigurationInvalidException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
