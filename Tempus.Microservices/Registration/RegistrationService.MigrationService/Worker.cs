using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Trace;
using RegistrationService.Data.Context;
using System.Diagnostics;

namespace RegistrationService.MigrationService
{
    public class Worker(
     IServiceProvider serviceProvider,
     IHostApplicationLifetime hostApplicationLifetime,
     IHostEnvironment hostEnvironment) : BackgroundService
    {
        public const string ActivitySourceName = "Migrations";
        private static readonly ActivitySource s_activitySource = new(ActivitySourceName);

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            using var activity = s_activitySource.StartActivity("Migrating database", ActivityKind.Client);

            try
            {
                using var scope = serviceProvider.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<RegistrationServiceDbContext>();

                await RunMigrationAsync(dbContext, cancellationToken);
            }
            catch (Exception ex)
            {
                activity?.RecordException(ex);
                throw;
            }

            hostApplicationLifetime.StopApplication();
        }

        private static async Task RunMigrationAsync(RegistrationServiceDbContext dbContext, CancellationToken cancellationToken)
        {
            var strategy = dbContext.Database.CreateExecutionStrategy();

            await strategy.ExecuteAsync(async () =>
            {
                var pendingMigrations = await dbContext.Database.GetPendingMigrationsAsync(cancellationToken);

                if (pendingMigrations.Any())
                {
                    await dbContext.Database.MigrateAsync(cancellationToken);
                }
            });
        }
    }
}
