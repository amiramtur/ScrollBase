using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Firebase.Database;
using Firebase.Database.Query;
using Microsoft.Maui.Networking;
using ScrollBase.Models;
using ScrollBase.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrollBase.ViewModels
{
    public partial class AppUserListVM : ObservableObject
    {
        private readonly FirebaseClient _client;
        private readonly DetailsPageVM _detailsVm;
        public AppUserListVM(FirebaseClient client, DetailsPageVM detailsVm)
        {
            _client = client;
            _detailsVm = detailsVm;
        }

        [ObservableProperty]
        ObservableCollection<AppUser> appuserlist = new();

        [RelayCommand]
        public async Task LoadData()
        {

            if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
            {
                await Shell.Current.DisplayAlert("error", "no internet", "ok");
                return;
            }

            var result = _client.Child("AppUser").AsObservable<AppUser>().Subscribe((item) =>
            {
                if (item.Object != null)
                {
                    item.Object.Id = item.Key; // saving id of each appuser for future changing / deletion
                    appuserlist.Add(item.Object);
                }
            });

        }

        [RelayCommand]
        public async Task DeleteAppUser(string Id)
        {
            //var result = await Shell.Current.DisplayAlert("Confirm", "are you sure want to delete?", "Ok", "Cancel");
            //if (result)
            //{
            await _client.Child($"AppUser/{Id}").DeleteAsync();
            await LoadData();
            //  }
        }

        [RelayCommand]
        public async Task ShowDetails(AppUser appUser)
        {
            //await Shell.Current.GoToAsync(nameof(DetailsPage), true,
            //    new Dictionary<string, object> { { "AppUser", appUser }, { "isEdit", true } });
            _detailsVm.Appuser = appUser;
            _detailsVm.IsEdit = true;
            await Shell.Current.GoToAsync(nameof(DetailsPage));
        }
    }
}
