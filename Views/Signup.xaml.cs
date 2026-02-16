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
        Console.WriteLine("a");
        //ContentPage p = new Login();
       // await App.Current.MainPage.Navigation.PushAsync(p);
    }
}