using MassTransit;
using Order.API.Application.Interfaces;
using Order.API.Domain.Enums;
using Order.API.Domain.ValueObjects;
using Shared.Events;

namespace Order.API.Infrastructure.Consumers;

public class StockNotReservedEventConsumer : IConsumer<StockNotReservedEvent>
{
    private readonly IOrderService _orderService;

    public StockNotReservedEventConsumer(IOrderService orderService)
    {
        _orderService = orderService;
    }

    public async Task Consume(ConsumeContext<StockNotReservedEvent> context)
    {
        await _orderService.UpdateStatusAsync(OrderId.From(context.Message.OrderId), OrderStatus.Failed);
    }
}
