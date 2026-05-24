using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Database.Query;
using ScrollBase.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScrollBase.Services;

namespace ScrollBase.ViewModels
{
    public partial class SignupVM : NetworkAwareViewModel
    {
        private readonly FirebaseClient _client;
        private readonly FirebaseAuthClient _authClient;

        [ObservableProperty]
        private string? _email;

        [ObservableProperty]
        private string? _password;

        public SignupVM(FirebaseClient client, FirebaseAuthClient authClient, NetworkService network) : base(network)
        {
            _client = client;
            _authClient = authClient;
        }

        [RelayCommand]
        private async Task Signup()
        {
            // network ----------------------------------------------

            bool hasInternet = await EnsureConnectedAsync();

            // if it tried 5 times and still failed, stop the signup process.
            // (EnsureConnectedAsync already shows the error alert for you, so we just return).
            if (!hasInternet)
            {
                return;
            }

            // network ----------------------------------------------

            // service input check
            if (!InputCheck.IsEmailValid(Email))
            {
                await Application.Current.MainPage.DisplayAlert("Invalid Email", "Ensure your input contains legal characters (Aa-Zz 1-9) and is at least 6 characters long", "OK");
                return;
            }
            else if (!InputCheck.IsPasswordValid(Password))
            {
                await Application.Current.MainPage.DisplayAlert("Invalid Password", "Ensure your input contains legal characters (Aa-Zz 1-9) and is at least 6 characters long", "OK");
                return;
            }

            try
            {
                // firebase signup
                var result = await _authClient.CreateUserWithEmailAndPasswordAsync(Email!, Password!);

                // updating screen with mainpage UI
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    if (Application.Current?.MainPage != null)
                    {
                        await _client.Child("AppUser").PostAsync(new AppUser
                        {
                            Id = _authClient.User.Uid,
                            Email = _email,
                            Password = _password
                        });

                        await Application.Current.MainPage.DisplayAlert("Signup", "Signup success.", "OK");

                        Application.Current.MainPage = new AppShell();
                    }
                });
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Signup failed", "Something went wrong", "OK");
            }
        }
    }
}
