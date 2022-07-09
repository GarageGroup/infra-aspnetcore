using System;
using Microsoft.AspNetCore.Http;

namespace GGroupp.Infra;

public sealed partial class EndpointResult<T> : IResult
{
    private readonly Result<EndpointOut<T>, EndpointFailure> result;

    public EndpointResult(Result<EndpointOut<T>, EndpointFailure> result)
        =>
        this.result = result;
}