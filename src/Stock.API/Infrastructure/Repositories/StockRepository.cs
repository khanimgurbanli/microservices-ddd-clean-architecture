using MongoDB.Driver;
using Stock.API.Domain.Aggregates;
using Stock.API.Domain.Interfaces;
using Stock.API.Domain.ValueObjects;
using Stock.API.Infrastructure.Database;

namespace Stock.API.Infrastructure.Repositories;

public class StockRepository : IStockRepository
{
    private readonly MongoDbContext _context;

    public StockRepository(MongoDbContext context)
    {
        _context = context;
    }

    public async Task<StockAggregate?> GetByIdAsync(StockId id)
    {
        var document = await _context.Stocks
            .Find(s => s.Id == id.Value)
            .FirstOrDefaultAsync();

        return document == null ? null : MapToAggregate(document);
    }

    public async Task<StockAggregate?> GetByProductIdAsync(ProductId productId)
    {
        var document = await _context.Stocks
            .Find(s => s.ProductId == productId.Value)
            .FirstOrDefaultAsync();

        return document == null ? null : MapToAggregate(document);
    }

    public async Task<IEnumerable<StockAggregate>> GetAllAsync()
    {
        var documents = await _context.Stocks
            .Find(_ => true)
            .ToListAsync();

        return documents.Select(MapToAggregate);
    }

    public async Task AddAsync(StockAggregate stock)
    {
        var document = new StockDocument
        {
            Id = stock.Id,
            ProductId = stock.ProductId,
            Count = stock.Count
        };

        await _context.Stocks.InsertOneAsync(document);
    }

    public async Task UpdateAsync(StockAggregate stock)
    {
        var document = new StockDocument
        {
            Id = stock.Id,
            ProductId = stock.ProductId,
            Count = stock.Count
        };

        await _context.Stocks.ReplaceOneAsync(s => s.Id == stock.Id, document);
    }

    public async Task DeleteAsync(StockId id)
    {
        await _context.Stocks.DeleteOneAsync(s => s.Id == id.Value);
    }

    private static StockAggregate MapToAggregate(StockDocument document)
    {
        return StockAggregate.Load(document.Id, document.ProductId, document.Count);
    }
}
