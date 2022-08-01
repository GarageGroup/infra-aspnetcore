namespace Microsoft.AspNetCore.Builder;

public static class EndpointApplicationBuilderExtensions
{
    public static TApplicationBuilder UseHttpsRedirection<TApplicationBuilder>(this TApplicationBuilder applicationBuilder!!)
        where TApplicationBuilder : class, IApplicationBuilder
    {
        _ = HttpsPolicyBuilderExtensions.UseHttpsRedirection(applicationBuilder);
        return applicationBuilder;
    }
}