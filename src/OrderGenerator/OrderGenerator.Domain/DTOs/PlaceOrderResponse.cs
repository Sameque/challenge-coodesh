using OrderGenerator.Domain.Enums;

namespace OrderGenerator.Domain.DTOs;

/// <summary>
/// Response returned to the client after order submission.
/// </summary>
public sealed record PlaceOrderResponse(
    Guid OrderId,
    string Symbol,
    OrderSide Side,
    int Quantity,
    decimal Price,
    OrderStatus Status,
    string? BrokerOrderId,
    string? RejectReason,
    DateTime CreatedAt
);
