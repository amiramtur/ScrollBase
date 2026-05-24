using Firebase.Auth;
using Firebase.Database;
using Firebase.Database.Query;
using ScrollBase.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq; // for the filter query
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace ScrollBase.ViewModels
{
    public partial class MainPageVM : INotifyPropertyChanged
    {
        private readonly FirebaseClient _firebaseClient;
        private readonly FirebaseAuthClient _authClient;

        // a master list to keep the original data safe when we filter (search query)
        private List<SavedPageModel> _allPages = new();

        public ObservableCollection<SavedPageModel> WebContainers { get; set; } = new();

        // the search query property linked to the XAML SearchBar
        private string _searchQuery;
        public string SearchQuery
        {
            get => _searchQuery;
            set
            {
                if (_searchQuery != value)
                {
                    _searchQuery = value;
                    OnPropertyChanged();
                    // instantly trigger the filter whenever the text is typed
                    FilterPages();
                }
            }
        }

        public MainPageVM(FirebaseClient firebaseClient, FirebaseAuthClient authClient)
        {
            _firebaseClient = firebaseClient;
            _authClient = authClient;
        }

        public async Task LoadUserFeedAsync()
        {
            try
            {
                var user = _authClient?.User;
                if (user == null || string.IsNullOrEmpty(user.Uid))
                    return;

                var uid = user.Uid;

                var savedPagesData = await _firebaseClient
                    .Child("SavedPages")
                    .Child(uid)
                    .OnceAsync<SavedPageModel>();

                WebContainers.Clear();
                _allPages.Clear(); // clear the master list on fresh load

                if (savedPagesData != null)
                {
                    foreach (var item in savedPagesData)
                    {
                        if (item?.Object != null)
                        {
                            WebContainers.Add(item.Object);
                            _allPages.Add(item.Object); // backing each page up
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"FIREBASE CRASH: {ex.Message}");
            }
        }

        // the Filter Logic
        private void FilterPages()
        {
            // if the search bar is empty, show all the pages again
            if (string.IsNullOrWhiteSpace(SearchQuery))
            {
                WebContainers.Clear();
                foreach (var item in _allPages)
                {
                    WebContainers.Add(item);
                }
            }
            else
            {
                // filter the list based on what's typed (ignoring uppercase/lowercase)
                var filtered = _allPages.Where(p =>
                    p.PageName != null &&
                    p.PageName.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase)).ToList();

                WebContainers.Clear();
                foreach (var item in filtered)
                {
                    WebContainers.Add(item);
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}