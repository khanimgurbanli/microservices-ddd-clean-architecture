using Moq;
using Xunit;
using Order.API.Application.Commands.Order;
using Order.API.Application.Commands.Order.Models;
using Order.API.Application.Models.Orders;
using Order.API.Application.Queries.Order;
using Order.API.Application.Interfaces;
using CSharpFunctionalExtensions;
using Order.API.Domain.Aggregates;

namespace Order.API.Tests.Application;

public class CreateOrderCommandHandlerTests
{
    private readonly Mock<IOrderService> _orderServiceMock;

    public CreateOrderCommandHandlerTests()
    {
        _orderServiceMock = new Mock<IOrderService>();
    }

    [Fact]
    public async Task Handle_ValidCommand_ShouldReturnSuccessResult()
    {
        var buyerId = Guid.NewGuid();
        var orderItems = new List<CreateOrderItemRequest>
        {
            new() { ProductId = Guid.NewGuid(), Count = 2, Price = 10.00m }
        };

        var aggregate = OrderAggregate.Create(buyerId);
        aggregate.AddItem(orderItems[0].ProductId, orderItems[0].Count, orderItems[0].Price);

        _orderServiceMock
            .Setup(x => x.CreateAsync(It.IsAny<OrderAggregate>()))
            .ReturnsAsync(Result.Success<OrderAggregate, Shared.Errors.DomainError>(aggregate));

        var handler = new CreateOrderCommandHandler(_orderServiceMock.Object);
        var command = new CreateOrderCommand { BuyerId = buyerId, OrderItems = orderItems };

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(buyerId, result.Value.BuyerId);
        Assert.Equal(20.00m, result.Value.TotalPrice);
    }
}

public class GetAllOrdersQueryHandlerTests
{
    private readonly Mock<IOrderService> _orderServiceMock;

    public GetAllOrdersQueryHandlerTests()
    {
        _orderServiceMock = new Mock<IOrderService>();
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccessResult()
    {
        var order1 = OrderAggregate.Create(Guid.NewGuid());
        order1.AddItem(Guid.NewGuid(), 1, 100.00m);

        var order2 = OrderAggregate.Create(Guid.NewGuid());
        order2.AddItem(Guid.NewGuid(), 1, 200.00m);

        var orders = new List<OrderAggregate>
        {
            order1,
            order2
        };

        _orderServiceMock
            .Setup(x => x.GetAllAsync())
            .ReturnsAsync(Result.Success<IEnumerable<OrderAggregate>, Shared.Errors.DomainError>(orders));

        var handler = new GetAllOrdersQueryHandler(_orderServiceMock.Object);
        var query = new GetAllOrdersQuery();

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(2, result.Value.Count());
    }
}
