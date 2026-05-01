using Order.API.Domain.Aggregates;
using Order.API.Domain.Enums;
using Order.API.Domain.Events;

namespace Order.API.Tests.Domain;

public class OrderAggregateTests
{
    [Fact]
    public void Create_ShouldCreateOrderWithSuspendedStatus()
    {
        var buyerId = Guid.NewGuid();

        var order = OrderAggregate.Create(buyerId);

        Assert.NotEqual(Guid.Empty, order.Id);
        Assert.Equal(buyerId, order.BuyerId);
        Assert.Equal(OrderStatus.Suspended, order.OrderStatus);
        Assert.Empty(order.Items);
        Assert.Single(order.DomainEvents);
    }

    [Fact]
    public void AddItem_ShouldAddItemAndRecalculateTotalPrice()
    {
        var order = OrderAggregate.Create(Guid.NewGuid());
        var productId = Guid.NewGuid();

        order.AddItem(productId, 2, 10.00m);

        Assert.Single(order.Items);
        Assert.Equal(20.00m, order.TotalPrice);
    }

    [Fact]
    public void AddItem_ToNonSuspendedOrder_ShouldThrowException()
    {
        var order = OrderAggregate.Create(Guid.NewGuid());
        order.UpdateStatus(OrderStatus.Completed);

        Assert.Throws<InvalidOperationException>(() =>
            order.AddItem(Guid.NewGuid(), 1, 10.00m));
    }

    [Fact]
    public void UpdateStatus_ShouldChangeStatusAndAddDomainEvent()
    {
        var order = OrderAggregate.Create(Guid.NewGuid());
        var initialEventsCount = order.DomainEvents.Count;

        order.UpdateStatus(OrderStatus.Completed);

        Assert.Equal(OrderStatus.Completed, order.OrderStatus);
        Assert.Equal(initialEventsCount + 1, order.DomainEvents.Count);
    }

    [Fact]
    public void ClearDomainEvents_ShouldRemoveAllEvents()
    {
        var order = OrderAggregate.Create(Guid.NewGuid());
        order.ClearDomainEvents();

        Assert.Empty(order.DomainEvents);
    }
}