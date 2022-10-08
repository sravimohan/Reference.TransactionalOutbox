using Dapper;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Reference.TransactionalOutbox.HealthChecks;

public class ReadyzHealthCheck : IHealthCheck
{
    readonly IDbConnection _db;

    static readonly TimeSpan Timeout = TimeSpan.FromSeconds(1);

    public ReadyzHealthCheck(IDbConnection db)
    {
        _db = db;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            await CanConnectToDatabase(_db, cancellationToken);
            return HealthCheckResult.Healthy();
        }
        catch
        {
            return new HealthCheckResult(context.Registration.FailureStatus);
        }
    }

    internal static async Task<bool> CanConnectToDatabase(IDbConnection db, CancellationToken cancellationToken = default)
    {
        try
        {
            await db.ExecuteScalarAsync<string>("select GETDATE()").WaitAsync(Timeout, cancellationToken);
            return true;
        }
        catch
        {
            return false;
        }
    }
}
