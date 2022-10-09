using Polly;
using System.Net;

namespace Reference.TransactionalOutbox.Tests.Integration
{
    public class InfrastructureFixture : IDisposable
    {
        internal readonly string OrderCreatedTopicArn;
        internal readonly string OrderCreatedQueueUrl;
        internal readonly string OrderCreatedSubscriptionArn;

        readonly HttpClient httpClient = new();

        public InfrastructureFixture()
        {
            var topicName = $"OrderCreated";
            var queueName = $"OrderCreated";

            OrderCreatedTopicArn = Setup.CreateTopic(topicName).Result;
            OrderCreatedQueueUrl = Setup.CreateQueue(queueName).Result;

            OrderCreatedSubscriptionArn = Setup.SubscribeToTopic(OrderCreatedTopicArn, OrderCreatedQueueUrl).Result;

            WaitForServiceToBeReady();
        }

        static Polly.Retry.RetryPolicy RetryPolicy => Policy.Handle<ApplicationException>()
            .WaitAndRetry(retryCount: 20, sleepDurationProvider: _ => TimeSpan.FromSeconds(1));

        void WaitForServiceToBeReady()
        {
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
        }

        public void Dispose()
        {
            // nothing
        }
    }
}
