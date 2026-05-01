using MassTransit;
using MediatR;
using MongoDB.Driver;
using Stock.API.Application;
using Stock.API.Application.Commands.Stock;
using Stock.API.Application.Queries.Stock;
using Stock.API.Infrastructure;
using Stock.API.Infrastructure.Consumers;
using Stock.API.Infrastructure.Dispatching;
using Shared.Common.Behaviors;
using Shared.Abstractions.Dispatching;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var useCqrs = builder.Configuration.GetValue("UseCqrs", true);
if (useCqrs)
{
    builder.Services.AddMediatR(cfg =>
    {
        cfg.RegisterServicesFromAssembly(typeof(CreateStockCommand).Assembly);
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
    x.AddConsumer<CreateOrderEventConsumer>();
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(builder.Configuration["RabbitMQ:Host"]);
        cfg.ReceiveEndpoint(Shared.RabbitMQSettings.Stock_OrderCreatedEventQueue, e =>
        {
            e.ConfigureConsumer<CreateOrderEventConsumer>(context);
        });
    });
});

var app = builder.Build();

try
{
    var mongoDbContext = app.Services.GetRequiredService<Stock.API.Infrastructure.Database.MongoDbContext>();
    var collection = mongoDbContext.Stocks;
    using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(3));
    if (!await collection.Find(_ => true).AnyAsync(cts.Token))
    {
        await collection.InsertOneAsync(new() { ProductId = Guid.NewGuid(), Count = 100 }, cancellationToken: cts.Token);
        await collection.InsertOneAsync(new() { ProductId = Guid.NewGuid(), Count = 200 }, cancellationToken: cts.Token);
        await collection.InsertOneAsync(new() { ProductId = Guid.NewGuid(), Count = 300 }, cancellationToken: cts.Token);
        await collection.InsertOneAsync(new() { ProductId = Guid.NewGuid(), Count = 400 }, cancellationToken: cts.Token);
    }
}
catch (Exception ex)
{
    app.Logger.LogWarning("MongoDB seed skipped: {ErrorType}: {ErrorMessage}", ex.GetType().Name, ex.Message);
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
