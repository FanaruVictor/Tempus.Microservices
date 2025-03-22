using FluentValidation;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using UserService.Infrastructure.Commons;
using UserService.Infrastructure.IServices;
using UserService.Infrastructure.Queries.Users.GetAll;
using UserService.Infrastructure.Services.Cloudynary;

namespace UserService.Infrastructure;

public static class ConfigureServices
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddHttpContextAccessor();
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(MediatrRequestContextBehaviour<,>));
        services.AddFluentValidationAutoValidation();
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        services.Configure<CloudinarySettings>(configuration.GetSection("CloudinarySettings"));
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(GetAllUsersQuery).Assembly));
        services.AddScoped<ICloudinaryService, CloudinaryService>();

        return services;
    }
}
