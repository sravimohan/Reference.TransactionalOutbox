namespace Reference.TransactionalOutbox.Application.Options;

public record AWS
{
    public string ServiceURL { get; init; } = default!;

    public SnsOptions SNS { get; init; } = default!;
}

public record SnsOptions
{
    public IDictionary<string, string> TopicArn { get; init; } = default!;
}
