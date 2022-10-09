namespace Reference.TransactionalOutbox.Application.Usecase.OutboxPolling;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddOutboxPolling(this IServiceCollection services) =>
        services.AddHostedService<OutboxPollingService>();
}
