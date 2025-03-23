using Microsoft.EntityFrameworkCore;
using RegistrationService.Core.Entities;
using System.Reflection;

namespace RegistrationService.Data.Context
{
    public class RegistrationServiceDbContext(DbContextOptions options) : DbContext(options)
    {
        public DbSet<Registration> Registrations => Set<Registration>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
