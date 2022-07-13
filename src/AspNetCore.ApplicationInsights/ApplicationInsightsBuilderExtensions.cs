using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Microsoft.Extensions.Hosting;

public static class ApplicationInsightsBuilderExtensions
{
    public static IHostBuilder ConfigureApplicationInsights(this IHostBuilder hostBuilder)
    {
        _ = hostBuilder ?? throw new ArgumentNullException(nameof(hostBuilder));

        return hostBuilder.ConfigureServices(ConfigureTelemetryServices).ConfigureLogging(ConfigureApplicationInsights);
    }

    private static void ConfigureApplicationInsights(HostBuilderContext context, ILoggingBuilder builder)
    {
        var instrumentationKey = context.Configuration["APPINSIGHTS_INSTRUMENTATIONKEY"];

        if (string.IsNullOrEmpty(instrumentationKey))
        {
            return;
        }

        builder.AddApplicationInsights(instrumentationKey);
    }

    private static void ConfigureTelemetryServices(HostBuilderContext _, IServiceCollection services)
        =>
        services.AddApplicationInsightsTelemetry();
}