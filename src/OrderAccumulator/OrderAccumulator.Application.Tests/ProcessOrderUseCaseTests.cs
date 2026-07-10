using Microsoft.Extensions.Logging;
using Moq;
using OrderAccumulator.Application.DTOs;
using OrderAccumulator.Application.UseCases;
using OrderAccumulator.Domain.Enums;
using OrderAccumulator.Domain.Interfaces;

namespace OrderAccumulator.Application.Tests;

public class ProcessOrderUseCaseTests
{
    private readonly Mock<IOrderRepository> _orderRepositoryMock;
    private readonly Mock<IExposureRepository> _exposureRepositoryMock;
    private readonly Mock<ILogger<ProcessOrderUseCase>> _loggerMock;
    private readonly ProcessOrderUseCase _useCase;

    public ProcessOrderUseCaseTests()
    {
        _orderRepositoryMock = new Mock<IOrderRepository>();
        _exposureRepositoryMock = new Mock<IExposureRepository>();
        _loggerMock = new Mock<ILogger<ProcessOrderUseCase>>();
        _useCase = new ProcessOrderUseCase(
            _orderRepositoryMock.Object,
            _exposureRepositoryMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_InvalidParameters_ReturnsRejected()
    {
        var request = new OrderRequest("", 0, 0, "buy");
        var result = await _useCase.ExecuteAsync(request);
        Assert.False(result.Accepted);
    }

    [Fact]
    public async Task ExecuteAsync_PriceExceedsMax_ReturnsRejected()
    {
        var request = new OrderRequest("PETR4", 10, 1500m, "buy");
        var result = await _useCase.ExecuteAsync(request);
        Assert.False(result.Accepted);
    }

    [Fact]
    public async Task ExecuteAsync_QuantityExceedsMax_ReturnsRejected()
    {
        var request = new OrderRequest("PETR4", 200000, 100m, "buy");
        var result = await _useCase.ExecuteAsync(request);
        Assert.False(result.Accepted);
    }

    [Fact]
    public async Task ExecuteAsync_ExposureExceedsMax_ReturnsRejected()
    {
        _exposureRepositoryMock
            .Setup(x => x.GetExposureAsync("PETR4", It.IsAny<CancellationToken>()))
            .ReturnsAsync(99_999_999m);

        var request = new OrderRequest("PETR4", 100, 100m, "buy");
        var result = await _useCase.ExecuteAsync(request);
        Assert.False(result.Accepted);
    }

    [Fact]
    public async Task ExecuteAsync_ValidOrder_ReturnsAccepted()
    {
        _exposureRepositoryMock
            .Setup(x => x.GetExposureAsync("PETR4", It.IsAny<CancellationToken>()))
            .ReturnsAsync(0m);

        var request = new OrderRequest("PETR4", 10, 100m, "buy");
        var result = await _useCase.ExecuteAsync(request);
        Assert.True(result.Accepted);
    }

    [Fact]
    public async Task ExecuteAsync_ValidOrder_PersistsOrder()
    {
        _exposureRepositoryMock
            .Setup(x => x.GetExposureAsync("PETR4", It.IsAny<CancellationToken>()))
            .ReturnsAsync(0m);

        var request = new OrderRequest("PETR4", 10, 100m, "buy");
        await _useCase.ExecuteAsync(request);

        _orderRepositoryMock.Verify(
            x => x.AddOrderAsync(It.IsAny<Domain.Entities.Order>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_ValidOrder_UpdatesExposure()
    {
        _exposureRepositoryMock
            .Setup(x => x.GetExposureAsync("PETR4", It.IsAny<CancellationToken>()))
            .ReturnsAsync(0m);

        var request = new OrderRequest("PETR4", 10, 100m, "buy");
        await _useCase.ExecuteAsync(request);

        _exposureRepositoryMock.Verify(
            x => x.UpdateExposureAsync("PETR4", 1000m, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_SellOrder_NegativeDelta()
    {
        _exposureRepositoryMock
            .Setup(x => x.GetExposureAsync("PETR4", It.IsAny<CancellationToken>()))
            .ReturnsAsync(5000m);

        var request = new OrderRequest("PETR4", 10, 100m, "sell");
        await _useCase.ExecuteAsync(request);

        _exposureRepositoryMock.Verify(
            x => x.UpdateExposureAsync("PETR4", -1000m, It.IsAny<CancellationToken>()),
            Times.Once);
    }
}