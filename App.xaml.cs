using ScrollBase.Views;

namespace ScrollBase
{
    public partial class App : Application
    {
        public App(ScrollBase.Views.Signup signupPage)
        {
            InitializeComponent();

            MainPage = new NavigationPage(signupPage);
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            // This tells MAUI to use your AppShell as the root window. 
            // AppShell will then look at its XAML to decide which page to show first.
            return new Window(MainPage);
        }
    }
}