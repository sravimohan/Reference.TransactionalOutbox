
using Reference.TransactionalOutbox.Api.HealthChecks;
using Reference.TransactionalOutbox.Api.Services;
using Reference.TransactionalOutbox.Api.Usecase.CreateOrder;
using Reference.TransactionalOutbox.Api.Usecase.OutboxPolling;
using Reference.TransactionalOutbox.Api.Usecase.PublishOrderCreated;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddHealthChecks()
    .AddCheck<ReadyzHealthCheck>("readyz", tags: new[] { "ready" });

builder.Services
    .AddDatabase(builder.Configuration)
    .AddEventPublishing(builder.Configuration)
    .AddCreateOrder()
    .AddOutboxPolling()
    .AddPublishOrderCreated();

var app = builder.Build();

app.MapPost("/order/", async (CreateOrderHandler handler, CreateOrderRequest request) => await handler.ProcessAsync(request));

app.MapHealthChecks("/readyz", new HealthCheckOptions { Predicate = healthCheck => healthCheck.Tags.Contains("ready") });

app.MapHealthChecks("livez", new HealthCheckOptions { Predicate = _ => false });

app.Run();

