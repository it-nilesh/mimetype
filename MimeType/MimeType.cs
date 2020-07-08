using MimeType.Resources;
using MimeType.VirtualFile;
using System;
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
        private readonly static MimeTypeStore _storeMimeType = new MimeTypeStore();

        public string this[string fileNameOrExtension]
        {
            get
            {
                return Get(fileNameOrExtension);
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

        public static string Get(string fileNameOrExtension)
        {
            if (_storeMimeType.Count == 0)
                AddMimeResources();

            return _storeMimeType.GetValue(GetExtension(fileNameOrExtension));
        }
        
        public static string GetExtension(string fileNameOrExtension)
        {
            var dotIndex = fileNameOrExtension.LastIndexOf(".");
            
            if(dotIndex >= 0)
            {
                fileNameOrExtension = fileNameOrExtension.Substring(dotIndex);
                fileNameOrExtension = fileNameOrExtension.StartsWith(".") ? fileNameOrExtension.Replace(".", "") : fileNameOrExtension;
            }
            
            return fileNameOrExtension;
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
