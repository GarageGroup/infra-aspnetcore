using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GarageGroup.Infra;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

namespace Microsoft.AspNetCore.Builder;

public sealed class EndpointApplication : IHost, IDisposable, IApplicationBuilder, IEndpointRouteBuilder, IAsyncDisposable, ISwaggerBuilder
{
    public static EndpointApplication Create(string[]? args = null)
    {
        var webApplicationBuilder = args is null ? WebApplication.CreateBuilder() : WebApplication.CreateBuilder(args);
        return InternalFromWebApplicationBuilder(webApplicationBuilder);
    }

    internal static EndpointApplication InternalFromWebApplicationBuilder(WebApplicationBuilder webApplicationBuilder)
    {
        webApplicationBuilder.Services.AddSocketsHttpHandlerProviderAsSingleton();

        var webApplication = webApplicationBuilder.Build();
        webApplication.UseRouting();

        return new(webApplication);
    }

    private readonly WebApplication webApplication;

    private readonly SwaggerBuilder swaggerBuilder;

    private EndpointApplication(WebApplication webApplication)
    {
        this.webApplication = webApplication;
        swaggerBuilder = new();
    }

    public IServiceProvider Services
        =>
        webApplication.Services;

    public IConfiguration Configuration
        =>
        webApplication.Configuration;

    public IWebHostEnvironment Environment
        =>
        webApplication.Environment;

    public IHostApplicationLifetime Lifetime
        =>
        webApplication.Lifetime;

    public ILogger Logger
        =>
        webApplication.Logger;

    public ICollection<string> Urls
        =>
        webApplication.Urls;

    IServiceProvider IApplicationBuilder.ApplicationServices
    {
        get => ((IApplicationBuilder)webApplication).ApplicationServices;
        set => ((IApplicationBuilder)webApplication).ApplicationServices = value;
    }

    IFeatureCollection IApplicationBuilder.ServerFeatures
        =>
        ((IApplicationBuilder)webApplication).ServerFeatures;

    IDictionary<string, object?> IApplicationBuilder.Properties
        =>
        ((IApplicationBuilder)webApplication).Properties;

    IServiceProvider IEndpointRouteBuilder.ServiceProvider
        =>
        ((IEndpointRouteBuilder)webApplication).ServiceProvider;

    ICollection<EndpointDataSource> IEndpointRouteBuilder.DataSources
        =>
        ((IEndpointRouteBuilder)webApplication).DataSources;

    public void Run(string? url = null)
        =>
        webApplication.Run(url);

    public Task RunAsync(string? url = null)
        =>
        webApplication.RunAsync(url);

    public Task StartAsync(CancellationToken cancellationToken = default)
        =>
        webApplication.StartAsync(cancellationToken);

    public Task StopAsync(CancellationToken cancellationToken = default)
        =>
        webApplication.StopAsync(cancellationToken);

    public ValueTask DisposeAsync()
        =>
        webApplication.DisposeAsync();

    void IDisposable.Dispose()
        =>
        ((IDisposable)webApplication).Dispose();

    IApplicationBuilder IApplicationBuilder.Use(Func<RequestDelegate, RequestDelegate> middleware)
        =>
        webApplication.Use(middleware);

    IApplicationBuilder IApplicationBuilder.New()
        =>
        ((IApplicationBuilder)webApplication).New();

    RequestDelegate IApplicationBuilder.Build()
        =>
        ((IApplicationBuilder)webApplication).Build();

    IApplicationBuilder IEndpointRouteBuilder.CreateApplicationBuilder()
        =>
        ((IEndpointRouteBuilder)webApplication).CreateApplicationBuilder();

    ISwaggerBuilder ISwaggerBuilder.Use(Action<OpenApiDocument> configurator)
        =>
        swaggerBuilder.Use(configurator);

    OpenApiDocument ISwaggerBuilder.Build()
        =>
        swaggerBuilder.Build();
}