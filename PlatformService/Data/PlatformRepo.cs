using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using PlatformService.Models;

namespace PlatformService.Data
{
    public class PlatformRepo : IPlatformRepo
    {
        private readonly AppDbContext _context;

        public PlatformRepo(AppDbContext dbContext)
        {
            _context = dbContext;
        }
        public void CreatePlatform(Platform platform)
        {
            if(platform == null) return;
           _context.Platforms.Add(platform);
        }

        public IEnumerable<Platform> GetAllPlatforms()
        {
           return _context.Platforms.ToList();
        }

        public Platform GetPlatformsById(int id)
        {
           return _context.Platforms.FirstOrDefault(p => p.Id == id);
        }

        public bool SaveChanges()
        {
           return(_context.SaveChanges() >= 0);
        }
    }
}