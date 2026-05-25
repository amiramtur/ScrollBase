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

            // set the BindingContext to the injected VM
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