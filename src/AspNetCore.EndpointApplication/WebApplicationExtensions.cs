using System;

namespace Microsoft.AspNetCore.Builder;

public static class WebApplicationExtensions
{
    public static EndpointApplication BuildEndpointApplication(this WebApplicationBuilder webApplicationBuilder)
    {
        ArgumentNullException.ThrowIfNull(webApplicationBuilder);
        return EndpointApplication.InternalFromWebApplicationBuilder(webApplicationBuilder);
    }
}