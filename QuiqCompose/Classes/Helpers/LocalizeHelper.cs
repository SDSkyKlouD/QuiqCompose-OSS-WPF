using System.Globalization;
using System.Threading;
using WPFLocalizeExtension.Engine;

namespace SDSK.QuiqCompose.WinDesktop.Classes.Helpers {
    public sealed class LocalizeHelper {
        public static string GetLocalizedString(string key)
            => (string) LocalizeDictionary.Instance.GetLocalizedObject(key, null, LocalizeDictionary.CurrentCulture);

        public static void SetCurrentUICulture(CultureInfo cultureInfo) {
            LocalizeDictionary.Instance.Culture = cultureInfo;
            Thread.CurrentThread.CurrentUICulture = cultureInfo;
            CultureInfo.CurrentUICulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;
            LocalizeDictionary.Instance.SetCurrentThreadCulture = true;
        }

        public static CultureInfo GetDefaultCurrentUICulture()
            => CultureInfo.DefaultThreadCurrentUICulture;

        public static CultureInfo GetCurrentThreadUICulture()
            => Thread.CurrentThread.CurrentUICulture;
    }
}
