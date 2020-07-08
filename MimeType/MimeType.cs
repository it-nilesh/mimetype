using MimeType.Resources;
using MimeType.VirtualFile;
using System;
using System.Linq;

namespace MimeType
{
    public interface IMime
    {
        string this[string extension] { get; }
    }

    public class Mime : IMime
    {
        private readonly static MimeTypeStore _storeMimeType = new MimeTypeStore();

        public string this[string extension]
        {
            get
            {
                return Get(extension);
            }
        }

        public static void AddMimeResources(params Type[] resourceNames)
        {
            var newResourceNames = resourceNames.ToList();
            newResourceNames.Add(typeof(AllMimeType));

            foreach (var mimeType in newResourceNames)
            {
                var folderOrFileName = MimeTypeResourceNameAttribute.GetName(mimeType);
                _storeMimeType.Add(VirtualFileFinder.Find(mimeType, folderOrFileName));
            }
        }

        public static string Get(string extension)
        {
            if (_storeMimeType.Count == 0)
                AddMimeResources();

            var newExtension = extension.StartsWith(".") ? extension.Replace(".", "") : extension;
            return _storeMimeType.GetValue(newExtension);
        }
    }

    public static class MimeExtensions
    {
        public static string Get(this IMime mime, string extension)
        {
            return mime[extension];
        }
    }
}
