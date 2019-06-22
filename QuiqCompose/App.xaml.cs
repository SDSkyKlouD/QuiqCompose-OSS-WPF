using System.Threading;
using System.Windows;
using SDSK.QuiqCompose.WinDesktop.Classes;
using SDSK.QuiqCompose.WinDesktop.Classes.Helpers;
using SDSK.QuiqCompose.WinDesktop.Classes.Helpers.Security;

namespace SDSK.QuiqCompose.WinDesktop {
    public partial class App : Application {
        public App() {
            Tweetinvi.TweetinviConfig.ApplicationSettings.TweetMode = Tweetinvi.TweetMode.Extended;
            Tweetinvi.TweetinviConfig.CurrentThreadSettings.InitialiseFrom(Tweetinvi.TweetinviConfig.ApplicationSettings);

            LocalizeHelper.SetCurrentUICulture(Thread.CurrentThread.CurrentUICulture);

            /* You MUST set instance of ApplicationData to make the application work.
             * 
             * Example :
             *      ApplicationData.SetInstance(new ApplicationData() {
             *          AppVersion = "yyyyMMdd",
             *          AppDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"QuiqCompose"),
             *          ConfigurationFileRelativePath = null,
             *          ConfigurationFileName = @"Settings.xml",
             *          ImageCacheFolderRelativePath = @"Caches/Images",
             *          CryptoSalt = @"QC_SALT".ToSecureString(),
             *          TwitterApp = new TwitterAppInfo() {
             *              ConsumerKey = "~~~~".ToSecureString(),
             *              ConsumerSecretKey = "~~~~".ToSecureString()
             *          }
             *      });
             * 
             * NOTE that at least `TwitterApp` property MUST be set (another properties has preset but `TwiterApp` doesn't)
             */
        }
    }
}
