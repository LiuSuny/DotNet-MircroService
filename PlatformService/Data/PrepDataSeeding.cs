using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PlatformService.Models;

namespace PlatformService.Data
{
    public static class PrepDataSeeding
    {
        public static void SeedData(AppDbContext context)
        {
           if(!context.Platforms.Any())
           {
             Console.WriteLine("---> seeding data...");
             var items = new List<Platform>
             {
                 new(){Name= "Dot Net", Publisher="Microsoft", Cost="free"},
                 new(){Name= "SQL Sever Express", Publisher="Microsoft", Cost="free"},
                 new(){Name= "Kubernetes", Publisher="Cloud Native Computing Foundation", Cost="free"}
             };

             context.Platforms.AddRange(items);
              context.SaveChanges();
           }
           else{
            Console.WriteLine("---> We already have data");
           }

        }
    }
}