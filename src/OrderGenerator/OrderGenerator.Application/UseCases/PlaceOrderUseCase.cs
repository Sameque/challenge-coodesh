using OrderGenerator.Domain.DTOs;
using OrderGenerator.Domain.Entities;
using OrderGenerator.Infrastructure.Exchange;

namespace OrderGenerator.Application.UseCases;

/// <summary>
/// Orchestrates the placement of a new order:
/// validates the symbol via the Exchange API,
/// sends it to the FIX exchange, and returns the result.
/// </summary>
public sealed class PlaceOrderUseCase
{
    private readonly FixApplication _fixApplication;

    public PlaceOrderUseCase(FixApplication fixApplication)
    {
        _fixApplication = fixApplication 
                            ?? throw new ArgumentNullException(nameof(fixApplication));
    }

    public async Task<PlaceOrderResponse> ExecuteAsync(
                        PlaceOrderRequest request,
                        CancellationToken cancellationToken = default)
    {
        var order = Order.Create(request.Ticker, request.Side, request.Quantity, request.Price);

        //TODO: inserir ordem no banco de dados

        var updatedOrder = await _fixApplication.SendOrder(order);

        //TODO atualizar ordem

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
