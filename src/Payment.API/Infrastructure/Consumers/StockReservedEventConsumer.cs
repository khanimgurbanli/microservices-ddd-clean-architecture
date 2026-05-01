using MassTransit;
using Payment.API.Application.Interfaces;
using Payment.API.Domain.Enums;
using Payment.API.Infrastructure.Transformations;
using Shared.Events;

namespace Payment.API.Infrastructure.Consumers;

public class StockReservedEventConsumer : IConsumer<StockReservedEvent>
{
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly IPaymentService _paymentService;
    private readonly ILogger<StockReservedEventConsumer> _logger;

    public StockReservedEventConsumer(IPublishEndpoint publishEndpoint, IPaymentService paymentService, ILogger<StockReservedEventConsumer> logger)
    {
        _publishEndpoint = publishEndpoint;
        _paymentService = paymentService;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<StockReservedEvent> context)
    {
        var aggregate = StockReservedEventMapper.ToAggregate(context.Message);
        var result = await _paymentService.ProcessAsync(aggregate);

        if (result.IsFailure)
        {
            var failed = new PaymentFailedEvent
            {
                OrderId = context.Message.OrderId,
                Message = result.Error.Message
            };

            await _publishEndpoint.Publish(failed);
            _logger.LogWarning("Payment failed event published for OrderId={OrderId}", context.Message.OrderId);
            return;
        }

        if (result.Value.Status == PaymentStatus.Failed)
        {
            var failed = new PaymentFailedEvent
            {
                OrderId = context.Message.OrderId,
                Message = "Payment failed."
            };

            await _publishEndpoint.Publish(failed);
            _logger.LogWarning("Payment failed event published for OrderId={OrderId}", context.Message.OrderId);
            return;
        }

        var completed = new PaymentCompletedEvent
        {
            OrderId = context.Message.OrderId
        };

        await _publishEndpoint.Publish(completed);
        _logger.LogInformation("Payment completed event published for OrderId={OrderId}", context.Message.OrderId);
    }
}
