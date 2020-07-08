using System;
using System.Linq;

namespace MimeType
{
    public class MimeTypeResourceNameAttribute : Attribute
    {
        public string Name { get; }

        public MimeTypeResourceNameAttribute(string name)
        {
            Name = name;
        }
        
        public static MimeTypeResourceNameAttribute GetOrNull(Type resourceType)
        {
            return resourceType
                .GetCustomAttributes(true)
                .OfType<MimeTypeResourceNameAttribute>()
                .FirstOrDefault();
        }

        public static string GetName(Type resourceType)
        {
            return GetOrNull(resourceType)?.Name ?? resourceType.FullName;
        }
    }
}
