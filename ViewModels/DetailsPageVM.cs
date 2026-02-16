using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Firebase.Database;
using Firebase.Database.Query;
using ScrollBase.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrollBase.ViewModels
{
    [QueryProperty(nameof(AppUser), "AppUser")]
    public partial class DetailsPageVM(FirebaseClient fc) : ObservableObject
    {
        private readonly FirebaseClient _client = fc;
        [ObservableProperty]
        AppUser appuser = new();

        [ObservableProperty]
        bool isEdit;

        [ObservableProperty]
        string failError;

        [RelayCommand]
        public async Task SaveAndUpdate(IConnectivity connectivity)
        {
            //TryConnect(connectivity);

            await _client.Child($"AppUser/{appuser.Id}").PutAsync(appuser);
            isEdit = false;
            failError = "update success";
            await Shell.Current.DisplayAlert("info", failError, "ok");
            await Shell.Current.GoToAsync("..");
        }


    }
}
