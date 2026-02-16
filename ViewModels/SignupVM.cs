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
    public partial class SignupVM : ObservableObject
    {
        private readonly FirebaseClient _client;
        private readonly FirebaseAuthClient _authClient;

        // using partial properties for AOT compatibility (MVVMTK0045 fix)
        [ObservableProperty]
        private string? _email;

        [ObservableProperty]
        private string? _password;

        public SignupVM(FirebaseClient client, FirebaseAuthClient authClient)
        {
            _client = client;
            _authClient = authClient;
        }

        [RelayCommand]
        private async Task Signup()
        {
            if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
            {
                await Shell.Current.DisplayAlert("Error", "No internet connection", "OK");
                return;
            }

            // service input check
            if (!InputCheck.IsEmailValid(Email))
            {
                await Shell.Current.DisplayAlert("Invalid Email", "Ensure your input contains legal characters (Aa-Zz 1-9) and is at least 6 characters long", "OK");
                return;
            }

            else if (!InputCheck.IsPasswordValid(Password))
            {
                await Shell.Current.DisplayAlert("Invalid Password", "Ensure your input contains legal characters (Aa-Zz 1-9) and is at least 6 characters long", "OK");
                return;
            }

            try
            {
                // attempt sign up and await the result
                var result = await _authClient.CreateUserWithEmailAndPasswordAsync(Email!, Password!);
                await _client.Child("AppUser").PostAsync(new AppUser
                {
                    Id = _authClient.User.Uid,
                    Email = _email,
                    Password = _password
                });
                // result is an id that can be used for checks
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Signup failed", "Something went wrong", "OK");
            }
        }
    }
}
