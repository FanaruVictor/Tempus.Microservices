using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using UserService.Data.Context;

namespace UserService.Data
{
    public static class ConfigurationService
    {
        public static void AddDb(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("UserService");

            var migrationsAssembly = typeof(UserServiceDbContext).GetTypeInfo().Assembly.GetName().Name;
            services.AddDbContext<UserServiceDbContext>(options => options.UseSqlServer(connectionString, sql =>
            {
                sql.MigrationsAssembly(migrationsAssembly);
                sql.MigrationsHistoryTable("__EFMigrationHistory");
            }));
        }
    }
}
