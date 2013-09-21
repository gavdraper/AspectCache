using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using AspectCache.Cache;

namespace AspectCache.CacheProvider.Redis
{
    [Serializable]
    public class RedisCache : ICache
    {
        public bool GetItem(string key, out object item)
        {
            item = null;
            using (var c = new ServiceStack.Redis.RedisClient("localhost", 6379))
            {
                if (c.Exists(key) == 1)
                {
                    item = ByteArrayToObject(c.Get(key));
                    return true;
                }
                return false;
            }
        }

        public void AddItem(string key, object item)
        {
            using (var c = new ServiceStack.Redis.RedisClient("localhost", 6379))
            {
                if (c.Exists(key) == 0)
                {
                    c.Add(key, ObjectToByteArray(item));
                }
            }
        }

        public void InvalidateItem(string key)
        {
            using (var c = new ServiceStack.Redis.RedisClient("localhost", 6379))
            {
                if (c.Exists(key) == 1)
                {
                    c.RemoveEntry(key);
                }
            }
        }

        private byte[] ObjectToByteArray(Object obj)
        {
            if (obj == null)
                return null;
            var bf = new BinaryFormatter();
            var ms = new MemoryStream();
            bf.Serialize(ms, obj);
            return ms.ToArray();
        }

        public static object ByteArrayToObject(byte[] data)
        {
            object obj = null;
            Stream streamWrite = new MemoryStream(data);
            var binaryWrite = new BinaryFormatter();
            streamWrite.Seek(0, SeekOrigin.Begin);
            obj = binaryWrite.Deserialize(streamWrite);
            return obj;
        }
    }
}

