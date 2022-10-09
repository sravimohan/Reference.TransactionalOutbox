namespace Reference.TransactionalOutbox.Api.Usecase.CreateOrder;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddCreateOrder(this IServiceCollection services) =>
        services.AddScoped<CreateOrderHandler>();
}
