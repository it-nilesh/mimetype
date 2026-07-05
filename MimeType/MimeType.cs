using MimeType.Resources;
using MimeType.VirtualFile;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MimeType
{
    public interface IMime
    {
        string this[string extension] { get; }
    }

    public class Mime : IMime
    {
        private static readonly MimeTypeStore _storeMimeType = new MimeTypeStore();
        private static readonly object _sync = new object();

        public string this[string fileNameOrExtension]
        {
            get
            {
                return Get(fileNameOrExtension);
            }
        }

        public static void AddMimeResources(params Type[] resourceNames)
        {
            foreach (var mimeType in GetResourceTypes(resourceNames))
            {
                var folderOrFileName = MimeTypeResourceNameAttribute.GetName(mimeType);
                var replaceExisting = mimeType != typeof(AllMimeType);

                lock (_sync)
                {
                    _storeMimeType.Add(mimeType, VirtualFileFinder.Find(mimeType, folderOrFileName), replaceExisting);
                }
            }
        }

        public static string Get(string fileNameOrExtension)
        {
            EnsureDefaultResources();

            lock (_sync)
            {
                return _storeMimeType.GetValue(GetExtension(fileNameOrExtension));
            }
        }

        public static string GetOrDefault(string fileNameOrExtension, string defaultMimeType = "application/octet-stream")
        {
            string mimeType;
            return TryGet(fileNameOrExtension, out mimeType) ? mimeType : defaultMimeType;
        }

        public static bool TryGet(string fileNameOrExtension, out string mimeType)
        {
            EnsureDefaultResources();

            lock (_sync)
            {
                return _storeMimeType.TryGetValue(GetExtension(fileNameOrExtension), out mimeType);
            }
        }

        public static bool Contains(string fileNameOrExtension)
        {
            string mimeType;
            return TryGet(fileNameOrExtension, out mimeType);
        }

        public static IReadOnlyDictionary<string, string> GetKnownTypes()
        {
            EnsureDefaultResources();

            lock (_sync)
            {
                return _storeMimeType.GetSnapshot();
            }
        }
        
        public static string GetExtension(string fileNameOrExtension)
        {
            if (string.IsNullOrWhiteSpace(fileNameOrExtension))
                return string.Empty;

            fileNameOrExtension = Path.GetFileName(fileNameOrExtension.Trim());
            var dotIndex = fileNameOrExtension.LastIndexOf(".");
            
            if(dotIndex >= 0)
            {
                fileNameOrExtension = fileNameOrExtension.Substring(dotIndex);
                fileNameOrExtension = fileNameOrExtension.StartsWith(".") ? fileNameOrExtension.TrimStart('.') : fileNameOrExtension;
            }
            
            return fileNameOrExtension;
        }

        private static void EnsureDefaultResources()
        {
            lock (_sync)
            {
                if (_storeMimeType.Count != 0)
                    return;
            }

            AddMimeResources();
        }

        private static IEnumerable<Type> GetResourceTypes(params Type[] resourceNames)
        {
            return (resourceNames ?? new Type[0])
                .Where(resourceName => resourceName != null)
                .Concat(new[] { typeof(AllMimeType) })
                .Distinct();
        }
    }

    public static class MimeExtensions
    {
        public static string Get(this IMime mime, string fileNameOrExtension)
        {
            return mime[fileNameOrExtension];
        }
    }
}
