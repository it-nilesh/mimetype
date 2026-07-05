using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;

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
            foreach (var fileName in _mimeStore.Keys.ToArray())
            {
                if (IsMatch(fileName, path, extension))
                {
                    _mimeStore[fileName] = DeserializeMimeStore(_virtualFileReader.Read(fileName));
                }
            }

            return this;
        }

        public IEnumerable<KeyValuePair<string, string>> GetValues()
        {
            foreach (var mimeStore in _mimeStore.Values)
            {
                if (mimeStore == null)
                    continue;

                foreach (var mimeType in mimeStore)
                    yield return mimeType;
            }
        }

        public static MimeStore DeserializeMimeStore(Stream s)
        {
            using (s)
            {
                var values = JsonSerializer.Deserialize<Dictionary<string, string>>(s);
                var mimeStore = new MimeStore();

                if (values == null)
                    return mimeStore;

                foreach (var value in values)
                    mimeStore[value.Key] = value.Value;

                return mimeStore;
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

        private static bool IsMatch(string fileName, string path, string extension)
        {
            return fileName.IndexOf(path, StringComparison.OrdinalIgnoreCase) >= 0 &&
                fileName.EndsWith("." + extension, StringComparison.OrdinalIgnoreCase);
        }
    }
}
