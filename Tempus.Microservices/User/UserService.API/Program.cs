using Microsoft.AspNetCore.Http.Features;
using System.Reflection;
using UserService.Data;
using UserService.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Configuration.AddUserSecrets(Assembly.GetExecutingAssembly());
builder.Configuration.AddEnvironmentVariables();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDb(builder.Configuration);
builder.Services.AddInfrastructureServices(builder.Configuration);


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
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 50 * 1024 * 1024; // Allow 50MB uploads
});


//builder.Services.AddMassTransit(busConfigurator =>
//{
//    busConfigurator.UsingRabbitMq((context, cfg) =>
//        {
//            cfg.Host(builder.Configuration["RabbitMQ:Host"], builder.Configuration["RabbitMQ:Port"], "/", h =>
//            {
//                h.Username(builder.Configuration["RabbitMQ:Username"]);
//                h.Password(builder.Configuration["RabbitMQ:Password"]);
//            });
//        });
//});

builder.AddRabbitMQClient("messaging");

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
