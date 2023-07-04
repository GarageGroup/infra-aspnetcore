using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GarageGroup.Infra;

public static class ControllerResult
{
    public static ActionResult<T> ToActionResult<T>(this Result<EndpointOut<T>, EndpointFailure> result)
        =>
        result.Fold(ToSuccessResult, ToFailureResult<T>);

    internal static int GetFailureStatusCode(this FailureStatusCode statusCode)
        =>
        statusCode is not FailureStatusCode.InternalServerError ? (int)statusCode : StatusCodes.Status500InternalServerError;

    private static ActionResult<T> ToSuccessResult<T>(EndpointOut<T> endpointOut)
        =>
        endpointOut.Status switch
        {
            SuccessStatusCode.Ok => new ActionResult<T>(endpointOut.Body),
            SuccessStatusCode.NoContent => new NoContentResult(),
            _ => new ObjectResult(endpointOut.Body)
            {
                StatusCode = (int)endpointOut.Status
            }
        };

    private static ActionResult<T> ToFailureResult<T>(EndpointFailure failure)
        =>
        failure.ToProblemDetails().ToFailureResult<T>();

    private static ProblemDetails ToProblemDetails(this EndpointFailure failure)
        =>
        new()
        {
            Type = "about:blank",
            Title = Enum.GetName(failure.Status),
            Status = failure.Status.GetFailureStatusCode(),
            Detail = failure.UserDetail
        };

    private static ActionResult<T> ToFailureResult<T>(this ProblemDetails problemDetails)
        =>
        new ObjectResult(problemDetails)
        {
            StatusCode = problemDetails.Status
        };
}