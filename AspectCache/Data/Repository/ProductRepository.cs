using System.Collections.Generic;
using System.Linq;
using EFCacheTest.Cache;
using EFCacheTest.Model;

namespace EFCacheTest.Data.Repository
{
    class ProductRepository 
    {
        readonly EFContext _ctx = new EFContext();

        public IEnumerable<Product> GetAll()
        {
            return _ctx.Products;
        }

        public void AddOrUpdate(Product product)
        {
            if (product.Id < 1)
                _ctx.Products.Add(product);
            else
            {
                var toEdit = Get(product.Id);
                toEdit.Name = product.Name;
            }
            _ctx.SaveChanges();
        }

        public void Delete(int id)
        {
            var product = Get(id);
            _ctx.Products.Remove(product);
            _ctx.SaveChanges();
        }


        [CacheAspect(keyPrefix: "Product", identifier: "id", function: CacheFunction.RetrieveOrAdd, cache: typeof(DictionaryCache))]
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
