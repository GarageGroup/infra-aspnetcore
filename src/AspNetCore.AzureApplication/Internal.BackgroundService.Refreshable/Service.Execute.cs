using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
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
        IOperationHolder<RequestTelemetry>? operation = null;

        try
        {
            await Task.Delay(OperationTimeout, cancellationToken);

            operation = telemetryClient?.StartOperation<RequestTelemetry>("RefreshTokenCredentials");
            _ = operation?.Telemetry.Properties.TryAdd("Service", nameof(RefreshableTokenCredentialBackgroundService));

            logger.LogInformation("Refresh token credentials operation is running...");
            await tokenCredential.RefreshTokensAsync(cancellationToken);

            operation?.Telemetry.Success = true;
            operation?.Telemetry.ResponseCode = "200";

            logger.LogInformation("Refresh token credentials operation has finished successfully.");
        }
        catch (OperationCanceledException ex)
        {
            operation?.Telemetry.Success = false;
            operation?.Telemetry.ResponseCode = "499";

            logger.LogInformation(ex, "Refresh token credentials operation was canceled.");
        }
        catch (Exception ex)
        {
            operation?.Telemetry.Success = false;
            operation?.Telemetry.ResponseCode = "500";

            telemetryClient?.TrackException(ex);

            logger.LogError(ex, "Refresh token credentials operation has failed.");
        }
        finally
        {
            operation?.Dispose();
        }
    }
}