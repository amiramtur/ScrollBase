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

        // Tracks if the switch is toggled
        [ObservableProperty]
        bool isRemoveMode;

        // The list that the Picker reads from (initialized so it's not null!)
        [ObservableProperty]
        ObservableCollection<SavedPageModel> savedPagesList = new();

        // The specific item the user clicks on inside the Picker
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
            // call the base class method
            bool hasInternet = await EnsureConnectedAsync();

            // if it tried 5 times and still failed, stop the signup process.
            // (EnsureConnectedAsync already shows the error alert for you, so we just return).
            if (!hasInternet)
            {
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
            // call the base class method
            bool hasInternet = await EnsureConnectedAsync();

            // if it tried 5 times and still failed, stop the signup process.
            // (EnsureConnectedAsync already shows the error alert for you, so we just return).
            if (!hasInternet)
            {
                return;
            }

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
                    // Using Application.Current.MainPage just in case Shell is detached!
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
                    // 1. Fetch all pages for this specific user to find the matching Firebase Key
                    var allUserPages = await _client
                        .Child("SavedPages")
                        .Child(uid)
                        .OnceAsync<SavedPageModel>();

                    // 2. Find the exact Firebase object that matches what the user selected in the Picker
                    var pageToDelete = allUserPages.FirstOrDefault(p =>
                        p.Object.PageName == SelectedPageToRemove.PageName &&
                        p.Object.PageLink == SelectedPageToRemove.PageLink);

                    if (pageToDelete != null)
                    {
                        // 3. We found the key (e.g. "-Om4LtV9faWXPDF4QReN")! Now tell Firebase to nuke it.
                        await _client
                            .Child("SavedPages")
                            .Child(uid)
                            .Child(pageToDelete.Key)
                            .DeleteAsync();

                        // 4. Remove it from your local ObservableCollection so it instantly vanishes from the UI Picker
                        SavedPagesList.Remove(SelectedPageToRemove);

                        // Clear the selection
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
                // 1. Get the current logged-in user
                var user = _authClient?.User;
                if (user == null || string.IsNullOrEmpty(user.Uid)) return;

                // 2. Fetch their saved pages from Firebase
                var pages = await _client
                    .Child("SavedPages")
                    .Child(user.Uid)
                    .OnceAsync<SavedPageModel>();

                // 3. Clear the old list and fill it with the newly fetched pages
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

        // This special method name is recognized by the MVVM Toolkit. 
        // It fires automatically whenever the switch is toggled!
        partial void OnIsRemoveModeChanged(bool value)
        {
            // 'value' is the new state of the switch.
            // True = Remove Mode, False = Add Mode
            if (value == true)
            {
                // The switch was just flipped to Remove!
                // We use the discard operator (_) to safely fire-and-forget the async task 
                // without holding up the UI thread.
                _ = LoadUserPagesAsync();
            }
            else
            {
                // Optional: Clear the list when they switch back to "Add" to save memory
                SavedPagesList.Clear();
            }
        }
    }
}
