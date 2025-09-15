using VemQueCabe.Application.Interfaces;
using VemQueCabe.Application.Services;

namespace VemQueCabe.Api.Extensions;

/// <summary>
/// Provides extension methods for configuring application services in the dependency injection container.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers application services by scanning the specified assemblies and adding classes in the "VemQueCabe.Application.Services" namespace as their implemented interfaces with a scoped lifetime.
    /// </summary>
    /// <param name="services">The IServiceCollection to add the services to.</param>
    /// <returns></returns>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.Scan(services => services
            .FromAssembliesOf(typeof(IUserService), typeof(UserService))
            .AddClasses(classes => classes
            .InNamespaces("VemQueCabe.Application.Services"))
            .AsImplementedInterfaces()
            .WithScopedLifetime());

        return services;
    }
}
