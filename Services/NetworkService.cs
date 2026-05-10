using ScrollBase.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrollBase.Services
{
    public class NetworkService
    {
        private const string ConnectionSuccess = "Connected successfully";
        private const string ConnectionFail = "No internet connection after all attempts";

        private readonly IConnectivity _connectivity;

        public NetworkService(IConnectivity connectivity) => _connectivity = connectivity;

        public bool IsConnected =>
            _connectivity.NetworkAccess == NetworkAccess.Internet;

        /// <summary>
        /// Polls for a connection, reporting a live countdown on each tick.
        /// Returns true as soon as a connection is found, false if all attempts fail.
        /// </summary>
        public async Task<bool> WaitForConnectionAsync(
            int maxAttempts = 5,
            int secondsBetweenChecks = 5,
            IProgress<NetworkRetryStatus>? progress = null,
            CancellationToken ct = default)
        {
            for (int attempt = 1; attempt <= maxAttempts; attempt++)
            {
                if (IsConnected)
                {
                    progress?.Report(new NetworkRetryStatus(
                        ConnectionSuccess, attempt, maxAttempts, 0,
                        IsConnected: true, GaveUp: false));
                    return true;
                }

                for (int left = secondsBetweenChecks; left > 0; left--)
                {
                    ct.ThrowIfCancellationRequested();

                    progress?.Report(new NetworkRetryStatus(
                        $"No connection — retrying in {left}s (attempt {attempt}/{maxAttempts})",
                        attempt, maxAttempts, SecondsRemaining: left,
                        IsConnected: false, GaveUp: false));

                    await Task.Delay(1000, ct);
                }
            }

            progress?.Report(new NetworkRetryStatus(
                ConnectionFail, maxAttempts, maxAttempts, 0,
                IsConnected: false, GaveUp: true));

            return false;
        }
    }

    public record NetworkRetryStatus(
        string Message,
        int Attempt,
        int MaxAttempts,
        int SecondsRemaining,
        bool IsConnected,
        bool GaveUp
    );
}
