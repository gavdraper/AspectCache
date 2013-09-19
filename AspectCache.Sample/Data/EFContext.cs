using System.Data.Entity;
using AspectCache.Model;

namespace AspectCache.Data
{
    public class EFContext : DbContext
    {
        public DbSet<Product> Products { get; set; }
    }
}
