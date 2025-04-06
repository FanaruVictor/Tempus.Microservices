using System.Reflection;
using FluentValidation;
using FluentValidation.AspNetCore;
using GroupService.Infrastructure.Queries.Groups.GetAllGroupsQuery;
using GroupService.Infrastructure.Services.Cloudynary;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tempus.Shared.Commons;

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
        services.AddScoped<ICloudinaryService, CloudinaryService>();
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(GetAllGroupsQuery).Assembly));
        services.Configure<CloudinarySettings>(configuration.GetSection("CloudinarySettings"));

        return services;
    }
}