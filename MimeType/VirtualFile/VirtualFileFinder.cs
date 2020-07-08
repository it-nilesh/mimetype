using System;

namespace MimeType.VirtualFile
{
    internal class VirtualFileFinder
    {
        public static VirtualFileInfo Find(Type resourceType, string filePath)
        {
            VirtualFileInfo virtualFileInfo = new VirtualFileInfo(resourceType.Assembly);
            virtualFileInfo.Get(".json").Read(filePath, "json");
            return virtualFileInfo;
        }
    }
}
