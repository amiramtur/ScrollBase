using Microsoft.Extensions.DependencyInjection;
using System;
using ScrollBase.Services;

namespace ScrollBase.Views;

public partial class AddWebPage : ContentPage
{
    public AddWebPage()
    {
        InitializeComponent();

        // Use MauiApplication.Current.Services to resolve the VM from the MAUI DI container.
        // Application.Current does not expose a Services property directly.
        var mauiApp = Microsoft.Maui.Controls.Application.Current as Microsoft.Maui.Controls.Application;
        var services = (mauiApp as IServiceProvider)?.GetService<IServiceProvider>() ?? App.Current?.Handler?.MauiContext?.Services;

        BindingContext = services?.GetService<ScrollBase.ViewModels.ManageWebPagesVM>()
                         ?? throw new InvalidOperationException("AddWebPageVM not registered in DI container.");

        AddButton.Clicked += (_, __) =>
        {
            System.Diagnostics.Debug.WriteLine("AddButton clicked (UI) - code-behind handler");
            if (BindingContext is ScrollBase.ViewModels.ManageWebPagesVM vm)
            {
                if (vm.AddPageCommand.CanExecute(null))
                    vm.AddPageCommand.Execute(null);
            }
        };
    }
}