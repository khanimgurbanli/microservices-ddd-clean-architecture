using CSharpFunctionalExtensions;
using Payment.API.Application.Interfaces;
using Payment.API.Domain.Aggregates;
using Shared.Errors;

namespace Payment.API.Infrastructure.Services;

public class PaymentService : IPaymentService
{
    public Task<Result<PaymentAggregate, DomainError>> ProcessAsync(PaymentAggregate payment)
    {
        return Task.FromResult(Result.Success<PaymentAggregate, DomainError>(payment));
    }
}
