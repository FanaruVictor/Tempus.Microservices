using FluentValidation;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RegistrationService.Infrastructure.Commands.Registrations.Create;
using RegistrationService.Infrastructure.Commons;
using RegistrationService.Infrastructure.IServices;
using RegistrationService.Infrastructure.Services;

namespace RegistrationService.Infrastructure;

public static class ConfigureServices
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddHttpContextAccessor();
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(MediatrRequestContextBehaviour<,>));
        services.AddFluentValidationAutoValidation();
        services.AddValidatorsFromAssemblyContaining<CreateRegistrationCommand>();
        services.Configure<CloudinarySettings>(configuration.GetSection("CloudinarySettings"));
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<CreateRegistrationCommand>());
        services.AddScoped<ICloudinaryService, CloudinaryService>();

        return services;
    }
}