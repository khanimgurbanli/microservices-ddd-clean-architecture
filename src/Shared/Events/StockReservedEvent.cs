using Shared.Events.Common;

namespace Shared.Events;

public class StockReservedEvent : IEvent
{
    public Guid OrderId { get; set; }
    public Guid BuyerId { get; set; }
    public decimal TotalPrice { get; set; }
}
