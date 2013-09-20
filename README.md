AspectCache
===  
Aspect cache allows you to add caching to existing repositories and services without having to plumb in boiletplate code to every method you want to add it to. It does this using Aspect Oriented Programming with [PostSharp](http://www.postsharp.net/).  
  
For example the followng Get method in a repository can have caching added to it just by adding the following attribute

    [AspectCache(keyPrefix: "Product", identifier: "id", function: CacheFunction.RetrieveOrAdd, cache: typeof(RedisCache))]
    public Product Get(int id)
    {
        return _ctx.Products.FirstOrDefault(x => x.Id == id);
    }    
You would need to create the RedisCache object and implement the ICache interface provided with AspectCache but that is all. The AspectCache solution provides non production ready examples of both a RedisCache and Dictionary implementation.
  
### Requirements
* PostSharp Visual Studio Extension Installed
* PostSharp (Free) Express License Installed

### Getting Started

A good first reference is the sample project included within the solution. It comes with basic examples of using Redis caching and a .Net dictionary cache. These can be swapped out by changing the caching attributes in the repository class to point to the cache you want to use.

    [AspectCache(keyPrefix: "Product", identifier: "id", function: CacheFunction.RetrieveOrAdd, cache: typeof(RedisCache))]

or

    [AspectCache(keyPrefix: "Product", identifier: "id", function: CacheFunction.RetrieveOrAdd, cache: typeof(DictionaryCache))]
####File new project
1. Reference the AspectCache assembly from your project.
2. Create a cache class that inherits from AspectCache.Cache.ICache, alternativly start from one of the examples in the sample project DictionaryCache or RedisCache.
3. Add the caching attributes to the methods you want to cache. 
4. Done

### Cache Functions
AspectCache currently has 3 different ways to use the cache

1. **Retrieve** - If the item is in the cache it will be returned before the method with the attribute runs and execution of that method will be cancelled. 
2. **RetrieveOrAdd** - The same as retrieve but if the item does not exist in cache then when the method with the attribute returns the item it will be added to the cache.
3. **Invalidate** - Before and After the method runs the item will be removed from the cache. This means the method starts with a clean cache and finishes with one. I may in future add a switch so you can choose if the cache is cleared before after or both.
