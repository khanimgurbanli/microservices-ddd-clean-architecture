using CSharpFunctionalExtensions;
using Moq;
using Order.API.Application.Commands.Order;
using Order.API.Application.Commands.Order.Models;
using Order.API.Application.Interfaces;
using Order.API.Application.Models.Orders;
using Order.API.Application.Queries.Order;
using Order.API.Domain.Aggregates;
using Order.API.Infrastructure.Dispatching;
using Shared.Errors;
using Xunit;

namespace Order.API.Tests.Application;

public class DirectRequestDispatcherTests
{
    [Fact]
    public async Task Send_CreateOrderCommand_ShouldReturnOrderResponse()
    {
        var orderServiceMock = new Mock<IOrderService>();

        var buyerId = Guid.NewGuid();
        var command = new CreateOrderCommand
        {
            BuyerId = buyerId,
            OrderItems = new List<CreateOrderItemRequest>
            {
                new() { ProductId = Guid.NewGuid(), Count = 2, Price = 10m }
            }
        };

        var aggregate = OrderAggregate.Create(buyerId);
        aggregate.AddItem(command.OrderItems[0].ProductId, command.OrderItems[0].Count, command.OrderItems[0].Price);

        orderServiceMock
            .Setup(x => x.CreateAsync(It.IsAny<OrderAggregate>()))
            .ReturnsAsync(Result.Success<OrderAggregate, DomainError>(aggregate));

        var dispatcher = new DirectRequestDispatcher(orderServiceMock.Object);

        var result = await dispatcher.Send<Result<OrderResponse, DomainError>>(command);

        Assert.True(result.IsSuccess);
        Assert.Equal(buyerId, result.Value.BuyerId);
        Assert.Equal(20m, result.Value.TotalPrice);
    }

    [Fact]
    public async Task Send_GetAllOrdersQuery_ShouldReturnResponses()
    {
        var orderServiceMock = new Mock<IOrderService>();

        var order1 = OrderAggregate.Create(Guid.NewGuid());
        order1.AddItem(Guid.NewGuid(), 1, 100m);

        var order2 = OrderAggregate.Create(Guid.NewGuid());
        order2.AddItem(Guid.NewGuid(), 1, 200m);

        orderServiceMock
            .Setup(x => x.GetAllAsync())
            .ReturnsAsync(Result.Success<IEnumerable<OrderAggregate>, DomainError>(new[] { order1, order2 }));

        var dispatcher = new DirectRequestDispatcher(orderServiceMock.Object);

        var result = await dispatcher.Send<Result<IEnumerable<OrderResponse>, DomainError>>(new GetAllOrdersQuery());

        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value.Count());
    }
}
