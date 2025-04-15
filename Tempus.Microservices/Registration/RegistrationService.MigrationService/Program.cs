using RegistrationService.Data.Context;
using RegistrationService.MigrationService;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddHostedService<Worker>();

builder.Services.AddOpenTelemetry()
    .WithTracing(tracing => tracing.AddSource(Worker.ActivitySourceName));

builder.AddSqlServerDbContext<RegistrationServiceDbContext>(connectionName: "RegistrationService");

var host = builder.Build();
host.Run();
