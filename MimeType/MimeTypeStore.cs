using MimeType.VirtualFile;
using System.Collections.Generic;

namespace MimeType
{
    internal class MimeTypeStore : List<VirtualFileInfo>
    {
        public string GetValue(string name)
        {
            string value = string.Empty;
            foreach (VirtualFileInfo virtualFileInfo in this)
            {
                value = virtualFileInfo.GetValue(name);
                if (!string.IsNullOrEmpty(value))
                    break;
            }

            return value;
        }
    }
}
