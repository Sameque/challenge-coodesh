using OrderAccumulator.Domain.Entities;
using OrderAccumulator.Domain.Enums;

namespace OrderAccumulator.Domain.Services;

public static class ExposureCalculator
{
    public static decimal CalculateExposure(IEnumerable<Order> orders)
    {
        return orders.Sum(o => o.Side == OrderSide.Buy
            ? o.Price * o.Quantity
            : -o.Price * o.Quantity);
    }

    public static decimal CalculateExposure(Order order)
    {
        return CalculateExposure(new[] { order });
    }

    public static decimal CalculateDelta(OrderSide side, decimal price, decimal quantity)
    {
        var orderValue = price * quantity;
        return side == OrderSide.Buy ? orderValue : -orderValue;
    }
}