using ScrollBase.ViewModels;

namespace ScrollBase.Views;

public partial class Signup : ContentPage
{
	public Signup(SignupVM vm)
	{
		InitializeComponent();
        BindingContext = vm;
	}

    private async void LoadLogin(object sender, EventArgs e)
    {
        var loginPage = Handler.MauiContext.Services.GetService<Login>();

        await Application.Current.MainPage.Navigation.PushAsync(loginPage);
    }
}