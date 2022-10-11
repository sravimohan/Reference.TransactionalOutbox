using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using Amazon.SQS;
using Amazon.SQS.Model;

namespace Reference.TransactionalOutbox.Tests.Integration;

internal static class Setup
{
    public static string ServiceURL =>
        string.IsNullOrEmpty(Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER"))
            ? "http://localhost:4566" : "http://localstack:4566";

    public static string ApiUrl =>
    string.IsNullOrEmpty(Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER"))
        ? "http://localhost:5000" : "http://api";

    internal static readonly AmazonSQSClient SQSClient;
    internal static readonly AmazonSimpleNotificationServiceClient SNSClient;

    static Setup()
    {
        SQSClient = CreateSQSClient();
        SNSClient = CreateSNSClient();
    }

    static AmazonSQSClient CreateSQSClient()
    {
        var sqsConfig = new AmazonSQSConfig()
        {
            ServiceURL = ServiceURL
        };

        return new AmazonSQSClient(sqsConfig);
    }

    static AmazonSimpleNotificationServiceClient CreateSNSClient()
    {
        var snsConfig = new AmazonSimpleNotificationServiceConfig
        {
            ServiceURL = ServiceURL
        };

        return new AmazonSimpleNotificationServiceClient(snsConfig);
    }

    public static async Task<string> CreateTopic(string topicName)
    {
        var response = await SNSClient.CreateTopicAsync(new CreateTopicRequest { Name = topicName });
        return response.TopicArn;
    }

    public static async Task<string> CreateQueue(string queueName)
    {
        var response = await SQSClient.CreateQueueAsync(new CreateQueueRequest
        {
            QueueName = queueName,
            Attributes = new Dictionary<string, string>
            {
                {  "VisibilityTimeout", "1" }
            }
        });
        return response.QueueUrl;
    }

    public static async Task<string> SubscribeToTopic(string topicArn, string queueUrl) =>
        await SNSClient.SubscribeQueueAsync(topicArn, SQSClient, queueUrl);
}
