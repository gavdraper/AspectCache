using System;
using System.Linq;
using EFCacheTest.Data.Repository;
using EFCacheTest.Model;

namespace EFCacheTest
{
    class Program
    {
        private static readonly ProductRepository repo = new ProductRepository();

        static void Main(string[] args)
        {
            repo.Clear();
            repo.AddOrUpdate(new Product() {Name = "24GB RAM"});
            repo.AddOrUpdate(new Product() { Name = "24\" Monitor"});

            
            var got = repo.Get(repo.GetAll().FirstOrDefault().Id);
            Console.WriteLine(got.Id + " " + got.Name);
            got = repo.Get(repo.GetAll().FirstOrDefault().Id);
            Console.WriteLine(got.Id + " " + got.Name);

            Console.ReadLine();

        }
    }
}

