namespace ScrollBase.Views;

public partial class Add : ContentPage
{
	public Add()
	{
		InitializeComponent();
	}

    private async void LoadHome(object sender, EventArgs e)
    {
        ContentPage p = new MainPage();
        await App.Current.MainPage.Navigation.PushAsync(p);
    }

    private async void LoadSaved(object sender, EventArgs e)
    {
        ContentPage p = new Saved();
        await App.Current.MainPage.Navigation.PushAsync(p);
    }
}