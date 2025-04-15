using GroupService.Data.Context;
using GroupService.MigrationService;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddHostedService<Worker>();

builder.AddSqlServerDbContext<GroupServiceDbContext>(connectionName: "GroupService");

var host = builder.Build();
host.Run();
