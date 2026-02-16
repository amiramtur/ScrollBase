namespace ScrollBase.Views;

public partial class Saved : ContentPage
{
	public Saved()
	{
		InitializeComponent();
	}

    private async void LoadHome(object sender, EventArgs e)
    {
        ContentPage p = new MainPage();
        await App.Current.MainPage.Navigation.PushAsync(p);
    }

    private async void LoadAdd(object sender, EventArgs e)
    {
        ContentPage p = new Add();
        await App.Current.MainPage.Navigation.PushAsync(p);
    }
}