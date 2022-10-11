using Reference.TransactionalOutbox.Application.Usecase.CreateOrder;
using System.Net.Http.Json;
using Xunit.Abstractions;

namespace Reference.TransactionalOutbox.Tests.Integration;

public class OutboxPollingTests : IClassFixture<InfrastructureFixture>
{
    readonly InfrastructureFixture _fixture;
    readonly HttpClient httpClient = new();
    readonly ITestOutputHelper _output;

    public OutboxPollingTests(InfrastructureFixture fixture, ITestOutputHelper output)
    {
        _fixture = fixture;
        _output = output;
    }

    [Fact]
    public async Task Can_publish_and_subscribe()
    {
        // create orders
        var orderCount = 10;
        await CreateOrders(orderCount);
        _output.WriteLine($"Number of order created: {orderCount}");

        // wait for the service to process the orders
        Thread.Sleep(5000);

        // subscribe
        var sqs = new SqsHandler(Setup.SQSClient, _fixture.OrderCreatedQueueUrl);

        var receivedCount = 0;
        while (receivedCount < orderCount)
        {
            var handledEvents = await sqs.Handle(CancellationToken.None);
            receivedCount += handledEvents.Count;
            _output.WriteLine($"Handled {receivedCount} of {orderCount}");
        }

        Assert.Equal(orderCount, receivedCount);
    }

    async Task CreateOrders(int count)
    {
        var created = 0;
        var createTasks = new List<Task>();

        while (created < count)
        {
            created++;

            var order = new CreateOrderRequest(ProductId: created, Quantity: 1);
            var task = httpClient.PostAsJsonAsync($"{Setup.ApiUrl}/order", order);
            createTasks.Add(task);
        }

        await Task.WhenAll(createTasks);
    }
}
