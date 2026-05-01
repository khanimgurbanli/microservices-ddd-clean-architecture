using MassTransit;
using Order.API.Application;
using Order.API.Application.Commands.Order;
using Order.API.Infrastructure;
using Order.API.Infrastructure.Consumers;
using Order.API.Infrastructure.Dispatching;
using Order.API.Infrastructure.Messaging;
using Order.API.Infrastructure.Persistence;
using Shared;
using Shared.Abstractions.Dispatching;
using Shared.Common.Behaviors;
using Shared.Events;
using MediatR;

var builder = WebApplication.CreateBuilder(args);

// Add layers
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();

var useCqrs = builder.Configuration.GetValue("UseCqrs", true);
if (useCqrs)
{
    builder.Services.AddMediatR(cfg =>
    {
        cfg.RegisterServicesFromAssembly(typeof(CreateOrderCommand).Assembly);
        cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
    });
    builder.Services.AddScoped<IRequestDispatcher, MediatRRequestDispatcher>();
}
else
{
    builder.Services.AddScoped<IRequestDispatcher, DirectRequestDispatcher>();
}

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<PaymentCompletedEventConsumer>();
    x.AddConsumer<StockNotReservedEventConsumer>();
    x.AddConsumer<PaymentFailedEventConsumer>();
    x.AddConsumer<DomainEventConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(builder.Configuration["RabbitMQ:Host"]);

        cfg.ReceiveEndpoint(RabbitMQSettings.Order_PaymentCompletedEventQueue, e =>
        {
            e.ConfigureConsumer<PaymentCompletedEventConsumer>(context);
        });
        cfg.ReceiveEndpoint(RabbitMQSettings.Order_StockNotReservedEventQueue, e =>
        {
            e.ConfigureConsumer<StockNotReservedEventConsumer>(context);
        });
        cfg.ReceiveEndpoint(RabbitMQSettings.Order_PaymentFailedEventQueue, e =>
        {
            e.ConfigureConsumer<PaymentFailedEventConsumer>(context);
        });
    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<OrderDbContext>();
    await OrderDbContextInitializer.InitializeAsync(dbContext);
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
