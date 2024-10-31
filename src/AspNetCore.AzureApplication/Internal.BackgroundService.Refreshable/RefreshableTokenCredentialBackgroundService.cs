using System;
using Azure.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace GarageGroup.Infra;

internal sealed partial class RefreshableTokenCredentialBackgroundService : BackgroundService
{
    public static RefreshableTokenCredentialBackgroundService Resolve(IServiceProvider serviceProvider)
    {
        ArgumentNullException.ThrowIfNull(serviceProvider);

        return new(
            tokenCredential: serviceProvider.GetService<TokenCredential>(),
            logger: serviceProvider.GetRequiredService<ILoggerFactory>().CreateLogger<RefreshableTokenCredentialBackgroundService>());
    }

    private static readonly TimeSpan OperationTimeout
        =
        TimeSpan.FromMinutes(30);

    private static readonly TimeSpan CancellationTimeout
        =
        TimeSpan.FromMinutes(10);

    private readonly ITokensRefreshSupplier? tokenCredential;

    private readonly ILogger logger;

    private RefreshableTokenCredentialBackgroundService(
        TokenCredential? tokenCredential, ILogger<RefreshableTokenCredentialBackgroundService> logger)
    {
        this.tokenCredential = tokenCredential as ITokensRefreshSupplier;
        this.logger = logger;
    }
}