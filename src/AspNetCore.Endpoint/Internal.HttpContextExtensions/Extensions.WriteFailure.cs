using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GGroupp.Infra;

partial class HttpContextExtensions
{
    internal static ValueTask<Unit> WriteFailureAsync(this HttpResponse response, EndpointFailure failure, CancellationToken cancellationToken)
    {
        response.StatusCode = (int)failure.Status;
        response.ContentType = "application/problem+json";

        var detail = new ProblemDetails
        {
            Type = "about:blank",
            Title = Enum.GetName(failure.Status),
            Status = response.StatusCode,
            Detail = failure.UserDetail
        };

        return Unit.InvokeAsync(response.WriteAsJsonAsync, detail, cancellationToken).ToValueTask();
    }
}