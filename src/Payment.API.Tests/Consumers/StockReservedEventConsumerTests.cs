using MassTransit;
using Moq;
using Microsoft.Extensions.Logging;
using Payment.API.Application.Interfaces;
using Payment.API.Domain.Aggregates;
using Payment.API.Infrastructure.Consumers;
using Shared.Events;
using Xunit;

namespace Payment.API.Tests.Consumers;

public class StockReservedEventConsumerTests
{
    [Fact]
    public async Task Consume_WhenPaymentSucceeds_ShouldPublishPaymentCompletedEvent()
    {
        var publishEndpoint = new Mock<IPublishEndpoint>();
        var paymentService = new Mock<IPaymentService>();
        var logger = new Mock<ILogger<StockReservedEventConsumer>>();

        var message = new StockReservedEvent
        {
            OrderId = Guid.NewGuid(),
            BuyerId = Guid.NewGuid(),
            TotalPrice = 10m
        };

        var context = new Mock<ConsumeContext<StockReservedEvent>>();
        context.SetupGet(x => x.Message).Returns(message);

        paymentService
            .Setup(x => x.ProcessAsync(It.IsAny<PaymentAggregate>()))
            .ReturnsAsync(CSharpFunctionalExtensions.Result.Success<PaymentAggregate, Shared.Errors.DomainError>(PaymentAggregate.Create(Payment.API.Domain.ValueObjects.OrderId.From(message.OrderId))));

        var consumer = new StockReservedEventConsumer(publishEndpoint.Object, paymentService.Object, logger.Object);

        await consumer.Consume(context.Object);

        publishEndpoint.Verify(
            x => x.Publish(It.Is<PaymentCompletedEvent>(e => e.OrderId == message.OrderId), It.IsAny<CancellationToken>()),
            Times.Once);
    }
}

