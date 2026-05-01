using CSharpFunctionalExtensions;
using Payment.API.Domain.Aggregates;
using Shared.Errors;

namespace Payment.API.Application.Interfaces;

public interface IPaymentService
{
    Task<Result<PaymentAggregate, DomainError>> ProcessAsync(PaymentAggregate payment);
}
