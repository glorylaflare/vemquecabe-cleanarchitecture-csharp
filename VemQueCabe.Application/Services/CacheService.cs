using System.Text.Json;
using StackExchange.Redis;
using VemQueCabe.Application.Interfaces;

namespace VemQueCabe.Application.Services;

/// <summary>
/// Provides caching functionalities using a distributed cache.
/// </summary>
public class CacheService : ICacheService
{
    private readonly IConnectionMultiplexer _connection;
    private readonly IDatabase _data;

    public CacheService(IConnectionMultiplexer connection)
    {
        _connection = connection;
        _data = _connection.GetDatabase();
    }
    
    public async Task<T?> GetAsync<T>(string key)
    {
        var value = await _data.StringGetAsync(key);
        return value.IsNullOrEmpty ? default : JsonSerializer.Deserialize<T>(value);
    }
    
    public async Task SetAsync<T>(string key, T value, TimeSpan expiration)
    {
        var jsonData = JsonSerializer.Serialize(value);
        await _data.StringSetAsync(key, jsonData, expiration);
    }
    
    public async Task RemoveAsync(string key)
    {
        await _data.KeyDeleteAsync(key);
    }
    
    public async Task RemoveByPatternAsync(string pattern)
    {
        var server = _connection.GetServer(_connection.GetEndPoints().First());
        var keys = server.Keys(pattern: $"*{pattern}*");
        foreach (var key in keys)
        {
            await _data.KeyDeleteAsync(key);
        }
    }
}
