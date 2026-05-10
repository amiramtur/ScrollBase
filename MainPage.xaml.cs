using Microsoft.Maui.Controls;
using ScrollBase.ViewModels;

namespace ScrollBase
{
    public partial class MainPage : ContentPage
    {
        private readonly MainPageVM _viewModel;

        public MainPage(MainPageVM viewModel)
        {
            InitializeComponent();

            // Set the BindingContext to our injected ViewModel
            _viewModel = viewModel;
            BindingContext = _viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            if (_viewModel != null)
            {
                await _viewModel.LoadUserFeedAsync();
            }
        }
    }
}