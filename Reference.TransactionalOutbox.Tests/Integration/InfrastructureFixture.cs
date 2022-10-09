namespace Reference.TransactionalOutbox.Tests.Integration
{
    public class InfrastructureFixture : IDisposable
    {
        internal readonly string OrderCreatedTopicArn;
        internal readonly string OrderCreatedQueueUrl;
        internal readonly string OrderCreatedSubscriptionArn;

        public InfrastructureFixture()
        {
            var topicName = $"OrderCreated";
            var queueName = $"OrderCreated";

            OrderCreatedTopicArn = Setup.CreateTopic(topicName).Result;
            OrderCreatedQueueUrl = Setup.CreateQueue(queueName).Result;

            OrderCreatedSubscriptionArn = Setup.SubscribeToTopic(OrderCreatedTopicArn, OrderCreatedQueueUrl).Result;
        }

        public void Dispose()
        {
            // nothing
        }
    }
}
