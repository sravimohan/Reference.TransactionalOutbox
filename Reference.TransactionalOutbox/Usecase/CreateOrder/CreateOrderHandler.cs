using Dapper;
using System.Text.Json;
using System.Transactions;

namespace Reference.TransactionalOutbox.Usecase.CreateOrder;

public record Order(int ProductId, int Quantity);

public record Outbox(string EventType, string EventValue);

public record CreateOrderRequest(int ProductId, int Quantity);

public class CreateOrderHandler
{
    const string EventType = "OrderCreated";
    readonly IDbConnection _db;
    readonly ILogger<CreateOrderHandler> _logger;

    public CreateOrderHandler(IDbConnection db, ILogger<CreateOrderHandler> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task ProcessAsync(CreateOrderRequest request)
    {
        var order = new Order(request.ProductId, request.Quantity);
        var outbox = new Outbox(EventType, JsonSerializer.Serialize(order));

        using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        await InsertOrder(order);
        await InsertOutbox(outbox);
        transaction.Complete();

        _logger.LogInformation("Order created, Product Id : {orderId}, Quantity: {quantity}", order.ProductId, order.Quantity);
    }

    async Task<int> InsertOrder(Order order) =>
        await _db.ExecuteScalarAsync<int>("INSERT INTO Orders (ProductId, Quantity) VALUES (@ProductId, @Quantity)", order);

    async Task<int> InsertOutbox(Outbox outbox) =>
        await _db.ExecuteScalarAsync<int>("INSERT INTO Outbox (EventType, EventValue) VALUES (@EventType, @EventValue)", outbox);
}
