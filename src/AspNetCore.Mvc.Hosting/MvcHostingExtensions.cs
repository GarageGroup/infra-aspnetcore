using System;
using GGroupp.Infra;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Extensions.Hosting;

public static class MvcHostingExtensions
{
    public static IHostBuilder ConfigureEndpointControllers(this IHostBuilder hostBuilder)
    {
        ArgumentNullException.ThrowIfNull(hostBuilder);
        return hostBuilder.ConfigureServices(ConfigureServices);
    }

    public static IHostBuilder ConfigureController<T>(this IHostBuilder hostBuilder, Func<IServiceProvider, T> controllerResolver)
        where T : class
    {
        ArgumentNullException.ThrowIfNull(hostBuilder);
        ArgumentNullException.ThrowIfNull(controllerResolver);

        return hostBuilder.ConfigureServices(InnerConfigureServices);

        void InnerConfigureServices(HostBuilderContext _, IServiceCollection services)
            =>
            services.AddTransient(controllerResolver);
    }

    private static void ConfigureServices(HostBuilderContext _, IServiceCollection services)
        =>
        services.AddMvcCore(SetupClaimValueProviderFactory).AddApiExplorer().AddControllersAsServices();

    private static void SetupClaimValueProviderFactory(MvcOptions options)
        =>
        options.ValueProviderFactories.Add(new ClaimValueProviderFactory());
}