using System;

namespace Microsoft.AspNetCore.Builder;

public static class WebApplicationExtensions
{
    public static EndpointApplication BuildEndpointApplication(this WebApplicationBuilder webApplicationBuilder)
        =>
        EndpointApplication.InternalFromWebApplicationBuilder(
            webApplicationBuilder ?? throw new ArgumentNullException(nameof(webApplicationBuilder)));
}