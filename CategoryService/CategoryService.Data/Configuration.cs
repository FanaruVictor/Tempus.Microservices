using CategoryService.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace CategoryService.Data;

public static class Configuration
{
    public static void AddDb(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration["category-service-connection-string"];

        var migrationsAssembly = typeof(CategoryServiceDbContext).GetTypeInfo().Assembly.GetName().Name;

        services.AddDbContext<CategoryServiceDbContext>(options => options.UseSqlServer(connectionString, sql =>
        {
            sql.MigrationsAssembly(migrationsAssembly);
            sql.MigrationsHistoryTable("__EFMigrationHistory");
        }));
    }
}
