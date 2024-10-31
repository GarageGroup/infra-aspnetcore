using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace GarageGroup.Infra;

partial class RefreshableTokenCredentialBackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (tokenCredential is null)
        {
            return;
        }

        logger.LogInformation("Refresh token credentials background service was started.");

        while (true)
        {
            if (stoppingToken.IsCancellationRequested)
            {
                logger.LogInformation("Refresh token credentials background service was stopped.");
                return;
            }

            using var cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken);
            cancellationTokenSource.CancelAfter(CancellationTimeout);

            await InnerExecuteAsync(tokenCredential, cancellationTokenSource.Token);
        }
    }

    private async Task InnerExecuteAsync(ITokensRefreshSupplier tokenCredential, CancellationToken cancellationToken)
    {
        try
        {
            await Task.Delay(OperationTimeout, cancellationToken);

            logger.LogInformation("Refresh token credentials operation is running...");
            await tokenCredential.RefreshTokensAsync(cancellationToken);
            logger.LogInformation("Refresh token credentials operation has finished successfully.");
        }
        catch (OperationCanceledException ex)
        {
            logger.LogWarning(ex, "Refresh token credentials operation was canceled.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Refresh token credentials operation has failed.");
        }
    }
}