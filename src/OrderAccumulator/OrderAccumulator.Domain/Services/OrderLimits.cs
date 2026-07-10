namespace OrderAccumulator.Domain.Services;

public static class OrderLimits
{
    public const decimal MaxPrice = 1000m;
    public const int MaxQuantity = 100000;
    public const decimal MaxExposure = 100_000_000.00m;
}