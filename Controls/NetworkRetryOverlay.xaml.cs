namespace ScrollBase.Controls;

public partial class NetworkRetryOverlay : ContentView
{
    public NetworkRetryOverlay()
    {
#if NET6_0_OR_GREATER
        this.LoadFromXaml(typeof(NetworkRetryOverlay));
#else
        InitializeComponent();
#endif
    }
}