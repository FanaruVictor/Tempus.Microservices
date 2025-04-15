using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Trace;
using System.Diagnostics;
using UserService.Core.Entities;
using UserService.Data.Context;
using BCryptNet = BCrypt.Net;

namespace UserService.MigrationService
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
                var dbContext = scope.ServiceProvider.GetRequiredService<UserServiceDbContext>();

                await RunMigrationAsync(dbContext, cancellationToken);

                if (hostEnvironment.IsDevelopment())
                {
                    await SeedDataAsync(dbContext, cancellationToken);
                }
            }
            catch (Exception ex)
            {
                activity?.RecordException(ex);
                throw;
            }

            hostApplicationLifetime.StopApplication();
        }

        private static async Task RunMigrationAsync(UserServiceDbContext dbContext, CancellationToken cancellationToken)
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

        private static async Task SeedDataAsync(UserServiceDbContext dbContext, CancellationToken cancellationToken)
        {
            User user = new()
            {
                Username = "admin",
                Email = "admin@admin.com",
                Password = BCryptNet.BCrypt.HashPassword("admin"),
            };

            var strategy = dbContext.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () =>
            {
                var user = await dbContext.Users
                    .FirstOrDefaultAsync(x => x.Email == "admin@admin.com", cancellationToken);

                if (user == null)
                {
                    await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);

                    await dbContext.Users.AddAsync(user, cancellationToken);

                    await dbContext.SaveChangesAsync(cancellationToken);

                    await transaction.CommitAsync(cancellationToken);
                }
            });
        }
    }
}
