using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RegistrationService.Data.Context;
using System.Reflection;

namespace RegistrationService.Data;

public static class ConfigurationService
{
    public static void AddDb(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("RegistrationService");

        var migrationsAssembly = typeof(RegistrationServiceDbContext).GetTypeInfo().Assembly.GetName().Name;
        services.AddDbContext<RegistrationServiceDbContext>(options => options.UseSqlServer(connectionString, sql =>
        {
            sql.MigrationsHistoryTable("__EFMigrationHistory");
        }));
    }
}
