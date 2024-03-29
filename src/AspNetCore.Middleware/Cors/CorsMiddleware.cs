using System;
using System.Threading.Tasks;
using GarageGroup.Infra;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.AspNetCore.Builder;

public static class CorsMiddleware
{
    private const string DefaultCorsSectionName = "Cors";

    public static TApplicationBuilder AllowCors<TApplicationBuilder>(this TApplicationBuilder app, string sectionName = DefaultCorsSectionName)
        where TApplicationBuilder : IApplicationBuilder
    {
        ArgumentNullException.ThrowIfNull(app);

        var section = app.ApplicationServices.GetService<IConfiguration>()?.GetSection(sectionName ?? DefaultCorsSectionName);
        var option = section.GetCorsOption();

        _ = app.Use(InnerInvokeAsync);
        return app;

        Task InnerInvokeAsync(HttpContext context, Func<Task> next)
            =>
            context.NextAsync(next, option);
    }

    public static TApplicationBuilder AllowCors<TApplicationBuilder>(
        this TApplicationBuilder app, Func<IServiceProvider, CorsOption> optionResolver)
        where TApplicationBuilder : IApplicationBuilder
    {
        ArgumentNullException.ThrowIfNull(app);

        _ = app.Use(InnerInvokeAsync);
        return app;

        Task InnerInvokeAsync(HttpContext context, Func<Task> next)
            =>
            context.NextAsync(next, optionResolver.Invoke(context.RequestServices));
    }

    private static Task NextAsync(this HttpContext context, Func<Task> next, CorsOption option)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(next);

        context.Response.SetHeaderValue("Access-Control-Allow-Origin", option.AllowOrigin);
        context.Response.SetHeaderValue("Access-Control-Allow-Credentials", option.AllowCredentials ? "true" : "false");
        context.Response.SetHeaderValue("Access-Control-Allow-Headers", option.AllowHeaders);
        context.Response.SetHeaderValue("Access-Control-Allow-Methods", option.AllowMethods);
        context.Response.SetHeaderValue("Access-Control-Max-Age", option.MaxAgeInMilliseconds?.ToString());
        context.Response.SetHeaderValue("Access-Control-Expose-Headers", option.ExposeHeaders);

        if (string.Equals(context.Request.Method, "OPTIONS", StringComparison.InvariantCultureIgnoreCase))
        {
            context.Response.StatusCode = StatusCodes.Status204NoContent;
            return Task.CompletedTask;
        }

        return next.Invoke();
    }

    private static void SetHeaderValue(this HttpResponse httpResponse, string headerName, string? headerValue)
    {
        if (string.IsNullOrEmpty(headerValue))
        {
            return;
        }

        httpResponse.Headers[headerName] = headerValue;
    }

    private static CorsOption GetCorsOption(this IConfigurationSection? section)
    {
        if (section is null)
        {
            return new();
        }

        return new(
            allowOrigin: section["AllowOrigin"],
            allowCredentials: section.GetValue<bool?>("AllowCredentials") ?? true,
            allowHeaders: section["AllowHeaders"],
            allowMethods: section["AllowMethods"],
            maxAgeInMilliseconds: section.GetValue<int?>("MaxAgeInMilliseconds"),
            exposeHeaders: section["ExposeHeaders"]);
    }
}