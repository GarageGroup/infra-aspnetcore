using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace GGroupp.Infra;

public sealed partial class EndpointResult<T>
{
    public Task ExecuteAsync(HttpContext httpContext)
    {
        _ = httpContext ?? throw new ArgumentNullException(nameof(httpContext));

        return result.MapFailure(httpContext.LogFailure).FoldValueAsync(WriteSuccessAsync, WriteFailureAsync).AsTask();

        ValueTask<Unit> WriteSuccessAsync(EndpointOut<T> success)
            =>
            httpContext.Response.WriteSuccessAsync(success, httpContext.RequestAborted);

        ValueTask<Unit> WriteFailureAsync(EndpointFailure failure)
            =>
            httpContext.Response.WriteFailureAsync(failure, httpContext.RequestAborted);
    }
}