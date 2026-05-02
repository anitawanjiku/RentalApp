using StarterApp.ViewModels;
using StarterApp.Views;

namespace StarterApp;

public partial class AppShell : Shell
{
    public AppShell(AppShellViewModel viewModel)
    {
        BindingContext = viewModel;
        InitializeComponent();

        Routing.RegisterRoute(nameof(ItemsListPage), typeof(ItemsListPage));
        Routing.RegisterRoute(nameof(CreateItemPage), typeof(CreateItemPage));
    }
}