using CategoryService.Core.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace CategoryService.Data.Context
{
    public class CategoryServiceDbContext(DbContextOptions options) : DbContext(options)
    {
        public DbSet<Category> Categories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
