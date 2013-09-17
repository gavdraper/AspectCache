using System;

namespace AspectCache.Cache
{
    public class RedisCache : ICache
    {

        public bool GetItem(string key, out object item)
        {
            throw new NotImplementedException();
        }

        public void AddItem(string key, object item)
        {
            throw new NotImplementedException();
        }

        public void InvalidateItem(string key)
        {
            throw new NotImplementedException();
        }
    }
}
