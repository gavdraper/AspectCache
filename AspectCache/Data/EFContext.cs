using System.Data.Entity;
using EFCacheTest.Model;

namespace EFCacheTest.Data
{
    public class EFContext : DbContext
    {
        public DbSet<Product> Products { get; set; }
    }
}
