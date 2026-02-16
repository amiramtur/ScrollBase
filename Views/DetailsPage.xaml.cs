using ScrollBase.ViewModels;

namespace ScrollBase.Views;

public partial class DetailsPage : ContentPage
{
	public DetailsPage(DetailsPageVM vm)
	{
		InitializeComponent();
        BindingContext = vm;
    }
}