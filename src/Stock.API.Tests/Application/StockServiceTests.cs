using Moq;
using Xunit;
using Stock.API.Infrastructure.Services;
using Stock.API.Domain.Aggregates;
using Stock.API.Domain.Interfaces;
using CSharpFunctionalExtensions;
using Stock.API.Domain.ValueObjects;

namespace Stock.API.Tests.Application;

public class StockServiceTests
{
    private readonly Mock<IStockRepository> _stockRepositoryMock;
    private readonly StockService _stockService;

    public StockServiceTests()
    {
        _stockRepositoryMock = new Mock<IStockRepository>();
        _stockService = new StockService(_stockRepositoryMock.Object);
    }

    [Fact]
    public async Task CreateAsync_ValidAggregate_ShouldCreateStock()
    {
        var productId = Guid.NewGuid();
        const int count = 100;
        var aggregate = StockAggregate.Create(productId, count);

        _stockRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<StockAggregate>()))
            .Returns(Task.CompletedTask);

        var result = await _stockService.CreateAsync(aggregate);

        Assert.True(result.IsSuccess);
        Assert.True(result.Value != null);
        Assert.Equal(productId, result.Value.ProductId);
        Assert.Equal(count, result.Value.Count);

        _stockRepositoryMock.Verify(x => x.AddAsync(It.IsAny<StockAggregate>()), Times.Once);
    }

    [Fact]
    public async Task GetByProductIdAsync_ExistingStock_ShouldReturnStock()
    {
        var productId = Guid.NewGuid();
        var stock = StockAggregate.Load(Guid.NewGuid(), productId, 50);

        _stockRepositoryMock
            .Setup(x => x.GetByProductIdAsync(ProductId.From(productId)))
            .ReturnsAsync(stock);

        var result = await _stockService.GetByProductIdAsync(ProductId.From(productId));

        Assert.True(result.IsSuccess);
        Assert.True(result.Value != null);
        Assert.Equal(productId, result.Value.ProductId);
        Assert.Equal(50, result.Value.Count);
    }

    [Fact]
    public async Task GetByProductIdAsync_NonExistingStock_ShouldReturnFailure()
    {
        var productId = Guid.NewGuid();

        _stockRepositoryMock
            .Setup(x => x.GetByProductIdAsync(ProductId.From(productId)))
            .ReturnsAsync((StockAggregate?)null);

        var result = await _stockService.GetByProductIdAsync(ProductId.From(productId));

        Assert.True(result.IsFailure);
        Assert.Contains("not found", result.Error.Message);
    }

    [Fact]
    public async Task ReserveAsync_SufficientStock_ShouldReturnSuccess()
    {
        var productId = Guid.NewGuid();
        var stock = StockAggregate.Load(Guid.NewGuid(), productId, 100);

        _stockRepositoryMock
            .Setup(x => x.GetByProductIdAsync(ProductId.From(productId)))
            .ReturnsAsync(stock);

        _stockRepositoryMock
            .Setup(x => x.UpdateAsync(It.IsAny<StockAggregate>()))
            .Returns(Task.CompletedTask);

        var result = await _stockService.ReserveAsync(ProductId.From(productId), Quantity.From(10));

        Assert.True(result.IsSuccess);
        Assert.Equal(90, result.Value.Count);
        _stockRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<StockAggregate>()), Times.Once);
    }

    [Fact]
    public async Task ReserveAsync_InsufficientStock_ShouldReturnFailure()
    {
        var productId = Guid.NewGuid();
        var stock = StockAggregate.Load(Guid.NewGuid(), productId, 5);

        _stockRepositoryMock
            .Setup(x => x.GetByProductIdAsync(ProductId.From(productId)))
            .ReturnsAsync(stock);

        var result = await _stockService.ReserveAsync(ProductId.From(productId), Quantity.From(10));

        Assert.True(result.IsFailure);
        Assert.Contains("Insufficient stock", result.Error.Message);
        _stockRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<StockAggregate>()), Times.Never);
    }

    [Fact]
    public async Task ReserveForOrderAsync_AllAvailable_ShouldUpdateAllAndReturnSuccess()
    {
        var productId1 = Guid.NewGuid();
        var productId2 = Guid.NewGuid();

        var stock1 = StockAggregate.Load(Guid.NewGuid(), productId1, 100);
        var stock2 = StockAggregate.Load(Guid.NewGuid(), productId2, 200);

        _stockRepositoryMock
            .Setup(x => x.GetByProductIdAsync(ProductId.From(productId1)))
            .ReturnsAsync(stock1);

        _stockRepositoryMock
            .Setup(x => x.GetByProductIdAsync(ProductId.From(productId2)))
            .ReturnsAsync(stock2);

        _stockRepositoryMock
            .Setup(x => x.UpdateAsync(It.IsAny<StockAggregate>()))
            .Returns(Task.CompletedTask);

        var items = new[]
        {
            new StockReservationItem(ProductId.From(productId1), Quantity.From(10)),
            new StockReservationItem(ProductId.From(productId2), Quantity.From(20))
        };

        var result = await _stockService.ReserveForOrderAsync(items);

        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value.Count());
        Assert.Equal(90, stock1.Count);
        Assert.Equal(180, stock2.Count);
        _stockRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<StockAggregate>()), Times.Exactly(2));
    }

    [Fact]
    public async Task ReserveForOrderAsync_Insufficient_ShouldReturnFailureAndNotUpdate()
    {
        var productId1 = Guid.NewGuid();
        var productId2 = Guid.NewGuid();

        var stock1 = StockAggregate.Load(Guid.NewGuid(), productId1, 5);
        var stock2 = StockAggregate.Load(Guid.NewGuid(), productId2, 200);

        _stockRepositoryMock
            .Setup(x => x.GetByProductIdAsync(ProductId.From(productId1)))
            .ReturnsAsync(stock1);

        _stockRepositoryMock
            .Setup(x => x.GetByProductIdAsync(ProductId.From(productId2)))
            .ReturnsAsync(stock2);

        var items = new[]
        {
            new StockReservationItem(ProductId.From(productId1), Quantity.From(10)),
            new StockReservationItem(ProductId.From(productId2), Quantity.From(20))
        };

        var result = await _stockService.ReserveForOrderAsync(items);

        Assert.True(result.IsFailure);
        _stockRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<StockAggregate>()), Times.Never);
    }
}
