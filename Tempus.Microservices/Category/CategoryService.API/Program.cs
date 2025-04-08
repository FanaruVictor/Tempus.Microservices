using CategoryService.API.Jobs;
using CategoryService.Data;
using CategoryService.Infrastructure;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Configuration.AddUserSecrets(Assembly.GetExecutingAssembly());
builder.Configuration.AddEnvironmentVariables();

builder.Services.AddDb(builder.Configuration);
builder.Services.AddInfrastructureServices();

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
builder.Services.AddHostedService<DeleteGroupJob>();

var app = builder.Build();

app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();