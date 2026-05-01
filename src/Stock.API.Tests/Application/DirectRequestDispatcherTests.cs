using CSharpFunctionalExtensions;
using Moq;
using Shared.Errors;
using Stock.API.Application.Commands.Stock;
using Stock.API.Application.Interfaces;
using Stock.API.Application.Models.Stock;
using Stock.API.Application.Queries.Stock;
using Stock.API.Domain.Aggregates;
using Stock.API.Infrastructure.Dispatching;
using Xunit;

namespace Stock.API.Tests.Application;

public class DirectRequestDispatcherTests
{
    [Fact]
    public async Task Send_CreateStockCommand_ShouldReturnStockResponse()
    {
        var stockServiceMock = new Mock<IStockService>();

        var productId = Guid.NewGuid();
        var command = new CreateStockCommand(productId, 100);

        var aggregate = StockAggregate.Create(productId, 100);

        stockServiceMock
            .Setup(x => x.CreateAsync(It.IsAny<StockAggregate>()))
            .ReturnsAsync(Result.Success<StockAggregate, DomainError>(aggregate));

        var dispatcher = new DirectRequestDispatcher(stockServiceMock.Object);

        var result = await dispatcher.Send<Result<StockResponse, DomainError>>(command);

        Assert.True(result.IsSuccess);
        Assert.Equal(productId, result.Value.ProductId);
        Assert.Equal(100, result.Value.Count);
    }

    [Fact]
    public async Task Send_GetAllStocksQuery_ShouldReturnResponses()
    {
        var stockServiceMock = new Mock<IStockService>();

        var s1 = StockAggregate.Create(Guid.NewGuid(), 10);
        var s2 = StockAggregate.Create(Guid.NewGuid(), 20);

        stockServiceMock
            .Setup(x => x.GetAllAsync())
            .ReturnsAsync(Result.Success<IEnumerable<StockAggregate>, DomainError>(new[] { s1, s2 }));

        var dispatcher = new DirectRequestDispatcher(stockServiceMock.Object);

        var result = await dispatcher.Send<Result<IEnumerable<StockResponse>, DomainError>>(new GetAllStocksQuery());

        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value.Count());
    }
}

