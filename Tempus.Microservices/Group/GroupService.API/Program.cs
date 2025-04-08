using GroupService.API.Jobs;
using GroupService.Data;
using System.Reflection;
using UserService.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Configuration.AddUserSecrets(Assembly.GetExecutingAssembly());
builder.Configuration.AddEnvironmentVariables();

builder.Services.AddDb(builder.Configuration);
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

builder.Services.AddHostedService<DeleteUserJob>();

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