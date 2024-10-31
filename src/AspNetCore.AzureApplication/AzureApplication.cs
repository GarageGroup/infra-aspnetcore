using System;
using Microsoft.Extensions.Hosting;

namespace Microsoft.AspNetCore.Builder;

public static class AzureApplication
{
    public static EndpointApplication Create(string[] args)
        =>
        InnerCreate(args, null);

    public static EndpointApplication Create(string[] args, Action<WebApplicationBuilder> configure)
        =>
        InnerCreate(args, configure ?? throw new ArgumentNullException(nameof(configure)));

    private static EndpointApplication InnerCreate(string[] args, Action<WebApplicationBuilder>? configure)
    {
        var appBuilder = WebApplication.CreateBuilder(args);

        appBuilder.Host
            .InternalConfigureApplicationInsights()
            .InternalConfigureRefreshableTokenCredentialStandard()
            .InternalConfigureAzureAppConfiguration();

        configure?.Invoke(appBuilder);
        return appBuilder.BuildEndpointApplication();
    }
}