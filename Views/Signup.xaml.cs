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

    private async void ToggleInfoBox(object sender, EventArgs e)
    {
        if (InfoBox.IsVisible)
        {
            // Fade out and shrink
            await Task.WhenAll(
                InfoBox.FadeTo(0, 200, Easing.CubicIn),
                InfoBox.ScaleTo(0.8, 200, Easing.CubicIn),
                BackgroundDimmer.FadeTo(0, 200)
            );

            InfoBox.IsVisible = false;
            BackgroundDimmer.IsVisible = false;
        }
        else
        {
            InfoBox.IsVisible = true;
            BackgroundDimmer.IsVisible = true;

            await Task.WhenAll(
                InfoBox.FadeTo(1, 250, Easing.SinInOut),
                InfoBox.ScaleTo(1, 250, Easing.SinInOut), // BackOut gives a tiny "bounce"
                BackgroundDimmer.FadeTo(0.5, 250) // Dims the background to 50% black
            );
        }
    }
}