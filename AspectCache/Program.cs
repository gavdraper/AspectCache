using System;
using System.Linq;
using AspectCache.Data.Repository;
using AspectCache.Model;

namespace AspectCache
{
    class Program
    {
        private static readonly ProductRepository repo = new ProductRepository();

        static void Main(string[] args)
        {
            repo.Clear();
            
            repo.AddOrUpdate(new Product() {Name = "24GB RAM"});            
            repo.AddOrUpdate(new Product() {Name = "24\" Monitor"});

            var all = repo.GetAll();

            Console.WriteLine("Items In DB....");
            foreach(var a in all)
                Console.WriteLine("{0} {1}",a.Id,a.Name);
            Console.WriteLine("...........................");

            var item = all.FirstOrDefault();

            Console.WriteLine("Requesting " + item.Id);
            var got = repo.Get(all.FirstOrDefault().Id);
            Console.WriteLine("\t\tRetrieved {0} {1}",got.Id,got.Name);
            
            Console.WriteLine("Requesting " + item.Id);
            got = repo.Get(all.FirstOrDefault().Id);
            Console.WriteLine("\t\tRetrieved {0} {1}", got.Id, got.Name);

            Console.WriteLine("Updating " + item.Id);
            item.Name += " V2";
            repo.AddOrUpdate(item);

            Console.WriteLine("Requesting " + item.Id);
            got = repo.Get(all.FirstOrDefault().Id);
            Console.WriteLine("\t\tRetrieved {0} {1}", got.Id, got.Name);

            Console.WriteLine("Requesting " + item.Id);
            got = repo.Get(all.FirstOrDefault().Id);
            Console.WriteLine("\t\tRetrieved {0} {1}", got.Id, got.Name);


            Console.WriteLine("Requesting All");
            var all2 = repo.GetAll();
            Console.WriteLine("\t\tRetrieved ");


            Console.WriteLine("Requesting All");
             all2 = repo.GetAll();
            Console.WriteLine("\t\tRetrieved ");

            Console.WriteLine("Updating " + item.Id);
            item.Name += " V3";
            repo.AddOrUpdate(item);

            Console.WriteLine("Requesting All");
            all2 = repo.GetAll();
            Console.WriteLine("\t\tRetrieved ");


            Console.WriteLine("Requesting All");
            all2 = repo.GetAll();
            Console.WriteLine("\t\tRetrieved ");

            Console.ReadLine();

        }
    }
}

