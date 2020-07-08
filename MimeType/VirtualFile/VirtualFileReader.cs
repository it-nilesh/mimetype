using System.IO;
using System.Reflection;

namespace MimeType.VirtualFile
{
    internal class VirtualFileReader
    {
        private readonly Assembly _assembly;
        
        public VirtualFileReader(Assembly assembly)
        {   
            _assembly = assembly;
        }

        public string[] GetFileNames()
        {
            return _assembly.GetManifestResourceNames();
        }

        public Stream Read(string fileName)
        {
            return _assembly.GetManifestResourceStream(fileName);
        }
    }
}
