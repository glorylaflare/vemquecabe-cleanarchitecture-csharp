using VemQueCabe.Application.Profiles;

namespace VemQueCabe.Api.Extensions;

/// <summary>
/// Provides extension methods for adding mapping configurations to the service collection.
/// </summary>
public static class MapperCollectionExtensions
{
    /// <summary>
    /// Adds AutoMapper services to the service collection, scanning for profiles in the specified assembly.
    /// </summary>
    /// <param name="services">The service collection to add the mapping services to.</param>
    /// <returns></returns>
    public static IServiceCollection AddMapping(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(UserProfile).Assembly);
        return services;
    }
}
