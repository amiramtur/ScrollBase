namespace ScrollBase.Views;

using Firebase.Auth;
using ScrollBase.ViewModels;

public partial class Login : ContentPage
{
    //private readonly FirebaseAuthClient _client;
	//public Login(FirebaseAuthClient client)
	public Login(LoginVM vm)
	{
		InitializeComponent();
        //BindingContext = new LoginVM(_client);
        BindingContext = vm;
	}

    private async void LoadSignup(object sender, EventArgs e)
    {
        var signupPage = Handler.MauiContext.Services.GetService<Signup>();

        await Application.Current.MainPage.Navigation.PushAsync(signupPage);
    }
}