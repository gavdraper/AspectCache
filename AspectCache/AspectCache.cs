using System;
using System.Collections.Generic;
using System.Linq;
using PostSharp.Aspects;
using AspectCache.Cache;
namespace AspectCache
{
    [Serializable]
    public class AspectCache : OnMethodBoundaryAspect
    {
        private readonly string _keyPrefix;
        private readonly string _identifier;
        private readonly CacheFunction _function;
        private bool _found;
        private readonly ICache _cache;

        public AspectCache(string keyPrefix, CacheFunction function, Type cache, string identifier = "")
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
            if (idField == "") return "";
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
            var conFore = Console.ForegroundColor;

            if (_function == CacheFunction.Retrieve || _function == CacheFunction.RetrieveOrAdd)
            {
                object item;

                if (_cache.GetItem(_keyPrefix + idValue, out item))
                {
                    _found = true;
                    args.FlowBehavior = FlowBehavior.Return;
                    args.ReturnValue = item;
                    Console.ForegroundColor = System.ConsoleColor.Green;
                    Console.WriteLine("\t\tInCache");
                    Console.ForegroundColor = conFore;
                }
                else
                {
                    _found = false;
                    Console.ForegroundColor = System.ConsoleColor.Yellow;
                    Console.WriteLine("\t\tNot Cached");
                    Console.ForegroundColor = conFore;
                    
                }

            }
            //This is repeated in exit incase any items in method have retrieved and re added the item
            //to the cache. This guarenttes that the start and end of the method will have a clean cache
            //for this item
            else if (_function == CacheFunction.Invalidate)
            {
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
            }
            //This is repeated in exit incase any items in method have retrieved and re added the item
            //to the cache. This guarenttes that the start and end of the method will have a clean cache
            //for this item
            else if ( _function == CacheFunction.Invalidate)
            {

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
