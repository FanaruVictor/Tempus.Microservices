using Microsoft.EntityFrameworkCore;
using System.Reflection;
using UserService.Core.Entities;

namespace UserService.Data.Context
{
    public class UserServiceDbContext(DbContextOptions options) : DbContext(options)
    {
        public DbSet<User> Users => Set<User>();
        public DbSet<UserPhoto> UserPhotos => Set<UserPhoto>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
