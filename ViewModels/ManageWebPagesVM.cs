using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Database.Query;
using Microsoft.Maui.Networking;
using ScrollBase.Models;
using ScrollBase.Services;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace ScrollBase.ViewModels
{
    public partial class ManageWebPagesVM : NetworkAwareViewModel
    {
        private readonly FirebaseClient _client;
        private readonly FirebaseAuthClient _authClient;


        [ObservableProperty]
        private string? _name;

        [ObservableProperty]
        private string? _link;

        // tracks if the switch is toggled
        [ObservableProperty]
        bool isRemoveMode;

        // the list that the picker in remove page reads from
        [ObservableProperty]
        ObservableCollection<SavedPageModel> savedPagesList = new();

        // the specific page the user picks
        [ObservableProperty]
        SavedPageModel selectedPageToRemove;

        public ManageWebPagesVM(FirebaseClient client, FirebaseAuthClient authClient, NetworkService network) : base(network)
        {
            _client = client;
            _authClient = authClient;
        }

        [RelayCommand]
        private async Task AddPage()
        {
            // network ----------------------------------------------

            // call the base class method
            bool hasInternet = await EnsureConnectedAsync();

            // if it tried 5 times and still failed, stop the signup process.
            // (EnsureConnectedAsync already shows the error alert for you, so we just return).
            if (!hasInternet)
            {
                return;
            }

            // network ----------------------------------------------

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
                await _client
                    .Child("SavedPages")
                    .Child(uid)
                    .PostAsync(new SavedPageModel
                    {
                        PageName = Name,
                        PageLink = Link
                    });

                await Shell.Current.DisplayAlert("Success", "Page saved.", "OK");

                Name = string.Empty;
                Link = string.Empty;
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Failed to save page: {ex.Message}", "OK");
            }
        }

        [RelayCommand]
        private async Task RemovePage()
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

            if (SelectedPageToRemove == null)
            {
                await Shell.Current.DisplayAlert("Error", "No page picked", "OK");
                return;
            }

            bool confirm = await Application.Current.MainPage.DisplayAlert(
                "Confirm",
                $"Are you sure you want to delete {SelectedPageToRemove.PageName}?",
                "Yes", "No");

            if (confirm)
            {
                var uid = _authClient?.User?.Uid;
                if (string.IsNullOrEmpty(uid))
                {
                    // using Application.Current.MainPage in case Shell is detached
                    await Application.Current.MainPage.DisplayAlert("Error", "No authenticated user found.", "OK");
                    return;
                }

                if (SelectedPageToRemove == null)
                {
                    await Application.Current.MainPage.DisplayAlert("Warning", "Please select a page to remove first.", "OK");
                    return;
                }

                try
                {
                    // fetching all pages for the currently connected user to find the matching FireBase Key
                    var allUserPages = await _client
                        .Child("SavedPages")
                        .Child(uid)
                        .OnceAsync<SavedPageModel>();

                    // find the exact Firebase object that matches the page picked by the user
                    var pageToDelete = allUserPages.FirstOrDefault(p =>
                        p.Object.PageName == SelectedPageToRemove.PageName &&
                        p.Object.PageLink == SelectedPageToRemove.PageLink);

                    if (pageToDelete != null)
                    {
                        // removing the key (the page) from FireBase
                        await _client
                            .Child("SavedPages")
                            .Child(uid)
                            .Child(pageToDelete.Key)
                            .DeleteAsync();

                        // removing it from local ObservableCollection so it vanishes from the UI picker
                        SavedPagesList.Remove(SelectedPageToRemove);

                        SelectedPageToRemove = null;

                        await Application.Current.MainPage.DisplayAlert("Success", "Page removed successfully.", "OK");
                    }
                    else
                    {
                        await Application.Current.MainPage.DisplayAlert("Error", "Could not find that page in the database.", "OK");
                    }
                }
                catch (Exception ex)
                {
                    await Application.Current.MainPage.DisplayAlert("Error", $"Failed to remove page: {ex.Message}", "OK");
                }
            }
        }

        [RelayCommand]
        private async Task LoadUserPagesAsync()
        {
            try
            {
                // getting the current connected user
                var user = _authClient?.User;
                if (user == null || string.IsNullOrEmpty(user.Uid)) return;

                // getting their saved pages from Firebase
                var pages = await _client
                    .Child("SavedPages")
                    .Child(user.Uid)
                    .OnceAsync<SavedPageModel>();

                SavedPagesList.Clear();
                foreach (var item in pages)
                {
                    if (item?.Object != null)
                    {
                        SavedPagesList.Add(item.Object);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching pages: {ex.Message}");
            }
        }

        // special MVVM method:
        // activates automatically whenever the switch is toggled
        partial void OnIsRemoveModeChanged(bool value)
        {
            // 'value' is the new state of the switch.
            // true = remove, false = add
            if (value == true)
            {
                // using the discard operator (_) to safely "forget" the async task without holding up the UI thread.
                _ = LoadUserPagesAsync();
            }
            else
            {
                // clearing the list when they switch back to "add" to save memory
                SavedPagesList.Clear();
            }
        }
    }
}
