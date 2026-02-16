using ScrollBase.ViewModels;

namespace ScrollBase.Views;

public partial class AppUserList : ContentPage
{
	public AppUserList(AppUserListVM vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}
}