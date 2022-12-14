namespace Reference.TransactionalOutbox.Application.Usecase.PublishOrderCreated;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddPublishOrderCreated(this IServiceCollection services) =>
        services.AddTransient<PublishOrderCreatedHandler>();
}
