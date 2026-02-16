using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Firebase.Auth;
using Microsoft.Maui.Networking;
using System;
using System.Threading.Tasks;

namespace ScrollBase.ViewModels
{
    public partial class LoginVM : ObservableObject
    {
        private readonly FirebaseAuthClient _client;

        [ObservableProperty]
        private string? _email;

        [ObservableProperty]
        private string? _password;

        public LoginVM(FirebaseAuthClient client)
        {
            _client = client;
        }

        [RelayCommand]
        private async Task Login()
        {
            if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
            {
                await Shell.Current.DisplayAlert("Error", "No internet connection", "OK");
                return;
            }

            try
            {
                // attempt sign in and await the result
                var result = await _client.SignInWithEmailAndPasswordAsync(Email!, Password!);

                await Shell.Current.DisplayAlert("Login", "Login success.", "OK");
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
