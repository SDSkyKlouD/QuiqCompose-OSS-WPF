namespace SDSK.QuiqCompose.WinDesktop.Windows.MVVM.DataTypes {
    public sealed class ComposeWindowAccountComboBoxItem {
        public long AccountID { get; set; }
        public string ProfileImageUrl { get; set; }
        public string Nickname { get; set; }
        private string _screenName;
        public string ScreenName {
            get => _screenName;
            set {
                _screenName = value;
                ScreenNameWithAt = "@" + value;
            }
        }
        public string ScreenNameWithAt { get; private set; }
    }
}
