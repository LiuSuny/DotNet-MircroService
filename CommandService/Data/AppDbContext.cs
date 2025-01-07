using CommandService.Models;
using Microsoft.EntityFrameworkCore;

namespace CommandService.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options): base(options)
        {
            
        }

        public DbSet<Command> Commands {get; set;}
        public DbSet<Platform> Platforms {get; set;}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Platform>()
                       .HasMany(r => r.Commands)
                       .WithOne(c => c.Platform!)
                       .HasForeignKey(f => f.PlatformId);

            modelBuilder.Entity<Command>()
                       .HasOne(r => r.Platform)
                       .WithMany(c => c.Commands!)
                       .HasForeignKey(f => f.PlatformId);

        }
    }
}