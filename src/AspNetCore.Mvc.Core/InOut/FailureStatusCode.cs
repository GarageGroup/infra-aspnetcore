namespace GarageGroup.Infra;

public enum FailureStatusCode
{
    InternalServerError,

    BadRequest = 400,

    Unauthorized = 401,

    PaymentRequired = 402,

    Forbidden = 403,

    NotFound = 404,

    NotAcceptable = 406,

    RequestTimeout = 408,

    Conflict = 409,

    Gone = 410,

    LengthRequired = 411,

    PreconditionFailed = 412,

    RequestedRangeNotSatisfiable = 416,

    ExpectationFailed = 417,

    UnprocessableEntity = 422,

    Locked = 423,

    TooManyRequests = 429
}