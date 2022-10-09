var builder = WebApplication.CreateBuilder(args);

var aws = builder.Configuration.GetSection(nameof(AWS)).Get<AWS>();

builder.Services
    .AddHealthChecks()
    .AddCheck<ReadyzHealthCheck>("readyz", tags: new[] { "ready" });

builder.Services
    .AddDatabase(builder.Configuration)
    .AddEventPublishing(aws)
    .AddCreateOrder()
    .AddOutboxPolling()
    .AddPublishOrderCreated();

var app = builder.Build();

app.MapPost("/order/", async (CreateOrderHandler handler, CreateOrderRequest request) => await handler.ProcessAsync(request));

app.MapHealthChecks("/readyz", new HealthCheckOptions { Predicate = healthCheck => healthCheck.Tags.Contains("ready") });

app.MapHealthChecks("livez", new HealthCheckOptions { Predicate = _ => false });

app.Run();

