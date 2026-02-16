using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Database.Query;
using Microsoft.Maui.Networking;
using ScrollBase.Models;
using ScrollBase.Services;
using System;
using System.Threading.Tasks;

namespace ScrollBase.ViewModels
{
    public partial class AddWebPageVM : ObservableObject
    {
        private readonly FirebaseClient _client;
        private readonly FirebaseAuthClient _authClient;

        [ObservableProperty]
        private string? _name;

        [ObservableProperty]
        private string? _link;

        public AddWebPageVM(FirebaseClient client, FirebaseAuthClient authClient)
        {
            _client = client;
            _authClient = authClient;
        }

        [RelayCommand]
        private async Task AddPage()
        {
            if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
            {
                await Shell.Current.DisplayAlert("Error", "No internet connection", "OK");
                return;
            }

            if (!InputCheck.IsLinkValid(Link))
            {
                await Shell.Current.DisplayAlert("Invalid Link", "Ensure your link is a valid http(s) URL", "OK");
                return;
            }

            var uid = _authClient.User.Uid;
            if (string.IsNullOrEmpty(uid))
            {
                await Shell.Current.DisplayAlert("Error", "No authenticated user found.", "OK");
                return;
            }

            try
            {
                await Shell.Current.DisplayAlert("Check", uid, "OK");
                await _client
                    .Child("AppUser")
                    .Child(uid)
                    .Child("Pages")
                    .PostAsync(new PageWindow
                    {
                        PageName = Name,
                        PageLink = Link
                    });

                await Shell.Current.DisplayAlert("Success", "Page saved.", "OK");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Failed to save page: {ex.Message}", "OK");
            }
        }
    }
}
