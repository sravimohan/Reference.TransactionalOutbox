namespace Reference.TransactionalOutbox.Api.Usecase.PublishOrderCreated;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddPublishOrderCreated(this IServiceCollection services) =>
        services.AddTransient<PublishOrderCreatedHandler>();
}
