using System;

namespace GGroupp.Infra;

public static partial class FailureExtensions
{
    public static EndpointFailure ToEndpointFailure<TFailureCode>(
        this Failure<TFailureCode> failure,
        Func<TFailureCode, FailureStatusCode> mapFailureCode,
        Func<TFailureCode, string>? userDetailFactory = null)
        where TFailureCode : struct
    {
        _ = mapFailureCode ?? throw new ArgumentNullException(nameof(mapFailureCode));

        var userDetail = userDetailFactory is not null ? userDetailFactory.Invoke(failure.FailureCode) : failure.FailureMessage;

        return new(
            status: mapFailureCode.Invoke(failure.FailureCode),
            failureMessage: failure.FailureMessage,
            userDetail: userDetail);
    }
}