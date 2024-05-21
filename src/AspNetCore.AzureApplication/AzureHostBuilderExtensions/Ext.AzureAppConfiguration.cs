using System;
using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.Hosting;

partial class AzureHostBuilderExtensions
{
    public static IHostBuilder ConfigureAzureAppConfiguration(this IHostBuilder hostBuilder)
    {
        ArgumentNullException.ThrowIfNull(hostBuilder);
        return hostBuilder.InternalConfigureAzureAppConfiguration();
    }

    internal static IHostBuilder InternalConfigureAzureAppConfiguration(this IHostBuilder hostBuilder)
        =>
        hostBuilder.ConfigureAppConfiguration(ConfigureApplicationInsights);

    private static void ConfigureApplicationInsights(HostBuilderContext context, IConfigurationBuilder builder)
    {
        var connectionString = context.Configuration.GetConnectionString("AppConfig");
        if (string.IsNullOrEmpty(connectionString))
        {
            return;
        }

        builder.AddAzureAppConfiguration(connectionString);
    }
}