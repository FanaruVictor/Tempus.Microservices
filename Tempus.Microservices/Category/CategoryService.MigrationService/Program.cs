using CategoryService.Data.Context;
using CategoryService.MigrationService;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddHostedService<Worker>();

builder.Services.AddOpenTelemetry()
    .WithTracing(tracing => tracing.AddSource(Worker.ActivitySourceName));

builder.AddSqlServerDbContext<CategoryServiceDbContext>(connectionName: "CategoryService");


var host = builder.Build();
host.Run();
