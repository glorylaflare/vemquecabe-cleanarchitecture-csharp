using VemQueCabe.Domain.Shared;
using VemQueCabe.Infra.Data;

namespace VemQueCabe.Api.Extensions;

/// <summary>
/// Provides extension methods for adding UnitOfWork to the service collection.
/// </summary>
public static class UnitOfWorkCollectionExtensions
{
    /// <summary>
    /// Adds the UnitOfWork service to the IServiceCollection.
    /// </summary>
    /// <param name="services">The IServiceCollection to add the service to.</param>
    /// <returns></returns>
    public static IServiceCollection AddUnitOfWork(this IServiceCollection services)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        return services;
    }
}
