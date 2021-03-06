﻿using System.Collections.Generic;
using System.Linq;
using AspectCache.CacheProvider.Redis;
using AspectCache.Sample.Model;

namespace AspectCache.Sample.Data.Repository
{
    public class ProductRepository 
    {
        readonly EFContext _ctx = new EFContext();

        [AspectCache(keyPrefix: "Products", 
            function: CacheFunction.RetrieveOrAdd, cache: typeof(RedisCache))]
        public List<Product> GetAll()
        {
            return _ctx.Products.ToList();
        }

        [AspectCache(keyPrefix: "Products", function: CacheFunction.Invalidate, cache: typeof(RedisCache))]
        [AspectCache(keyPrefix: "Product", identifier: "product.Id", function: CacheFunction.Invalidate, cache: typeof(RedisCache))]
        public Product AddOrUpdate(Product product)
        {
            var savedProduct = product;
            if (product.Id < 1)
                _ctx.Products.Add(product);
            else
            {
                var toEdit = Get(product.Id);
                toEdit.Name = product.Name;
                savedProduct = toEdit;
            }
            _ctx.SaveChanges();
            return savedProduct;
        }

        [AspectCache(keyPrefix: "Products", function: CacheFunction.Invalidate, cache: typeof(RedisCache))]
        [AspectCache(keyPrefix: "Product", identifier: "id", function: CacheFunction.Invalidate, cache: typeof(RedisCache))]
        public void Delete(int id)
        {
            var product = Get(id);
            _ctx.Products.Remove(product);
            _ctx.SaveChanges();
        }

        [AspectCache(keyPrefix: "Products", function: CacheFunction.Invalidate, cache: typeof(RedisCache))]
        [AspectCache(keyPrefix: "Product", identifier: "product.Id", function: CacheFunction.Invalidate, cache: typeof(RedisCache))]
        public void Delete(Product product)
        {
            _ctx.Products.Remove(product);
            _ctx.SaveChanges();
        }
        
        [AspectCache(keyPrefix: "Product", identifier: "id", function: CacheFunction.RetrieveOrAdd, cache: typeof(RedisCache))]
        public Product Get(int id)
        {
            return _ctx.Products.FirstOrDefault(x => x.Id == id);
        }


        [AspectCache(keyPrefix: "Products", function: CacheFunction.Invalidate, cache: typeof(RedisCache))]
        public void Clear()
        {
            var products = GetAll();
            foreach (var p in products)
                Delete(p.Id);
            _ctx.SaveChanges();
        }
    }
}
