using Firebase.Auth;
using Firebase.Auth.Providers;
using Firebase.Auth.Repository;
using Firebase.Database;
using Microsoft.Extensions.Logging;
using ScrollBase.Models;
using ScrollBase.Services;
using ScrollBase.ViewModels;
using ScrollBase.Views;

namespace ScrollBase
{

    // http://scrollbasedb.firebaseapp.com/
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
    		builder.Logging.AddDebug();

            builder.Services.AddSingleton(new FirebaseAuthClient(new FirebaseAuthConfig()
            {
                ApiKey = "AIzaSyAdX5bhFe52PlSWD47e5i9Xf_IK3ZbaTqU",
                AuthDomain = "scrollbasedb.firebaseapp.com",
                Providers = new FirebaseAuthProvider[]
                   {
                       new EmailProvider()
                   },
                UserRepository = new FileUserRepository("appuser")//persist data into %AppData%\appuser
            }));
#endif
            builder.Services.AddSingleton(new FirebaseClient("https://scrollbasedb-default-rtdb.europe-west1.firebasedatabase.app/"));

            builder.Services.AddSingleton<IConnectivity>(Connectivity.Current);
            builder.Services.AddSingleton<MainPage>();
            builder.Services.AddSingleton<MainPageVM>();
            builder.Services.AddSingleton<Login>();
            builder.Services.AddSingleton<LoginVM>();
            builder.Services.AddSingleton<Signup>();
            builder.Services.AddSingleton<SignupVM>();
            builder.Services.AddSingleton<AppUserListVM>();
            builder.Services.AddSingleton<AppUserList>();
            builder.Services.AddSingleton<DetailsPageVM>();
            builder.Services.AddSingleton<DetailsPage>();
            builder.Services.AddSingleton<ManageWebPagesVM>();
            builder.Services.AddSingleton<NetworkService>();

            return builder.Build();
        }
    }
}
