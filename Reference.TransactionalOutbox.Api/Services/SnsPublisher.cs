using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using Reference.TransactionalOutbox.Api.Options;

namespace Reference.TransactionalOutbox.Api.Services;


public class SnsPublisher
{
    readonly IAmazonSimpleNotificationService _sns;
    readonly SnsOptions _options;

    public SnsPublisher(IAmazonSimpleNotificationService sns, SnsOptions options)
    {
        _sns = sns;
        _options = options;
    }

    public async Task<bool> Publish<T>(IEnumerable<string> messages, CancellationToken ct)
    {
        var request = new PublishBatchRequest
        {
            TopicArn = _options.TopicArn[typeof(T).Name],
            PublishBatchRequestEntries = CreateBatchRequest(messages),
        };

        var response = await _sns.PublishBatchAsync(request, ct);
        return response.HttpStatusCode == System.Net.HttpStatusCode.OK;
    }

    static List<PublishBatchRequestEntry> CreateBatchRequest(IEnumerable<string> ordersJson) =>
        ordersJson.Select(order => new PublishBatchRequestEntry { Message = order }).ToList();
}
