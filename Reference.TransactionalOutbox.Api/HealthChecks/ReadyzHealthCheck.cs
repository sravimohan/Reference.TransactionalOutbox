using Microsoft.Extensions.Diagnostics.HealthChecks;
using Reference.TransactionalOutbox.Application.Usecase.HealthChecks;

namespace Reference.TransactionalOutbox.Api.HealthChecks;

public class ReadyzHealthCheck : IHealthCheck
{
    readonly DatabaseHealthCheck _databaseHealthCheck;

    public ReadyzHealthCheck(DatabaseHealthCheck databaseHealthCheck) =>
        _databaseHealthCheck = databaseHealthCheck;

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default) =>
        await _databaseHealthCheck.CanConnectToDatabase(cancellationToken)
            ? HealthCheckResult.Healthy()
            : new HealthCheckResult(context.Registration.FailureStatus);
}
