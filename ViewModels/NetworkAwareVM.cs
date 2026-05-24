using CommunityToolkit.Mvvm.ComponentModel;
using ScrollBase.Services;
using System.Threading.Tasks;

namespace ScrollBase.ViewModels
{
    public abstract class NetworkAwareViewModel : ObservableObject
    {
        protected readonly NetworkService _network;

        private string _networkStatus = string.Empty;
        public string NetworkStatus
        {
            get => _networkStatus;
            set { _networkStatus = value; OnPropertyChanged(); }
        }

        private bool _isRetrying;
        public bool IsRetrying
        {
            get => _isRetrying;
            set { _isRetrying = value; OnPropertyChanged(); }
        }

        protected NetworkAwareViewModel(NetworkService network) => _network = network;

        // this function is called whenever a connection check is necessary
        // it returns false if failed to connect
        protected async Task<bool> EnsureConnectedAsync(CancellationToken ct = default)
        {
            var progress = new Progress<NetworkRetryStatus>(status =>
            {
                NetworkStatus = status.Message;
                IsRetrying = !status.IsConnected && !status.GaveUp;
            });

            // timer utility
            bool connected = await _network.WaitForConnectionAsync(
                maxAttempts: 5, secondsBetweenChecks: 5, progress: progress, ct: ct);

            IsRetrying = false;
            NetworkStatus = string.Empty;

            if (!connected)
                await Application.Current.MainPage.DisplayAlert("Error", "No internet connection", "OK");

            return connected;
        }
    }
}
