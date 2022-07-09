using System;

namespace GGroupp.Infra;

public static partial class EndpointResult
{
    public static EndpointResult<T> From<T>(Result<EndpointOut<T>, EndpointFailure> result)
        =>
        new(result);
}