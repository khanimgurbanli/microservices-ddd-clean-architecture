namespace Stock.API.Domain.ValueObjects;

public sealed record StockReservationItem(ProductId ProductId, Quantity Quantity);
