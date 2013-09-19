using System;
using System.Collections.Generic;
using System.Linq;
using PostSharp.Aspects;
using AspectCache.Cache;
namespace AspectCache
{
    [Serializable]
    class AspectCache : OnMethodBoundaryAspect
    {
        private readonly string _keyPrefix;
        private readonly string _identifier;
        private readonly CacheFunction _function;
        private bool _found;
        private readonly ICache _cache;

        public AspectCache(string keyPrefix, string identifier, CacheFunction function, Type cache)
        {
            _keyPrefix = keyPrefix;
            _identifier = identifier;
            _function = function;
            _cache = (ICache)Activator.CreateInstance(cache);
        }

        private Dictionary<string, object> getParams(MethodExecutionArgs args)
        {
            var methodParams = new Dictionary<string, object>();
            var ps = args.Method.GetParameters();
            for (var i = 0; i < ps.Count(); i++)
                methodParams.Add(ps[i].Name, args.Arguments[i]);
            return methodParams;
        }

        private string getIdValue(Dictionary<string, object> keys, string idField)
        {
            var idValue = "";
            var path = idField.Split('.');
            var root = keys[path[0]];
            if (path.Length == 1)
                idValue = root.ToString();
            else
            {
                for (var i = 1; i < (path.Length); i++)
                {
                    if (i == path.Length - 1)
                        idValue = root.GetType().GetProperty(path[i]).GetValue(root, null).ToString();
                    else
                        root = root.GetType().GetProperty(path[i]);
                }
            }
            return idValue;
        }

        public override void OnEntry(MethodExecutionArgs args)
        {
            var methodParams = getParams(args);
            var idValue = getIdValue(methodParams, _identifier);

            if (_function == CacheFunction.Retrieve || _function == CacheFunction.RetrieveOrAdd)
            {
                object item;

                if (_cache.GetItem(_keyPrefix + idValue, out item))
                {
                    _found = true;
                    args.FlowBehavior = FlowBehavior.Return;
                    args.ReturnValue = item;
                    Console.WriteLine("\tFound In Cache");
                }
                else
                {
                    _found = false;
                    Console.WriteLine("\tNot Found In Cache");
                    
                }

            }
            //This is repeated in exit incase any items in method have retrieved and re added the item
            //to the cache. This guarenttes that the start and end of the method will have a clean cache
            //for this item
            else if (_function == CacheFunction.Invalidate)
            {
                Console.WriteLine("\tRemoved From Cache");
                _cache.InvalidateItem(_keyPrefix + idValue);
            }

        }

        public override void OnExit(MethodExecutionArgs args)
        {
            var methodParams = getParams(args);
            var idValue = getIdValue(methodParams, _identifier);
            if (!_found && _function == CacheFunction.RetrieveOrAdd && args.ReturnValue != null)
            {
                _cache.AddItem(_keyPrefix + idValue, args.ReturnValue);
                Console.WriteLine("\tAdded to cache");
            }
            //This is repeated in exit incase any items in method have retrieved and re added the item
            //to the cache. This guarenttes that the start and end of the method will have a clean cache
            //for this item
            else if ( _function == CacheFunction.Invalidate)
            {
                Console.WriteLine("\tRemoved From Cache");
                _cache.InvalidateItem(_keyPrefix + idValue);
            }
        }
    }

    public enum CacheFunction
    {
        RetrieveOrAdd,
        Retrieve,
        Invalidate
    }

}
