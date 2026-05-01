using MongoDB.Bson.Serialization.Attributes;

namespace Stock.API.Infrastructure.Database;

public class StockDocument
{
    [BsonId]
    [BsonGuidRepresentation(MongoDB.Bson.GuidRepresentation.CSharpLegacy)]
    public Guid Id { get; set; }

    [BsonGuidRepresentation(MongoDB.Bson.GuidRepresentation.CSharpLegacy)]
    public Guid ProductId { get; set; }

    [BsonRepresentation(MongoDB.Bson.BsonType.Int64)]
    public int Count { get; set; }
}
