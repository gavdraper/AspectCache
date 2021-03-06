﻿using System;
using System.Collections.Generic;
using AspectCache.Cache;

namespace AspectCache.CacheProvider.Dictionary
{
    [Serializable]
    class DictionaryCache : ICache
    {
        private static readonly Dictionary<string, object> _cache = new Dictionary<string, object>();

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
            lock (_cache)
            {
                if (!_cache.ContainsKey(key))
                    _cache.Add(key, item);
            }
        }

        public void InvalidateItem(string key)
        {
            lock (_cache)
            {
                if (_cache.ContainsKey(key))
                    _cache.Remove(key);
            }
        }
    }
}
