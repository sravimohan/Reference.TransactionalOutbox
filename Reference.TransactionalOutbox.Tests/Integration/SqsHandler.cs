using Amazon.SQS;
using Amazon.SQS.Model;

namespace Reference.TransactionalOutbox.Tests.Integration;

internal class SqsHandler
{
    readonly IAmazonSQS _sqs;
    readonly string _queueUrl;
    readonly List<string> ALL = new() { "All" };

    public SqsHandler(IAmazonSQS sqs, string queueUrl)
    {
        _sqs = sqs;
        _queueUrl = queueUrl;
    }

    ReceiveMessageRequest ReceiveMessageRequest(String queueUrl) => new()
    {
        QueueUrl = queueUrl,
        MaxNumberOfMessages = 10,
        MessageAttributeNames = ALL,
        AttributeNames = ALL,
    };

    internal async Task<List<Message>> Handle(CancellationToken ct)
    {
        var sqsResponse = await _sqs.ReceiveMessageAsync(ReceiveMessageRequest(_queueUrl), ct);

        // delete processed Messages
        sqsResponse.Messages.ForEach(async m => await _sqs.DeleteMessageAsync(_queueUrl, m.ReceiptHandle, ct));

        return sqsResponse.Messages;
    }
}
