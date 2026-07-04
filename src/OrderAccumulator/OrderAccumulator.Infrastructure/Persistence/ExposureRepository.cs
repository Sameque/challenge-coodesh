using Microsoft.Extensions.Caching.Memory;
using OrderAccumulator.Domain.Interfaces;
using System.Threading.Tasks;

namespace OrderAccumulator.Infrastructure.Persistence;

public class ExposureRepository : IExposureRepository
{
    private readonly IMemoryCache _cache;
    private readonly object _lock = new();
    private const string CacheKeyPrefix = "exposure_";

    public ExposureRepository(IMemoryCache cache)
    {
        _cache = cache;
    }

    public Task<decimal> GetExposureAsync(string symbol)
    {
        string key = $"{CacheKeyPrefix}{symbol.ToUpper()}";
        
        if (_cache.TryGetValue(key, out decimal exposure))
            return Task.FromResult(exposure);

        return Task.FromResult(0m);
    }

    public Task UpdateExposureAsync(string symbol, decimal delta)
    {
        string key = $"{CacheKeyPrefix}{symbol.ToUpper()}";

        lock (_lock)
        {
            decimal currentExposure = 0;
            if (_cache.TryGetValue(key, out decimal existingValue))
            {
                currentExposure = existingValue;
            }

            _cache.Set(key, currentExposure + delta);
        }

        return Task.CompletedTask;
    }
}
