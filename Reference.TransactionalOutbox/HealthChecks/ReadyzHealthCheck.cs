using Dapper;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Reference.TransactionalOutbox.HealthChecks;

public class ReadyzHealthCheck : IHealthCheck
{
    readonly IDbConnection _db;

    public ReadyzHealthCheck(IDbConnection db)
    {
        _db = db;
    }

    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            var timeout = new TimeSpan(0, 0, 1);
            _ = _db.ExecuteScalarAsync<string>("select GETDATE()").WaitAsync(timeout, cancellationToken).Result;
            return Task.FromResult(HealthCheckResult.Healthy());
        }
        catch
        {
            return Task.FromResult(new HealthCheckResult(context.Registration.FailureStatus));
        }
    }
}
