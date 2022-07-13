using System;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Extensions.Hosting;

public static class EndpointHostBuilderExtensions
{
    public static IHostBuilder ConfigureEndpointsApiExplorer(this IHostBuilder hostBuilder)
    {
        _ = hostBuilder ?? throw new ArgumentNullException(nameof(hostBuilder));

        return hostBuilder.ConfigureServices(static s => s.AddEndpointsApiExplorer());
    }
}