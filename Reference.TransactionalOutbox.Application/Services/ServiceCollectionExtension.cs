using Amazon.SimpleNotificationService;
using Microsoft.Extensions.Configuration;
using Reference.TransactionalOutbox.Application.Options;

namespace Reference.TransactionalOutbox.Application.Services;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionstring = configuration.GetConnectionString("Reference");
        Console.WriteLine($"Connecting to DB: {connectionstring}");

        return services
            .AddTransient<IDbConnection>(db => new SqlConnection(connectionstring))
            .AddTransient<DatabaseHealthCheck>();
    }

    public static IServiceCollection AddEventPublishing(this IServiceCollection services, AWS aws)
    {
        services
            .AddSingleton<IAmazonSimpleNotificationService>(_ => SNSClient(aws.ServiceURL))
            .AddSingleton(_ => aws.SNS)
            .AddSingleton<SnsPublisher>();

        return services;
    }

    static AmazonSimpleNotificationServiceClient SNSClient(string? ServiceURL = null)
    {
        var config = new AmazonSimpleNotificationServiceConfig();

        if (ServiceURL != null)
        {
            config.ServiceURL = ServiceURL;
        }

        return new AmazonSimpleNotificationServiceClient(config);
    }
}
