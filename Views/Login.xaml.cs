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
        int i = 0;
        //ContentPage p = new Signup();
        //await App.Current.MainPage.Navigation.PushAsync(p);
    }
}