using System;
using System.Collections.Generic;

namespace EFCacheTest.Cache
{
    [Serializable]
    class DictionaryCache : ICache
    {
        private readonly Dictionary<string,object>  _cache = new Dictionary<string, object>();

        public bool GetItem(string key, out object item)
        {
            item = null;
            if (_cache.ContainsKey(key))
            {
                item = _cache[key];
                return true;
            }
            return false;
        }

        public void AddItem(string key, object item)
        {
            _cache.Add(key, item);
        }

        public void InvalidateItem(string key)
        {
            _cache.Remove(key);
        }
    }
}
