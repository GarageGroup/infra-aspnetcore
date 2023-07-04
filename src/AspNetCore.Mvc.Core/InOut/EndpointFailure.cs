using System.Diagnostics.CodeAnalysis;

namespace GarageGroup.Infra;

public readonly record struct EndpointFailure
{
    public static EndpointFailure BadRequest([AllowNull] string userDetail)
        =>
        new(FailureStatusCode.BadRequest, null, userDetail);

    private readonly string? failureMessage;

    private readonly string? userDetail;

    public EndpointFailure(FailureStatusCode status, [AllowNull] string failureMessage, [AllowNull] string userDetail)
    {
        Status = status;
        this.failureMessage = string.IsNullOrEmpty(failureMessage) ? null : failureMessage;
        this.userDetail = string.IsNullOrEmpty(userDetail) ? null : userDetail;
    }

    public FailureStatusCode Status { get; }

    public string FailureMessage => failureMessage ?? string.Empty;

    public string UserDetail => userDetail ?? string.Empty;
}