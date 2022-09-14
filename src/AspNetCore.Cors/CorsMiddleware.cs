using System;
using System.Threading.Tasks;
using GGroupp.Infra;
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
        _ = app ?? throw new ArgumentNullException(nameof(app));

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
        _ = app ?? throw new ArgumentNullException(nameof(app));

        _ = app.Use(InnerInvokeAsync);

        return app;

        Task InnerInvokeAsync(HttpContext context, Func<Task> next)
            =>
            context.NextAsync(next, optionResolver.Invoke(context.RequestServices));
    }

    private static Task NextAsync(this HttpContext context, Func<Task> next, CorsOption option)
    {
        _ = context ?? throw new ArgumentNullException(nameof(context));
        _ = next ?? throw new ArgumentNullException(nameof(next));

        context.Response.SetHeaderValue("Access-Control-Allow-Origin", option.AllowOrigin);
        context.Response.SetHeaderValue("Access-Control-Allow-Credentials", option.AllowCredentials ? "true" : "false");
        context.Response.SetHeaderValue("Access-Control-Allow-Headers", option.AllowHeaders);
        context.Response.SetHeaderValue("Access-Control-Allow-Methods", option.AllowMethods);
        context.Response.SetHeaderValue("Access-Control-Max-Age", option.MaxAgeInMilliseconds?.ToString());

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
        =>
        new(
            allowOrigin: section?.GetValue<string>("AllowOrigin"),
            allowCredentials: section?.GetValue<bool?>("AllowCredentials") ?? true,
            allowHeaders: section?.GetValue<string>("AllowHeaders"),
            allowMethods: section?.GetValue<string>("AllowMethods"),
            maxAgeInMilliseconds: section?.GetValue<int?>("MaxAgeInMilliseconds"));
}