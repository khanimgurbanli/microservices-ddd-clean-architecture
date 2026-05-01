using MassTransit;
using Order.API.Application.Interfaces;
using Order.API.Domain.Enums;
using Order.API.Domain.ValueObjects;
using Shared.Events;

namespace Order.API.Infrastructure.Consumers;

public class PaymentCompletedEventConsumer : IConsumer<PaymentCompletedEvent>
{
    private readonly IOrderService _orderService;

    public PaymentCompletedEventConsumer(IOrderService orderService)
    {
        _orderService = orderService;
    }

    public async Task Consume(ConsumeContext<PaymentCompletedEvent> context)
    {
        await _orderService.UpdateStatusAsync(OrderId.From(context.Message.OrderId), OrderStatus.Completed);
    }
}
