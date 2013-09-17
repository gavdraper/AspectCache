using System;
using System.Collections.Generic;
using System.Linq;
using PostSharp.Aspects;

namespace EFCacheTest.Cache
{
    [Serializable]
    class CacheAspect : OnMethodBoundaryAspect
    {
        private readonly string _keyPrefix;
        private readonly string _identifier;
        private readonly CacheFunction _function;
        private bool _found;
        private readonly ICache _cache;

        public CacheAspect(string keyPrefix, string identifier, CacheFunction function, Type cache)
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

        public override void OnEntry(MethodExecutionArgs args)
        {
            var methodParams = getParams(args);

            if (_function == CacheFunction.Retrieve || _function == CacheFunction.RetrieveOrAdd)
            {
                object item;

                if (_cache.GetItem(_keyPrefix + methodParams[_identifier], out item))
                {
                    _found = true;
                    args.FlowBehavior = FlowBehavior.Return;
                    args.ReturnValue = item;
                    Console.WriteLine("Found In Cache");
                }
                else Console.WriteLine("Not Found In Cache");
            }
            else if (_function == CacheFunction.Invalidate)
            {
                _cache.InvalidateItem(_keyPrefix + methodParams[_identifier]);
            }
        }

        public override void OnExit(MethodExecutionArgs args)
        {
            var methodParams = getParams(args);
            if (!_found && _function == CacheFunction.RetrieveOrAdd)
            {
                _cache.AddItem(_keyPrefix + methodParams[_identifier], args.ReturnValue);
                Console.WriteLine("Added to cache");
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
