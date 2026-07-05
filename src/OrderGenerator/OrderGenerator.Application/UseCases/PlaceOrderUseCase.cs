using OrderGenerator.Domain.DTOs;
using OrderGenerator.Domain.Entities;
using OrderGenerator.Domain.Interfaces;
using OrderGenerator.Infrastructure.Exchange;

namespace OrderGenerator.Application.UseCases;

/// <summary>
/// Orchestrates the placement of a new order:
/// persists the initial state, sends it to the FIX exchange,
/// and updates the state based on the exchange response.
/// </summary>
public sealed class PlaceOrderUseCase
{
    private readonly FixApplication _fixApplication;
    private readonly IOrderRepository _orderRepository;

    public PlaceOrderUseCase(
                FixApplication fixApplication, 
                IOrderRepository orderRepository)
    {
        _fixApplication = fixApplication
                            ?? throw new ArgumentNullException(nameof(fixApplication));
        _orderRepository = orderRepository
                            ?? throw new ArgumentNullException(nameof(orderRepository));
    }

    public async Task<PlaceOrderResponse> ExecuteAsync(
                        PlaceOrderRequest request,
                        CancellationToken cancellationToken = default)
    {
        var order = Order.Create(request.Ticker, request.Side, request.Quantity, request.Price);

        await _orderRepository.AddAsync(order, cancellationToken);

        var updatedOrder = await _fixApplication.SendOrder(order);

        await _orderRepository.UpdateAsync(updatedOrder, cancellationToken);

        return MapToResponse(updatedOrder);
    }

    private static PlaceOrderResponse MapToResponse(Order order) =>
        new(
            order.Id,
            order.Ticker,
            order.Side,
            order.Quantity,
            order.Price,
            order.Status,
            order.ExchangeOrderId,
            order.RejectReason,
            order.CreatedAt
        );
}
