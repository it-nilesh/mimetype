using MimeType.VirtualFile;
using System;
using System.Collections.Generic;

namespace MimeType
{
    internal class MimeTypeStore
    {
        private readonly Dictionary<Type, VirtualFileInfo> _resources;
        private readonly MimeStore _mimeTypes;

        public MimeTypeStore()
        {
            _resources = new Dictionary<Type, VirtualFileInfo>();
            _mimeTypes = new MimeStore();
        }

        public int Count
        {
            get { return _mimeTypes.Count; }
        }

        public void Add(Type resourceType, VirtualFileInfo virtualFileInfo, bool replaceExisting)
        {
            if (resourceType == null || virtualFileInfo == null)
                return;

            if (_resources.ContainsKey(resourceType))
                return;

            _resources[resourceType] = virtualFileInfo;

            foreach (var mimeType in virtualFileInfo.GetValues())
            {
                if (replaceExisting || !_mimeTypes.ContainsKey(mimeType.Key))
                    _mimeTypes[mimeType.Key] = mimeType.Value;
            }
        }

        public string GetValue(string name)
        {
            string value;
            return TryGetValue(name, out value) ? value : string.Empty;
        }

        public bool TryGetValue(string name, out string value)
        {
            value = string.Empty;

            if (string.IsNullOrWhiteSpace(name))
                return false;

            return _mimeTypes.TryGetValue(name, out value);
        }

        public IReadOnlyDictionary<string, string> GetSnapshot()
        {
            return new Dictionary<string, string>(_mimeTypes, _mimeTypes.Comparer);
        }
    }
}
