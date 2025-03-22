using Microsoft.EntityFrameworkCore;
using RegistrationService.Core.Entities;
using System.Reflection;

namespace RegistrationService.Data.Context
{
    public class RegistrationServiceDbContext : DbContext
    {
        public RegistrationServiceDbContext(DbContextOptions options) : base(options) { }

        public DbSet<Registration> Registrations => Set<Registration>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
