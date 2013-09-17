namespace AspectCache.Cache
{
    public interface ICache
    {
        bool GetItem(string key, out object item);
        void AddItem(string key, object item);
        void InvalidateItem(string key);
    }
}
