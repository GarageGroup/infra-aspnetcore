using System;

namespace Microsoft.AspNetCore.Builder;

public static class EndpointApplicationBuilderExtensions
{
    public static TApplicationBuilder UseHttpsRedirection<TApplicationBuilder>(this TApplicationBuilder applicationBuilder)
        where TApplicationBuilder : class, IApplicationBuilder
    {
        ArgumentNullException.ThrowIfNull(applicationBuilder);

        _ = HttpsPolicyBuilderExtensions.UseHttpsRedirection(applicationBuilder);
        return applicationBuilder;
    }
}