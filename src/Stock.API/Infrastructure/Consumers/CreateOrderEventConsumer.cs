using MassTransit;
using Shared.Events;
using Stock.API.Application.Interfaces;
using Stock.API.Infrastructure.Transformations;

namespace Stock.API.Infrastructure.Consumers;

public class CreateOrderEventConsumer : IConsumer<CreateOrderEvent>
{
    private readonly ISendEndpointProvider _bus;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly IStockService _stockService;

    public CreateOrderEventConsumer(IStockService stockService, ISendEndpointProvider bus, IPublishEndpoint publishEndpoint)
    {
        _stockService = stockService;
        _bus = bus;
        _publishEndpoint = publishEndpoint;
    }

    public async Task Consume(ConsumeContext<CreateOrderEvent> context)
    {
        var items = CreateOrderEventMapper.MapToReservationItems(context.Message).ToList();
        var reserveResult = await _stockService.ReserveForOrderAsync(items);

        if (reserveResult.IsSuccess)
        {
            var stockReservedEvent = new StockReservedEvent
            {
                OrderId = context.Message.OrderId,
                BuyerId = context.Message.BuyerId,
                TotalPrice = context.Message.TotalPrice,
            };

            var sendEndpoint = await _bus.GetSendEndpoint(new Uri($"queue:{Shared.RabbitMQSettings.Payment_StockReservedEventQueue}"));
            await sendEndpoint.Send(stockReservedEvent);
        }
        else
        {
            var stockNotReservedEvent = new StockNotReservedEvent
            {
                OrderId = context.Message.OrderId,
                BuyerId = context.Message.BuyerId,
                Message = "Insufficient stock",
            };

            await _publishEndpoint.Publish(stockNotReservedEvent);
        }
    }
}
