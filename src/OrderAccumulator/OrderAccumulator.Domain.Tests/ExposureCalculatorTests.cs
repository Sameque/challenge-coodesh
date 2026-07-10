using OrderAccumulator.Domain.Entities;
using OrderAccumulator.Domain.Enums;
using OrderAccumulator.Domain.Services;

namespace OrderAccumulator.Domain.Tests;

public class ExposureCalculatorTests
{
    [Fact]
    public void CalculateDelta_BuyOrder_ReturnsPositiveValue()
    {
        var delta = ExposureCalculator.CalculateDelta(OrderSide.Buy, 100m, 10m);
        Assert.Equal(1000m, delta);
    }

    [Fact]
    public void CalculateDelta_SellOrder_ReturnsNegativeValue()
    {
        var delta = ExposureCalculator.CalculateDelta(OrderSide.Sell, 100m, 10m);
        Assert.Equal(-1000m, delta);
    }

    [Fact]
    public void CalculateExposure_MixedOrders_CalculatesCorrectly()
    {
        var orders = new List<Order>
        {
            CreateOrder(100m, 10m, OrderSide.Buy),
            CreateOrder(50m, 20m, OrderSide.Sell)
        };

        var exposure = ExposureCalculator.CalculateExposure(orders);

        Assert.Equal(0m, exposure);
    }

    [Fact]
    public void CalculateExposure_BuyOnly_ReturnsPositiveExposure()
    {
        var orders = new List<Order>
        {
            CreateOrder(100m, 10m, OrderSide.Buy),
            CreateOrder(200m, 5m, OrderSide.Buy)
        };

        var exposure = ExposureCalculator.CalculateExposure(orders);

        Assert.Equal(2000m, exposure);
    }

    [Fact]
    public void CalculateExposure_EmptyList_ReturnsZero()
    {
        var orders = new List<Order>();
        var exposure = ExposureCalculator.CalculateExposure(orders);
        Assert.Equal(0m, exposure);
    }

    private static Order CreateOrder(decimal price, decimal quantity, OrderSide side)
    {
        var order = Order.Create("PETR4", quantity, price, side);
        return order;
    }
}