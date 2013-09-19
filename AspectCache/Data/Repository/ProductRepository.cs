using System.Collections.Generic;
using System.Linq;
using AspectCache.Cache;
using AspectCache.Model;

namespace AspectCache.Data.Repository
{
    class ProductRepository 
    {
        readonly EFContext _ctx = new EFContext();

        public IEnumerable<Product> GetAll()
        {
            return _ctx.Products;
        }

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

        [AspectCache(keyPrefix: "Product", identifier: "id", function: CacheFunction.Invalidate, cache: typeof(RedisCache))]
        public void Delete(int id)
        {
            var product = Get(id);
            _ctx.Products.Remove(product);
            _ctx.SaveChanges();
        }

        [AspectCache(keyPrefix: "Product", identifier: "id", function: CacheFunction.RetrieveOrAdd, cache: typeof(RedisCache))]
        public Product Get(int id)
        {
            return _ctx.Products.FirstOrDefault(x => x.Id == id);
        }

        public void Clear()
        {
            var products = GetAll();
            foreach (var p in products)
                _ctx.Products.Remove(p);
            _ctx.SaveChanges();
        }
    }
}
