using System;
using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Extensions.Hosting;

partial class AzureHostBuilderExtensions
{
    public static IHostBuilder ConfigureApplicationInsights(this IHostBuilder hostBuilder)
    {
        ArgumentNullException.ThrowIfNull(hostBuilder);
        return hostBuilder.InternalConfigureApplicationInsights();
    }

    internal static IHostBuilder InternalConfigureApplicationInsights(this IHostBuilder hostBuilder)
    {
        return hostBuilder.ConfigureServices(ConfigureTelemetryServices);

        static void ConfigureTelemetryServices(HostBuilderContext context, IServiceCollection services)
        {
            var connectionString = context.GetApplicationInsightsConnectionString();
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                return;
            }

            services.AddApplicationInsightsTelemetry(InnerConfigure);

            void InnerConfigure(ApplicationInsightsServiceOptions options)
                =>
                options.ConnectionString = connectionString;
        }
    }

    private static string? GetApplicationInsightsConnectionString(
        this HostBuilderContext context)
        =>
        context.Configuration["ApplicationInsights:ConnectionString"] ?? context.Configuration["APPLICATIONINSIGHTS_CONNECTION_STRING"];
}