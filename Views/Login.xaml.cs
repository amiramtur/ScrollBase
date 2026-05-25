namespace ScrollBase.Views;

using Firebase.Auth;
using ScrollBase.ViewModels;

public partial class Login : ContentPage
{
	public Login(LoginVM vm)
	{
		InitializeComponent();
        BindingContext = vm;
	}
}