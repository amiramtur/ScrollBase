using Firebase.Auth;
using ScrollBase.Views;

namespace ScrollBase
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(DetailsPage), typeof(DetailsPage));
        }

        private void OnLogoutClicked(object sender, EventArgs e)
        {
            // signout from firebase
            var authClient = Handler.MauiContext.Services.GetService<FirebaseAuthClient>();
            authClient?.SignOut();

            MainThread.BeginInvokeOnMainThread(() =>
            {
                // asking Dependency Injection for a fresh signup page
                var signupPage = Handler.MauiContext.Services.GetService<Signup>();

                Application.Current.MainPage = new NavigationPage(signupPage);
            });
        }
    }
}
