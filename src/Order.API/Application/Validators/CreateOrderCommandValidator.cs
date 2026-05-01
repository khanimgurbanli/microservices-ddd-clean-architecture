using FluentValidation;
using Order.API.Application.Commands.Order;

namespace Order.API.Application.Validators;

public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderCommandValidator()
    {
        RuleFor(x => x.BuyerId)
            .NotEmpty()
            .WithMessage("BuyerId is required");

        RuleFor(x => x.OrderItems)
            .NotEmpty()
            .WithMessage("Order must contain at least one item");

        RuleForEach(x => x.OrderItems).ChildRules(item =>
        {
            item.RuleFor(x => x.ProductId)
                .NotEmpty()
                .WithMessage("ProductId is required");

            item.RuleFor(x => x.Count)
                .GreaterThan(0)
                .WithMessage("Count must be greater than 0");

            item.RuleFor(x => x.Price)
                .GreaterThan(0)
                .WithMessage("Price must be greater than 0");
        });
    }
}
