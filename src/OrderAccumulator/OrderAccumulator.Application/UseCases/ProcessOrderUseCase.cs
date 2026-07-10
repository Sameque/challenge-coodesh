using Microsoft.Extensions.Logging;
using OrderAccumulator.Application.DTOs;
using OrderAccumulator.Domain.Entities;
using OrderAccumulator.Domain.Enums;
using OrderAccumulator.Domain.Interfaces;
using OrderAccumulator.Domain.Services;

namespace OrderAccumulator.Application.UseCases;

public class ProcessOrderUseCase
{
    private readonly IOrderRepository _repository;
    private readonly IExposureRepository _exposureRepository;
    private readonly ILogger<ProcessOrderUseCase> _logger;

    public ProcessOrderUseCase(
        IOrderRepository repository,
        IExposureRepository exposureRepository,
        ILogger<ProcessOrderUseCase> logger)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _exposureRepository = exposureRepository ?? throw new ArgumentNullException(nameof(exposureRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<OrderResponse> ExecuteAsync(OrderRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Symbol) || request.Quantity <= 0 || request.Price <= 0)
            return new OrderResponse(false, "Invalid order parameters.");

        if (request.Price > OrderLimits.MaxPrice)
            return new OrderResponse(false, $"Price exceeds the maximum allowed value of {OrderLimits.MaxPrice}.");

        if (request.Quantity > OrderLimits.MaxQuantity)
            return new OrderResponse(false, $"Quantity exceeds the maximum allowed value of {OrderLimits.MaxQuantity}.");

        var side = request.Side.ToLower() == "buy" ? OrderSide.Buy : OrderSide.Sell;

        var symbol = request.Symbol.ToUpper();
        var delta = ExposureCalculator.CalculateDelta(side, request.Price, request.Quantity);
        var currentExposure = await _exposureRepository.GetExposureAsync(symbol, cancellationToken);

        if (currentExposure + delta > OrderLimits.MaxExposure)
        {
            return new OrderResponse(false, $"Order rejected: Total exposure for {symbol} would exceed the maximum allowed limit of 100,000,000.00.");
        }

        var order = Order.Create(symbol, request.Quantity, request.Price, side);

        await _repository.AddOrderAsync(order, cancellationToken);

        try
        {
            await _exposureRepository.UpdateExposureAsync(symbol, delta, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to update exposure for {Symbol}. Order {OrderId} saved but exposure may be stale.",
                symbol, order.Id);
        }

        return new OrderResponse(true, "Order accepted and processed.");
    }
}
