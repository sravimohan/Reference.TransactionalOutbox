using Polly;
using Reference.TransactionalOutbox.Application.Usecase.CreateOrder;
using System.Net;
using System.Net.Http.Json;

namespace Reference.TransactionalOutbox.Tests.Integration;

public class OutboxPollingTests : IClassFixture<InfrastructureFixture>
{
    readonly InfrastructureFixture _fixture;

    static Polly.Retry.RetryPolicy RetryPolicy => Policy.Handle<ApplicationException>()
        .WaitAndRetry(retryCount: 20, sleepDurationProvider: _ => TimeSpan.FromSeconds(1));

    public OutboxPollingTests(InfrastructureFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task Can_publish_and_subscribe()
    {
        // setup infrastructure
        var httpClient = new HttpClient();

        // wait for service to be ready
        RetryPolicy.Execute(() =>
        {
            var livezResponse = httpClient.GetAsync($"{Setup.ApiUrl}/livez");
            var readyzResponse = httpClient.GetAsync($"{Setup.ApiUrl}/readyz");

            if (
                livezResponse.Result.StatusCode == HttpStatusCode.OK &&
                readyzResponse.Result.StatusCode == HttpStatusCode.OK)
            {
                return;
            }

            Console.WriteLine($"Waiting for service to be ready...");
            throw new ApplicationException();
        });

        // create orders
        var orders = new List<CreateOrderRequest>
        {
            new CreateOrderRequest(ProductId: 1, Quantity: 1),
            new CreateOrderRequest(ProductId: 2, Quantity: 2),
            new CreateOrderRequest(ProductId: 3, Quantity: 3)
        };

        orders.ForEach(async order => await httpClient.PostAsJsonAsync($"{Setup.ApiUrl}/order", order));

        // wait for events to be published
        Thread.Sleep(5000);

        // subscribe
        var sqs = new SqsHandler(Setup.SQSClient, _fixture.OrderCreatedQueueUrl);
        var handledEvents = await sqs.Handle(CancellationToken.None);
        Assert.Equal(orders.Count, handledEvents.Count);
    }
}
