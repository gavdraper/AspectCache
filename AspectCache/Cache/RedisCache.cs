﻿using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace AspectCache.Cache
{
    [Serializable]
    public class RedisCache : ICache
    {

        public bool GetItem(string key, out object item)
        {
            item = null;
            var c = new ServiceStack.Redis.RedisClient("localhost", 6379);
            if (c.Exists(key)==1)
            {             
                item = ByteArrayToObject(c.Get(key));
                return true;
            }
            return false;
        }

        public void AddItem(string key, object item)
        {
            var c = new ServiceStack.Redis.RedisClient("localhost", 6379);
            c.Add(key, ObjectToByteArray(item));
        }

        public void InvalidateItem(string key)
        {
            var c = new ServiceStack.Redis.RedisClient("localhost", 6379);
            c.RemoveEntry(key);
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
            System.IO.Stream streamWrite = new System.IO.MemoryStream(data);
            var binaryWrite = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            streamWrite.Seek(0, System.IO.SeekOrigin.Begin);
            obj = binaryWrite.Deserialize(streamWrite);
            return obj;
        }

    }
}
