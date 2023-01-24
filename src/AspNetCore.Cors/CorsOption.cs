using System.Diagnostics.CodeAnalysis;

namespace GGroupp.Infra;

public sealed record class CorsOption
{
    private const string AllValues = "*";

    private const string DeaultMethods = "GET,POST,OPTIONS,PUT,DELETE,PATCH";

    public CorsOption(
        [AllowNull] string allowOrigin = AllValues,
        bool allowCredentials = true,
        [AllowNull] string allowHeaders = AllValues,
        [AllowNull] string allowMethods = DeaultMethods,
        int? maxAgeInMilliseconds = null,
        [AllowNull] string exposeHeaders = AllValues)
    {
        AllowOrigin = string.IsNullOrEmpty(allowOrigin) ? AllValues : allowOrigin;
        AllowCredentials = allowCredentials;
        AllowHeaders = string.IsNullOrEmpty(allowHeaders) ? AllValues : allowHeaders;
        AllowMethods = string.IsNullOrEmpty(allowMethods) ? DeaultMethods : allowMethods;
        MaxAgeInMilliseconds = maxAgeInMilliseconds;
        ExposeHeaders = string.IsNullOrEmpty(exposeHeaders) ? AllValues : exposeHeaders;
    }

    public string AllowOrigin { get; }

    public bool AllowCredentials { get; }

    public string AllowHeaders { get; }

    public string AllowMethods { get; }

    public int? MaxAgeInMilliseconds { get; }

    public string ExposeHeaders { get; }
}