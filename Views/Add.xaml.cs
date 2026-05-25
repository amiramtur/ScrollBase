using ScrollBase.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrollBase.Views;

public partial class Add : ContentPage
{
    public Add(ManageWebPagesVM viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;    // binding to the UI
    }

    private async void LoadHome(object sender, EventArgs e)
    {
        var mainPage = Handler.MauiContext.Services.GetService<MainPage>();

        await App.Current.MainPage.Navigation.PushAsync(mainPage);
    }

    private async void LoadSaved(object sender, EventArgs e)
    {
        ContentPage p = new Saved();
        await App.Current.MainPage.Navigation.PushAsync(p);
    }
}