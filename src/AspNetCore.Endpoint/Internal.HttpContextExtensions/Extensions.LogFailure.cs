using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace GGroupp.Infra;

partial class HttpContextExtensions
{
    internal static EndpointFailure LogFailure(this HttpContext context, EndpointFailure failure)
        =>
        failure.Status switch
        {
            FailureStatusCode.InternalServerError => context.LogError(failure),
            _ => context.LogInfo(failure)
        };

    private static EndpointFailure LogInfo(this HttpContext context, EndpointFailure failure)
    {
        if (string.IsNullOrEmpty(failure.FailureMessage))
        {
            return failure;
        }

        context.GetLogger().LogInformation("A failure occurred: {status} {message}", failure.Status, failure.FailureMessage);
        return failure;
    }

    private static EndpointFailure LogError(this HttpContext context, EndpointFailure failure)
    {
        if (string.IsNullOrEmpty(failure.FailureMessage))
        {
            context.GetLogger().LogError("An unexpected error occurred");
        }
        else
        {
            context.GetLogger().LogError("An unexpected error occurred: {status} {message}", failure.Status, failure.FailureMessage);
        }

        return failure;
    }

    private static ILogger GetLogger(this HttpContext context)
        =>
        context.RequestServices.GetRequiredService<ILoggerFactory>().CreateLogger("HttpEndpoint");
}