namespace Reference.TransactionalOutbox.Application.Usecase.HealthChecks;

public class DatabaseHealthCheck
{
    readonly IDbConnection _db;

    static readonly TimeSpan Timeout = TimeSpan.FromSeconds(1);

    public DatabaseHealthCheck(IDbConnection db) => _db = db;

    internal async Task<bool> CanConnectToDatabase(CancellationToken cancellationToken = default)
    {
        try
        {
            await _db.ExecuteScalarAsync("select GETDATE()").WaitAsync(Timeout, cancellationToken);
            return true;
        }
        catch
        {
            return false;
        }
    }
}
