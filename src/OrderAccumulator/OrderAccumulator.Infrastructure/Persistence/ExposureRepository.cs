using OrderAccumulator.Domain.Interfaces;
using StackExchange.Redis;

namespace OrderAccumulator.Infrastructure.Persistence;

public class ExposureRepository : IExposureRepository
{
    private readonly IDatabase _redis;
    private const string CacheKeyPrefix = "exposure_";

    public ExposureRepository(IConnectionMultiplexer connectionMultiplexer)
    {
        _redis = connectionMultiplexer.GetDatabase();
    }

    public async Task<decimal> GetExposureAsync(string symbol, CancellationToken cancellationToken = default)
    {
        string key = $"{CacheKeyPrefix}{symbol.ToUpper()}";
        var value = await _redis.StringGetAsync(key);

        if (value.HasValue && decimal.TryParse((string?)value, out decimal exposure))
            return exposure;

        return 0m;
    }

    public async Task UpdateExposureAsync(string symbol, decimal delta, CancellationToken cancellationToken = default)
    {
        string key = $"{CacheKeyPrefix}{symbol.ToUpper()}";
        await _redis.StringIncrementAsync(key, (double)delta);
    }
}
