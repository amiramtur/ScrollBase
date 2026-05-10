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
            // 1. Signout from firebase
            var authClient = Handler.MauiContext.Services.GetService<FirebaseAuthClient>();
            authClient?.SignOut();

            // 2. Hop onto the Main UI Thread to update the screen
            MainThread.BeginInvokeOnMainThread(() =>
            {
                // 3. Ask Dependency Injection for a fresh signup page
                var signupPage = Handler.MauiContext.Services.GetService<Signup>();

                // 4. Swap the root back to the NavigationPage holding signup screen
                Application.Current.MainPage = new NavigationPage(signupPage);
            });
        }
    }
}
