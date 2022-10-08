﻿using Dapper;
using System.Transactions;
using IsolationLevel = System.Transactions.IsolationLevel;

namespace Reference.TransactionalOutbox.Usecase.OutboxPolling;

public class OutboxPollingService : BackgroundService
{
    readonly IDbConnection _db;
    readonly PublishOrderCreatedHandler _publishOrderCreatedHandler;
    readonly ILogger<OutboxPollingService> _logger;

    readonly static TimeSpan Frequency = TimeSpan.FromSeconds(1);

    public OutboxPollingService(
        IDbConnection db,
        PublishOrderCreatedHandler publishOrderCreatedHandler,
        ILogger<OutboxPollingService> logger)
    {
        _db = db;
        _publishOrderCreatedHandler = publishOrderCreatedHandler;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await Process(stoppingToken);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "OutboxPollingService");
            }

            await Task.Delay(Frequency, stoppingToken);
        }
    }

    async Task Process(CancellationToken ct)
    {
        using var scope = new TransactionScope(
           TransactionScopeOption.RequiresNew,
           new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }, // required for using READPAST hint
           TransactionScopeAsyncFlowOption.Enabled); // required for async

        var events = await ReadFromOutbox();
        _logger.LogInformation("Handling {count} events from outbox", events.Count());


        if (!events.Any())
            return;

        await HandleOrderCreated(events, ct);

        scope.Complete();
    }

    async Task HandleOrderCreated(IEnumerable<Outbox> events, CancellationToken ct)
    {
        var orders = events.Where(e => e.EventType == PublishOrderCreatedHandler.EventType).Select(e => e.EventValue);
        await _publishOrderCreatedHandler.Publish(orders, ct);
    }

    async Task<IEnumerable<Outbox>> ReadFromOutbox() =>
        await _db.QueryAsync<Outbox>("DELETE TOP (1) FROM Outbox WITH (READPAST) OUTPUT DELETED.EventType, DELETED.EventValue");
}