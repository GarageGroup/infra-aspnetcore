using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
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

        appBuilder.Host.ConfigureApplicationInsights();
        if (configure is not null)
        {
            configure.Invoke(appBuilder);
        }

        var app = appBuilder.BuildEndpointApplication();
        app.Map("/health", static app => app.Use(static _ => InvokeHealthCheckAsync));

        return app;
    }

    private static Task InvokeHealthCheckAsync(HttpContext httpContext)
    {
        httpContext.Response.StatusCode = 200;
        return httpContext.Response.WriteAsync("Healthy", httpContext.RequestAborted);
    }
}