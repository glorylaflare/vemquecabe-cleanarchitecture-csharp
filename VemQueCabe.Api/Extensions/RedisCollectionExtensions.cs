using StackExchange.Redis;
using VemQueCabe.Application.Interfaces;
using VemQueCabe.Application.Services;

namespace VemQueCabe.Api.Extensions;

/// <summary>
/// Provides extension methods for configuring Redis cache in the application.
/// </summary>
public static class RedisCollectionExtensions
{
    /// <summary>
    /// Configures Redis cache services.
    /// </summary>
    /// <param name="services">The service collection to add the Redis cache services to.</param>
    /// <param name="configuration">The application configuration containing Redis settings.</param>
    /// <returns></returns>
    public static IServiceCollection AddRedisCache(this IServiceCollection services, IConfiguration configuration)
    {
        
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = configuration.GetConnectionString("Redis");
        });
        
        services.AddSingleton<IConnectionMultiplexer>(_ =>
        {
            var options = ConfigurationOptions.Parse(configuration.GetConnectionString("Redis"), true);
            return ConnectionMultiplexer.Connect(options);
        });
        
        services.AddScoped<ICacheService, CacheService>();
        return services;
    }
}
