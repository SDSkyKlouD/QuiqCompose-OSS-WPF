using System.Collections.Generic;
using System.Windows.Input;
using Newtonsoft.Json;
using SDSK.QuiqCompose.WinDesktop.Classes.Helpers;

namespace SDSK.QuiqCompose.WinDesktop.Windows.MVVM.DataTypes {
    public sealed class AppInfoWindowOSLItem {
        [JsonProperty(PropertyName = "libraryName")]
        public string LibraryName { get; set; }

        [JsonProperty(PropertyName = "libraryDesc")]
        public string LibraryDesc { get; set; }

        [JsonProperty(PropertyName = "libraryUrl")]
        public string LibraryUrl { get; set; }

        [JsonProperty(PropertyName = "licenseType")]
        public string LicenseType { get; set; }

        private List<AppInfoWindowOSLItem> _dependencies;
        [JsonProperty(PropertyName = "dependencies", Required = Required.Default)]
        public List<AppInfoWindowOSLItem> Dependencies {
            get => _dependencies;
            set {
                _dependencies = value;
                
                if(value.Count <= 0 || value == null) {
                    HasDependencies = false;
                } else {
                    HasDependencies = true;
                }
            }
        }

        [JsonIgnore]
        public bool HasDependencies { get; private set; } = false;

        [JsonIgnore]
        public ICommand OpenHomepageCommand { get; } = new CommandHelper.ProcessStartCommand();
    }
}
