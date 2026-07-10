using Moq;
using OrderGenerator.Application.UseCases;
using OrderGenerator.Domain.DTOs;
using OrderGenerator.Domain.Entities;
using OrderGenerator.Domain.Enums;
using OrderGenerator.Domain.Interfaces;

namespace OrderGenerator.Application.Tests;

public class PlaceOrderUseCaseTests
{
    private readonly Mock<IFixApplication> _fixApplicationMock;
    private readonly Mock<IOrderRepository> _orderRepositoryMock;
    private readonly PlaceOrderUseCase _useCase;

    public PlaceOrderUseCaseTests()
    {
        _fixApplicationMock = new Mock<IFixApplication>();
        _orderRepositoryMock = new Mock<IOrderRepository>();
        _useCase = new PlaceOrderUseCase(
            _fixApplicationMock.Object,
            _orderRepositoryMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_NullRequest_ThrowsArgumentNullException()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(
            () => _useCase.ExecuteAsync(null!));
    }

    [Fact]
    public async Task ExecuteAsync_OrderPersisted()
    {
        var order = Order.Create("PETR4", OrderSide.BUY, 10, 100m);
        _fixApplicationMock
            .Setup(x => x.SendOrder(It.IsAny<Order>()))
            .ReturnsAsync(order);

        var request = new PlaceOrderRequest
        {
            Ticker = "PETR4",
            Side = OrderSide.BUY,
            Quantity = 10,
            Price = 100m
        };

        await _useCase.ExecuteAsync(request);

        _orderRepositoryMock.Verify(
            x => x.AddAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_OrderUpdatedAfterExchange()
    {
        var order = Order.Create("PETR4", OrderSide.BUY, 10, 100m);
        order.Accept("EXCH-123");

        _fixApplicationMock
            .Setup(x => x.SendOrder(It.IsAny<Order>()))
            .ReturnsAsync(order);

        var request = new PlaceOrderRequest
        {
            Ticker = "PETR4",
            Side = OrderSide.BUY,
            Quantity = 10,
            Price = 100m
        };

        await _useCase.ExecuteAsync(request);

        _orderRepositoryMock.Verify(
            x => x.UpdateAsync(It.Is<Order>(o => o.Status == OrderStatus.Accepted),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_RejectedOrder_ReturnsRejectedResponse()
    {
        var order = Order.Create("PETR4", OrderSide.BUY, 10, 100m);
        order.Reject("Insufficient funds");

        _fixApplicationMock
            .Setup(x => x.SendOrder(It.IsAny<Order>()))
            .ReturnsAsync(order);

        var request = new PlaceOrderRequest
        {
            Ticker = "PETR4",
            Side = OrderSide.BUY,
            Quantity = 10,
            Price = 100m
        };

        var response = await _useCase.ExecuteAsync(request);

        Assert.Equal(OrderStatus.Rejected, response.Status);
        Assert.Equal("Insufficient funds", response.RejectReason);
    }
}