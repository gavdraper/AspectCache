using System.Data.Entity;
using AspectCache.Sample.Model;

namespace AspectCache.Sample.Data
{
    public class EFContext : DbContext
    {
        public DbSet<Product> Products { get; set; }
    }
}
