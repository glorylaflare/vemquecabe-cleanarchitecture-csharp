namespace VemQueCabe.Application.Interfaces;

/// <summary>
/// Defines a contract for a caching service.
/// </summary>
public interface ICacheService
{
    /// <summary>
    /// Retrieves a cached value by its key.
    /// </summary>
    /// <typeparam name="T">The type of the cached value.</typeparam>
    /// <param name="key">The key of the cached value.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the cached value, or null if the key does not exist.</returns>
    Task<T?> GetAsync<T>(string key);

    /// <summary>
    /// Caches a value with a specified key and expiration time.
    /// </summary>
    /// <typeparam name="T">The type of the value to cache.</typeparam>
    /// <param name="key">The key to associate with the cached value.</param>
    /// <param name="value">The value to cache.</param>
    /// <param name="expiration">The duration after which the cached value expires.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task SetAsync<T>(string key, T value, TimeSpan expiration);

    /// <summary>
    /// Removes a cached value by its key.
    /// </summary>
    /// <param name="key">The key of the cached value to remove.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task RemoveAsync(string key);
    
    /// <summary>
    /// Removes cached values that match a specified pattern.
    /// </summary>
    /// <param name="pattern">The pattern to match keys against.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>   
    Task RemoveByPatternAsync(string pattern);
}
