using MongoDB.Driver;
using Stock.API.Infrastructure.Database;

namespace Stock.API.Infrastructure.Database;

public class MongoDbContext
{
    private readonly IMongoDatabase _database;

    public MongoDbContext(IConfiguration configuration)
    {
        var settings = MongoClientSettings.FromConnectionString(configuration.GetConnectionString("MongoDB"));
        settings.ServerSelectionTimeout = TimeSpan.FromSeconds(3);
        settings.ConnectTimeout = TimeSpan.FromSeconds(3);
        settings.SocketTimeout = TimeSpan.FromSeconds(3);

        var mongoClient = new MongoClient(settings);
        _database = mongoClient.GetDatabase("StockDB");
    }

    public IMongoCollection<StockDocument> Stocks => _database.GetCollection<StockDocument>("stocks");
}
