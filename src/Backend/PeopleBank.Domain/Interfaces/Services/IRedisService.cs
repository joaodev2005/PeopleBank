namespace PeopleBank.Domain.Interfaces.Services;

public interface IRedisService
{
    Task<T?> GetIdempotentResponseAsync<T>(string key);
    Task SetIdempotentResponseAsync<T>(string key, T response, int expirationMinutes = 5);
    Task<bool> AcquireLockAsync(string key, decimal value);
    Task ReleaseLockAsync(string key);
}
