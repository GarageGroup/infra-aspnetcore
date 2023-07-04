namespace GarageGroup.Infra;

public readonly record struct EndpointOut<T>
{
    public EndpointOut(SuccessStatusCode status, T body)
    {
        Status = status;
        Body = body;
    }

    public SuccessStatusCode Status { get; }

    public T Body { get; }
}