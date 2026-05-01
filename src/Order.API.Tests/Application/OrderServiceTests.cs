using Moq;
using Xunit;
using Order.API.Infrastructure.Services;
using Order.API.Domain.Aggregates;
using Order.API.Domain.Enums;
using Order.API.Domain.Interfaces;
using Order.API.Domain.ValueObjects;
using CSharpFunctionalExtensions;

namespace Order.API.Tests.Application;

public class OrderServiceTests
{
    private readonly Mock<IOrderRepository> _orderRepositoryMock;
    private readonly Mock<IDomainEventDispatcher> _eventDispatcherMock;
    private readonly OrderService _orderService;

    public OrderServiceTests()
    {
        _orderRepositoryMock = new Mock<IOrderRepository>();
        _eventDispatcherMock = new Mock<IDomainEventDispatcher>();
        _orderService = new OrderService(_orderRepositoryMock.Object, _eventDispatcherMock.Object);
    }

    [Fact]
    public async Task CreateAsync_ValidAggregate_ShouldPersistAndDispatchEvents()
    {
        var buyerId = Guid.NewGuid();
        var order = OrderAggregate.Create(buyerId);
        order.AddItem(Guid.NewGuid(), 2, 10.00m);

        _orderRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<OrderAggregate>()))
            .Returns(Task.CompletedTask);

        var result = await _orderService.CreateAsync(order);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(buyerId, result.Value.BuyerId);
        Assert.Equal(20.00m, result.Value.TotalPrice);

        _orderRepositoryMock.Verify(x => x.AddAsync(It.IsAny<OrderAggregate>()), Times.Once);
        _eventDispatcherMock.Verify(x => x.DispatchAsync(It.IsAny<IEnumerable<Order.API.Domain.Events.DomainEvent>>()), Times.Once);
    }

    [Fact]
    public async Task UpdateStatusAsync_ExistingOrder_ShouldUpdateAndDispatchEvents()
    {
        var orderId = Guid.NewGuid();
        var order = OrderAggregate.Create(Guid.NewGuid());

        _orderRepositoryMock
            .Setup(x => x.GetByIdAsync(OrderId.From(orderId)))
            .ReturnsAsync(order);

        _orderRepositoryMock
            .Setup(x => x.UpdateAsync(It.IsAny<OrderAggregate>()))
            .Returns(Task.CompletedTask);

        var result = await _orderService.UpdateStatusAsync(OrderId.From(orderId), OrderStatus.Completed);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);

        _orderRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<OrderAggregate>()), Times.Once);
        _eventDispatcherMock.Verify(x => x.DispatchAsync(It.IsAny<IEnumerable<Order.API.Domain.Events.DomainEvent>>()), Times.AtLeastOnce);
    }

    [Fact]
    public async Task DeleteAsync_ShouldCallRepository()
    {
        var orderId = Guid.NewGuid();

        var result = await _orderService.DeleteAsync(OrderId.From(orderId));

        Assert.True(result.IsSuccess);
        Assert.Equal(orderId, result.Value.Value);
        _orderRepositoryMock.Verify(x => x.DeleteAsync(OrderId.From(orderId)), Times.Once);
    }
}
