using System;
using System.Diagnostics.CodeAnalysis;

namespace GarageGroup.Infra;

public sealed record class ApiKeyAuthenticationOption
{
    private const string DefaultHeaderName = "X-API-Key";

    private const string DefaultQueryParameterName = "apiKey";

    public ApiKeyAuthenticationOption(
        [AllowNull] string primaryKey,
        [AllowNull] string secondaryKey,
        [AllowNull] string headerName,
        [AllowNull] string queryParameterName)
    {
        PrimaryKey = primaryKey.OrEmpty();
        SecondaryKey = secondaryKey.OrEmpty();
        HeaderName = headerName.OrNullIfWhiteSpace() ?? DefaultHeaderName;
        QueryParameterName = queryParameterName.OrNullIfWhiteSpace() ?? DefaultQueryParameterName;
    }

    public string PrimaryKey { get; }

    public string SecondaryKey { get; }

    public string HeaderName { get; }

    public string QueryParameterName { get; }

    public bool IsDisabled { get; init; }
}