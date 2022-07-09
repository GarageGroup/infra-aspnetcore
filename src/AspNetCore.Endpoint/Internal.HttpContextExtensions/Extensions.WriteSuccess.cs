using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace GGroupp.Infra;

partial class HttpContextExtensions
{
    internal static ValueTask<Unit> WriteSuccessAsync<T>(this HttpResponse response, EndpointOut<T> value, CancellationToken cancellationToken)
    {
        response.StatusCode = (int)value.Status;

        if (value.Status is SuccessStatusCode.NoContent)
        {
            return default;
        }

        return Unit.InvokeAsync(response.WriteAsJsonAsync, value.Body, cancellationToken).ToValueTask();
    }
}