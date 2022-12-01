using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Microsoft.Extensions.Hosting;

public static class ApplicationInsightsBuilderExtensions
{
    public static IHostBuilder ConfigureApplicationInsights(this IHostBuilder hostBuilder)
    {
        ArgumentNullException.ThrowIfNull(hostBuilder);
        return hostBuilder.ConfigureServices(ConfigureTelemetryServices).ConfigureLogging(ConfigureApplicationInsights);
    }

    private static void ConfigureApplicationInsights(HostBuilderContext context, ILoggingBuilder builder)
    {
        if (context.HasApplicationInsightsConnectionString() is false)
        {
            return;
        }

        builder.AddApplicationInsights();
    }

    private static void ConfigureTelemetryServices(HostBuilderContext context, IServiceCollection services)
    {
        if (context.HasApplicationInsightsConnectionString() is false)
        {
            return;
        }

        services.AddApplicationInsightsTelemetry();
    }

    private static bool HasApplicationInsightsConnectionString(this HostBuilderContext context)
        =>
        string.IsNullOrEmpty(context.Configuration["ApplicationInsights:ConnectionString"]) is false;
}