using Microsoft.EntityFrameworkCore;
using OrderAccumulator.Domain.Enums;
using OrderAccumulator.Domain.Interfaces;
using OrderAccumulator.Domain.Services;

namespace OrderAccumulator.Infrastructure.Persistence;

public class ExposureInitializer
{
    private readonly OrderAccumulatorDbContext _db;
    private readonly IExposureRepository _exposureRepository;

    public ExposureInitializer(OrderAccumulatorDbContext db, IExposureRepository exposureRepository)
    {
        _db = db;
        _exposureRepository = exposureRepository;
    }

    public async Task InitializeAsync()
    {
        await _db.Database.MigrateAsync();

        var orders = await _db.Orders
            .Where(o => o.Status == OrderStatus.Accepted)
            .ToListAsync();

        var exposures = orders.GroupBy(o => o.Symbol)
            .Select(g => new
            {
                Symbol = g.Key,
                TotalExposure = ExposureCalculator.CalculateExposure(g)
            });

        foreach (var exp in exposures)
        {
            await _exposureRepository.UpdateExposureAsync(exp.Symbol, exp.TotalExposure);
        }
    }
}
