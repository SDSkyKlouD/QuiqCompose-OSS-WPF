using System;
using System.IO;
using System.Security;
using SDSK.QuiqCompose.WinDesktop.Classes.Helpers.Security;
using Tweetinvi.Models;

namespace SDSK.QuiqCompose.WinDesktop.Classes {
    /// <summary>
    /// A class stores various data to make the application work.
    /// Use `ApplicationData.Instance` when you have to access these data.
    /// </summary>
    internal class ApplicationData {
        /// <summary>
        /// A static instance of ApplicationData
        /// </summary>
        internal static ApplicationData Instance { get; private set; }

        /// <summary>
        /// Name of the application
        /// </summary>
        internal string AppName { get; } = "QuiqCompose";
        /// <summary>
        /// Version of the application
        /// </summary>
        internal string AppVersion { get; set; } = System.Diagnostics.FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).FileVersion;
        /// <summary>
        /// Absolute path where the files and folders related to the application goes
        /// </summary>
        internal string AppDataPath { get; set; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"QuiqCompose");
        /// <summary>
        /// Root path of the configuration file, relative to AppDataPath
        /// </summary>
        internal string ConfigurationFileRelativePath { get; set; } = null;
        /// <summary>
        /// File name of the configuration file
        /// </summary>
        internal string ConfigurationFileName { get; set; } = @"Settings.xml";
        /// <summary>
        /// Root path for storing cached images. relative to AppDataPath
        /// </summary>
        internal string ImageCacheFolderRelativePath { get; set; } = @"Caches\Images";
        /// <summary>
        /// Salt string used when encrypt/decrypt Twitter user auth tokens
        /// </summary>
        internal SecureString CryptoSalt { get; set; } = @"QC_SALT".ToSecureString();
        /// <summary>
        /// Twitter application information
        /// </summary>
        internal TwitterAppInfo TwitterApp { get; set; } = null;

        /// <summary>
        /// Set static instance of ApplicationData
        /// </summary>
        /// <param name="instance">An ApplicationData instance</param>
        /// <exception cref="ArgumentNullException"></exception>
        internal static void SetInstance(ApplicationData instance) {
            if(instance != null && instance is ApplicationData) {
                Instance = instance;
            } else {
                throw new ArgumentNullException(nameof(instance));
            }
        }
        /// <summary>
        /// Get absolute path of the folder where the configuration file saved
        /// </summary>
        /// <returns></returns>
        internal string GetConfigurationFileRootFolderAbsolutePath() => Path.Combine(AppDataPath, ConfigurationFileRelativePath ?? "");
        /// <summary>
        /// Get absolute path of the configuration file
        /// </summary>
        internal string GetConfigurationFileAbsolutePath() => Path.Combine(AppDataPath, ConfigurationFileRelativePath ?? "", ConfigurationFileName);
        /// <summary>
        /// Get absolute path of image cache folder
        /// </summary>
        internal string GetImageCacheFolderAbsolutePath() => Path.Combine(AppDataPath, ImageCacheFolderRelativePath ?? "");
    }

    /// <summary>
    /// A class stores Twitter application information
    /// </summary>
    internal class TwitterAppInfo {
        private SecureString _consumerKey = null;
        internal SecureString ConsumerKey {
            get {
                if(_consumerKey != null) {
                    return _consumerKey;
                } else {
                    throw new InvalidOperationException("ConsumerKey is not set but tried to access it");
                }
            }
            set => _consumerKey = value;
        }
        private SecureString _consumerSecretKey = null;
        internal SecureString ConsumerSecretKey {
            get {
                if(_consumerSecretKey != null) {
                    return _consumerSecretKey;
                } else {
                    throw new InvalidOperationException("ConsumerSecretKey is not set but tried to access it");
                }
            }
            set => _consumerSecretKey = value;
        }

        internal TwitterCredentials GetTwitterCredentials() => new TwitterCredentials(ConsumerKey.ReadAsString(), ConsumerSecretKey.ReadAsString());
    }
}
