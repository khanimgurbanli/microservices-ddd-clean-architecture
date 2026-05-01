using Payment.API.Domain.Enums;
using Payment.API.Domain.ValueObjects;

namespace Payment.API.Domain.Aggregates;

public class PaymentAggregate
{
    public OrderId OrderId { get; private set; }
    public PaymentStatus Status { get; private set; }

    private PaymentAggregate()
    {
    }

    public static PaymentAggregate Create(OrderId orderId)
    {
        return new PaymentAggregate
        {
            OrderId = orderId,
            Status = PaymentStatus.Completed
        };
    }

    public void MarkFailed()
    {
        Status = PaymentStatus.Failed;
    }
}
