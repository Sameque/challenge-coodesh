namespace OrderAccumulator.Domain.Interfaces;

public interface IExposureRepository
{
    Task<decimal> GetExposureAsync(string symbol, CancellationToken cancellationToken = default);
    Task UpdateExposureAsync(string symbol, decimal delta, CancellationToken cancellationToken = default);
}
