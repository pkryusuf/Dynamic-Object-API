using CRUD.API.Models;
using Microsoft.EntityFrameworkCore;

namespace CRUD.API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<DynamicObject> DynamicObjects { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DynamicObject>().ToTable("DynamicObjects");
        }
    }
}
