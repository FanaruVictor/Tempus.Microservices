using RegistrationService.API.Jobs;
using RegistrationService.Data;
using RegistrationService.Data.Context;
using RegistrationService.Infrastructure;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Configuration.AddUserSecrets(Assembly.GetExecutingAssembly());
builder.Configuration.AddEnvironmentVariables();

builder.Services.AddDb(builder.Configuration);
builder.EnrichSqlServerDbContext<RegistrationServiceDbContext>(configureSettings: settings =>
{
    settings.DisableRetry = false;
    settings.CommandTimeout = 30;

});
builder.Services.AddInfrastructureServices(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var allowedOrigin = builder.Configuration["AllowedOrigin"].Split(',') ?? ["*"];

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        policyBuilder =>
        {
            policyBuilder.WithOrigins(allowedOrigin);
            policyBuilder.AllowAnyHeader();
            policyBuilder.AllowAnyMethod();
        });
});


builder.AddRabbitMQClient("messaging");

builder.Services.AddHostedService<DeleteCategoryJob>();

builder.Services.AddHttpClient("categoryservice-api", static client => client.BaseAddress = new("https://categoryservice-api"));

var app = builder.Build();

app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
