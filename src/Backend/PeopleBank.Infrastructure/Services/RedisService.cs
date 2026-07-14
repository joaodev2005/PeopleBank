using System.Text.Json;
using PeopleBank.Domain.Interfaces.Services;
using StackExchange.Redis;

namespace PeopleBank.Infrastructure.Services;

public class RedisService : IRedisService
{
    private readonly IDatabase _database;

    public RedisService(IConnectionMultiplexer redis)
    {
        _database = redis.GetDatabase();
    }

    public async Task<T?> GetIdempotentResponseAsync<T>(string key)
    {
        var value = await _database.StringGetAsync($"idempotency:{key}");
        if (!value.HasValue)
            return default;

        var json = (string)value!;
        return JsonSerializer.Deserialize<T>(json);
    }

    public async Task SetIdempotentResponseAsync<T>(string key, T response, int expirationMinutes = 5)
    {
        var json = JsonSerializer.Serialize(response);
        await _database.StringSetAsync(
            $"idempotency:{key}",
            json,
            TimeSpan.FromMinutes(expirationMinutes)
        );
    }

    public async Task<bool> AcquireLockAsync(string key, decimal value)
    {
        var lockKey = $"lock:{key}";
        var lockValue = Guid.NewGuid().ToString();

        var acquired = await _database.LockTakeAsync(lockKey, lockValue, TimeSpan.FromSeconds(30));

        if (acquired)
        {
            await _database.StringSetAsync($"{lockKey}:value", value.ToString(), TimeSpan.FromSeconds(30));
        }

        return acquired;
    }

    public async Task ReleaseLockAsync(string key)
    {
        var lockKey = $"lock:{key}";
        await _database.KeyDeleteAsync(lockKey);
        await _database.KeyDeleteAsync($"{lockKey}:value");
    }
}