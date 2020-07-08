using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace MimeType.VirtualFile
{
    internal class VirtualFileInfo
    {
        private readonly VirtualFileReader _virtualFileReader;
        private readonly Dictionary<string, MimeStore> _mimeStore;

        public VirtualFileInfo(Assembly assembly)
        {
            _mimeStore = new Dictionary<string, MimeStore>(StringComparer.OrdinalIgnoreCase);
            _virtualFileReader = new VirtualFileReader(assembly);
        }

        public VirtualFileInfo Get(params string[] extension)
        {
            string[] fileNames;
            if (extension.Length > 0)
            {
                fileNames = _virtualFileReader
                    .GetFileNames()
                    .Where(x => extension.Any(y => x.EndsWith(y)))
                    .ToArray();
            }
            else
            {
                fileNames = _virtualFileReader.GetFileNames();
            }

            foreach (var fileName in fileNames)
            {
                _mimeStore[fileName] = null;
            }

            return this;
        }

        public VirtualFileInfo Read(string path, string extension)
        {
            for (int keyIndex = 0; keyIndex < _mimeStore.Count; keyIndex++)
            {
                string fileName = _mimeStore.ElementAt(keyIndex).Key;
                if (Regex.IsMatch(fileName, $@"^*.{path}.*-*.({extension})") ||
                    Regex.IsMatch(fileName, $@"^*.{path}.*.({extension})"))
                {
                    _mimeStore[fileName] = Deserialize<MimeStore>(_virtualFileReader.Read(fileName));
                }
            }

            return this;
        }

        public static T Deserialize<T>(Stream s)
        {
            using (StreamReader reader = new StreamReader(s))
            using (JsonTextReader jsonReader = new JsonTextReader(reader))
            {
                JsonSerializer ser = new JsonSerializer();
                return ser.Deserialize<T>(jsonReader);
            }
        }

        public string GetValue(string name)
        {
            string value = string.Empty;
            foreach (var mimeStore in _mimeStore)
            {
                _mimeStore[mimeStore.Key]?.TryGetValue(name, out value);
                if (!string.IsNullOrEmpty(value))
                    break;
            }

            return value;
        }
    }
}
