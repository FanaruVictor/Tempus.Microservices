using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RegistrationService.Data.Context;
using System.Reflection;

namespace RegistrationService.Data;

public static class Configuration
{
    public static void AddDb(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration["registration-service-connection-string"];

        var migrationsAssembly = typeof(RegistrationServiceDbContext).GetTypeInfo().Assembly.GetName().Name;

        services.AddDbContext<RegistrationServiceDbContext>(options => options.UseSqlServer(connectionString, sql =>
        {
            sql.MigrationsAssembly(migrationsAssembly);
            sql.MigrationsHistoryTable("__EFMigrationHistory");
        }));

    }
}