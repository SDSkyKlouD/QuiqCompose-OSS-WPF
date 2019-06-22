using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;

namespace SDSK.QuiqCompose.WinDesktop.Classes.Helpers {
    public static class JsonHelper {
        public static ICollection<T> DeserializeEmbeddedFileToList<T>(string path) {
            string assemblyPath = typeof(App).Namespace;
            string askedPath = $"{assemblyPath}{(!(path.StartsWith("/") || path.StartsWith("\\")) ? "." : "")}{path.Replace('/', '.').Replace('\\', '.').Replace(' ', '_')}";
            string rawJson;

            using(Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(askedPath)) {
                if(stream == null) return null;

                using(StreamReader reader = new StreamReader(stream)) {
                    rawJson = reader.ReadToEnd();
                }
            }

            if(rawJson.Equals(string.Empty) || rawJson == null) {
                return null;
            } else {
                return JsonConvert.DeserializeObject<ICollection<T>>(rawJson);
            }
        }
    }
}
