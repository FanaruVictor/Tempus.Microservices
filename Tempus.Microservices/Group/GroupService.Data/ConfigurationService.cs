using GroupService.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace GroupService.Data;

public static class ConfigurationService
{
    public static void AddDb(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("GroupService");

        var migrationsAssembly = typeof(GroupServiceDbContext).GetTypeInfo().Assembly.GetName().Name;
        services.AddDbContext<GroupServiceDbContext>(options => options.UseSqlServer(connectionString, sql =>
        {
            sql.MigrationsAssembly(migrationsAssembly);
            sql.MigrationsHistoryTable("__EFMigrationHistory");
        }));
    }
}