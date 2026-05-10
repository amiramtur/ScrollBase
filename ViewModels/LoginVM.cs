using Android.Net;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Firebase.Auth;
using Microsoft.Maui.Networking;
using System;
using System.Threading.Tasks;
using ScrollBase.Services;

namespace ScrollBase.ViewModels
{
    public partial class LoginVM : NetworkAwareViewModel
    {
        private readonly FirebaseAuthClient _client;

        [ObservableProperty]
        private string? _email;

        [ObservableProperty]
        private string? _password;

        public LoginVM(FirebaseAuthClient client, NetworkService network) : base(network)
        {
            _client = client;
        }

        [RelayCommand]
        private async Task Login()
        {
            // call the base class method
            bool hasInternet = await EnsureConnectedAsync();

            // if it tried 5 times and still failed, stop the signup process.
            // (EnsureConnectedAsync already shows the error alert for you, so we just return).
            if (!hasInternet)
            {
                return;
            }

            try
            {
                // attempt sign in and await the result
                // 1. Firebase login
                var result = await _client.SignInWithEmailAndPasswordAsync(Email!, Password!);

                // 2. Hop onto the Main UI Thread to update the screen
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    if (Application.Current?.MainPage != null)
                    {
                        await Application.Current.MainPage.DisplayAlert("Login", "Login success.", "OK");

                        // 3. Swap the entire app over Flyout Menu
                        Application.Current.MainPage = new AppShell();
                    }
                });
            }
            catch (FirebaseAuthException fae)
            {
                // Firebase-specific errors (invalid credentials, user not found, etc.)
                await Shell.Current.DisplayAlert("Login failed", fae.Reason.ToString(), "OK");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Login failed", ex.Message, "OK");
            }
        }
    }
}
