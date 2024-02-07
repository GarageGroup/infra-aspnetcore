using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net.Mime;
using System.Text.Json;
using System.Threading.Tasks;
using GarageGroup.Infra;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

namespace Microsoft.AspNetCore.Builder;

public static class ApiKeyAuthenticationMiddleware
{
    private static readonly JsonSerializerOptions SerializerOptions;

    static ApiKeyAuthenticationMiddleware()
        =>
        SerializerOptions = new(JsonSerializerDefaults.Web);

    private const int FailureStatusCode = 401;

    private const string FailureDescription = "Unauthorized";

    private const string ContentTypeHeaderName = "Content-Type";

    private const string ProblemSchemaId = "ProblemDetails";

    private const string SecuritySchemeKey = "apiKeyAuthentication";

    public static TApplicationBuilder UseApiKeyAuthentication<TApplicationBuilder>(
        this TApplicationBuilder applicationBuilder, string sectionName = "ApiKey", PathString path = default)
        where TApplicationBuilder : class, IApplicationBuilder
    {
        ArgumentNullException.ThrowIfNull(applicationBuilder);

        var section = applicationBuilder.ApplicationServices.GetRequiredService<IConfiguration>().GetRequiredSection(sectionName);

        var option = new ApiKeyAuthenticationOption(
            primaryKey: section["PrimaryKey"],
            secondaryKey: section["SecondaryKey"],
            headerName: section["HeaderName"],
            queryParameterName: section["QueryParameterName"])
        {
            IsDisabled = section.GetValue<bool>("IsDisabled")
        };

        return applicationBuilder.InnerUseApiKeyAuthentication(option, path);
    }

    public static TApplicationBuilder UseApiKeyAuthentication<TApplicationBuilder>(
        this TApplicationBuilder applicationBuilder, ApiKeyAuthenticationOption option, PathString path = default)
        where TApplicationBuilder : class, IApplicationBuilder
    {
        ArgumentNullException.ThrowIfNull(applicationBuilder);
        ArgumentNullException.ThrowIfNull(option);

        return applicationBuilder.InnerUseApiKeyAuthentication(option, path);
    }

    private static TApplicationBuilder InnerUseApiKeyAuthentication<TApplicationBuilder>(
        this TApplicationBuilder applicationBuilder, ApiKeyAuthenticationOption option, PathString pathMatch)
        where TApplicationBuilder : class, IApplicationBuilder
    {
        if (option.IsDisabled)
        {
            return applicationBuilder;
        }

        if (applicationBuilder is ISwaggerBuilder swaggerBuilder)
        {
            _ = swaggerBuilder.Use(InnerConfigureSwagger);
        }

        _ = applicationBuilder.Use(InnerInvokeAsync);
        return applicationBuilder;

        void InnerConfigureSwagger([AllowNull] OpenApiDocument openApiDocument)
            =>
            ConfigureSwagger(openApiDocument, option, pathMatch);

        Task InnerInvokeAsync(HttpContext context, Func<Task> nextAsync)
            =>
            context.VerifyApiKeyAsync(nextAsync, option, pathMatch);
    }

    private static void ConfigureSwagger([AllowNull] OpenApiDocument openApiDocument, ApiKeyAuthenticationOption option, PathString pathMatch)
    {
        if (openApiDocument?.Paths?.Count is not > 0)
        {
            return;
        }

        var paths = pathMatch.HasValue switch
        {
            false => openApiDocument.Paths.SelectMany(GetOperations).ToArray(),
            _ => openApiDocument.Paths.Where(IsPathMatched).SelectMany(GetOperations).ToArray()
        };

        if (paths.Length is 0)
        {
            return;
        }

        openApiDocument.Components ??= new();
        openApiDocument.Components.Schemas ??= new Dictionary<string, OpenApiSchema>();

        openApiDocument.Components.SecuritySchemes ??= new Dictionary<string, OpenApiSecurityScheme>();
        openApiDocument.Components.SecuritySchemes[SecuritySchemeKey] = new()
        {
            Name = option.HeaderName,
            Type = SecuritySchemeType.ApiKey,
            In = ParameterLocation.Header,
            Description = "API key authentication header"
        };

        var referenceScurityScheme = new OpenApiSecurityScheme
        {
            Reference = new()
            {
                Type = ReferenceType.SecurityScheme,
                Id = SecuritySchemeKey
            }
        };

        var securityRequirement = new OpenApiSecurityRequirement
        {
            [referenceScurityScheme] = []
        };

        if (openApiDocument.Components.Schemas.ContainsKey(ProblemSchemaId) is false)
        {
            openApiDocument.Components.Schemas[ProblemSchemaId] = CreateProblemSchema();
        }

        foreach (var path in paths)
        {
            path.Security ??= [];
            path.Security.Add(securityRequirement);

            path.Responses ??= [];

            if (path.Responses.ContainsKey(FailureStatusCode.ToString()))
            {
                continue;
            }

            path.Responses[FailureStatusCode.ToString()] = new()
            {
                Description = FailureDescription,
                Content = new Dictionary<string, OpenApiMediaType>
                {
                    [MediaTypeNames.Application.Json] = new()
                    {
                        Schema = new()
                        {
                            Nullable = false,
                            Reference = new()
                            {
                                Type = ReferenceType.Schema,
                                Id = ProblemSchemaId
                            }
                        }
                    }
                }
            };
        }

        bool IsPathMatched(KeyValuePair<string, OpenApiPathItem> item)
            =>
            item.Key.StartsWith(pathMatch, StringComparison.InvariantCultureIgnoreCase);

        static IEnumerable<OpenApiOperation> GetOperations(KeyValuePair<string, OpenApiPathItem> item)
            =>
            item.Value.Operations?.Select(GetValue) ?? [];

        static TValue GetValue<TKey, TValue>(KeyValuePair<TKey, TValue> pair)
            =>
            pair.Value;
    }

    private static Task VerifyApiKeyAsync(this HttpContext context, Func<Task> nextAsync, ApiKeyAuthenticationOption option, PathString pathMatch)
    {
        if (pathMatch.HasValue && context.Request.Path.StartsWithSegments(pathMatch, out _, out _) is false)
        {
            return nextAsync.Invoke();
        }

        var apiKey = context.GetApiKey(option);
        if (apiKey.IsApiKeyValid(option))
        {
            return nextAsync.Invoke();
        }

        var logger = context.RequestServices.GetRequiredService<ILoggerFactory>().CreateLogger("ApiKeyAuthenticationMiddleware");
        string failureMessage;

        if (string.IsNullOrWhiteSpace(apiKey))
        {
            failureMessage = "ApiKey must be specified";
            logger.LogInformation("ApiKey is absent");
        }
        else
        {
            failureMessage = "ApiKey is invalid";
            logger.LogInformation("ApiKey '{apiKey}' is invalid", apiKey);
        }

        context.Response.StatusCode = FailureStatusCode;

        context.Response.Headers.Remove(ContentTypeHeaderName);
        context.Response.Headers.Add(ContentTypeHeaderName, MediaTypeNames.Application.Json);

        var problem = new
        {
            Type = FailureDescription,
            Title = "about:blank",
            Status = FailureStatusCode,
            Detail = failureMessage
        };

        var failureJson = JsonSerializer.Serialize(problem, SerializerOptions);
        return context.Response.WriteAsync(failureJson, context.RequestAborted);
    }

    private static string? GetApiKey(this HttpContext context, ApiKeyAuthenticationOption option)
    {
        if (context.Request.Headers.TryGetValue(option.HeaderName, out var headerValue))
        {
            var headerKey = headerValue.ToString();
            if (string.IsNullOrWhiteSpace(headerKey) is false)
            {
                return headerKey;
            }
        }

        if (context.Request.Query.TryGetValue(option.QueryParameterName, out var queryValue))
        {
            return queryValue.ToString();
        }

        return null;
    }

    private static bool IsApiKeyValid(this string? apiKey, ApiKeyAuthenticationOption option)
    {
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            return false;
        }

        return string.Equals(apiKey, option.PrimaryKey, StringComparison.InvariantCulture) ||
            string.Equals(apiKey, option.SecondaryKey, StringComparison.InvariantCulture);
    }

    private static OpenApiSchema CreateProblemSchema()
        =>
        new()
        {
            Type = "object",
            Properties = new Dictionary<string, OpenApiSchema>
            {
                ["type"] = CreateStringSchema(),
                ["title"] = CreateStringSchema(),
                ["status"] = CreateInt32Schema(),
                ["detail"] = CreateStringSchema(),
                ["instance"] = CreateStringSchema()
            }
        };

    private static OpenApiSchema CreateStringSchema()
        =>
        new()
        {
            Type = "string",
            Nullable = true
        };

    private static OpenApiSchema CreateInt32Schema()
        =>
        new()
        {
            Type = "integer",
            Format = "int32",
            Nullable = true
        };
}
