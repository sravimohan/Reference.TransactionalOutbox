using Amazon.SimpleNotificationService;
using Reference.TransactionalOutbox.Api.Options;

namespace Reference.TransactionalOutbox.Api.Services;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionstring = configuration.GetConnectionString("Reference");
        Console.WriteLine($"Connecting to DB: {connectionstring}");

        services
            .AddTransient<IDbConnection>(db =>
                new SqlConnection(connectionstring)
            );

        return services;
    }

    public static IServiceCollection AddEventPublishing(this IServiceCollection services, IConfiguration configuration)
    {
        var aws = configuration.GetSection(nameof(AWS)).Get<AWS>();

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
