namespace Reference.TransactionalOutbox.Application.Usecase.PublishOrderCreated;

public class PublishOrderCreatedHandler
{
    readonly SnsPublisher _snsPublisher;

    public const string EventType = "OrderCreated";

    public PublishOrderCreatedHandler(SnsPublisher snsPublisher) =>
        _snsPublisher = snsPublisher;

    public async Task<bool> Publish(IEnumerable<string> ordersJson, CancellationToken ct) =>
        await _snsPublisher.Publish<OrderCreated>(ordersJson, ct);
}
