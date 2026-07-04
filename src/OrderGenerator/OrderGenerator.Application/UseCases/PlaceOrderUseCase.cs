using OrderGenerator.Application.DTOs;
using OrderGenerator.Domain.Entities;
using OrderGenerator.Domain.Interfaces;

namespace OrderGenerator.Application.UseCases;

/// <summary>
/// Orchestrates the placement of a new order:
/// validates the symbol via the Exchange API,
/// sends it to the FIX exchange, and returns the result.
/// </summary>
public sealed class PlaceOrderUseCase
{
    private readonly IExchangeApiClient _exchangeApiClient;
    private readonly IExchangeService _exchangeService;

    public PlaceOrderUseCase(
        IExchangeApiClient exchangeApiClient,
        IExchangeService exchangeService)
    {
        _exchangeApiClient = exchangeApiClient ??
            throw new ArgumentNullException(nameof(exchangeApiClient));
        _exchangeService = exchangeService ??
            throw new ArgumentNullException(nameof(exchangeService));
    }

    public async Task<PlaceOrderResponse> ExecuteAsync(
                        PlaceOrderRequest request,
                        CancellationToken cancellationToken = default)
    {
        bool symbolExists = await _exchangeApiClient.SymbolExistsAsync(request.Symbol, cancellationToken);
        if (!symbolExists)
            throw new InvalidOperationException($"Symbol '{request.Symbol}' is not available for trading.");

        var order = Order.Create(request.Symbol, request.Side, request.Quantity, request.Price);

        var updatedOrder = await _exchangeService.SendOrderAsync(order, cancellationToken);

        return MapToResponse(updatedOrder);
    }

    private static PlaceOrderResponse MapToResponse(Order order) =>
        new(
            order.Id,
            order.Symbol,
            order.Side,
            order.Quantity,
            order.Price,
            order.Status,
            order.ExchangeOrderId,
            order.RejectReason,
            order.CreatedAt
        );
}
