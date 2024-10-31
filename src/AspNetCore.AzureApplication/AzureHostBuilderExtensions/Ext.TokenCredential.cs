using System;
using GarageGroup.Infra;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Extensions.Hosting;

partial class AzureHostBuilderExtensions
{
    public static IHostBuilder ConfigureRefreshableTokenCredentialStandard(this IHostBuilder hostBuilder)
    {
        ArgumentNullException.ThrowIfNull(hostBuilder);
        return hostBuilder.InternalConfigureRefreshableTokenCredentialStandard();
    }

    internal static IHostBuilder InternalConfigureRefreshableTokenCredentialStandard(this IHostBuilder hostBuilder)
        =>
        hostBuilder.ConfigureServices(ConfigureRefreshableTokenCredentialStandardServices);

    private static void ConfigureRefreshableTokenCredentialStandardServices(HostBuilderContext context, IServiceCollection services)
        =>
        services.AddRefreshableTokenCredentialStandardAsSingleton().AddHostedService(RefreshableTokenCredentialBackgroundService.Resolve);
}