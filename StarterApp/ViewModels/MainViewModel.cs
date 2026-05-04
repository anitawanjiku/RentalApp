using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StarterApp.Database.Models;
using StarterApp.Services;
using StarterApp.Views;

namespace StarterApp.ViewModels;

/// <summary>
/// ViewModel for the main dashboard page.
/// Manages user information display and navigation to all major sections of the app.
/// Follows the MVVM pattern — the MainPage.xaml binds to properties and commands
/// defined here without containing any logic itself.
/// </summary>
public partial class MainViewModel : BaseViewModel
{
    private readonly IAuthenticationService _authService;
    private readonly INavigationService _navigationService;

    /// <summary>The currently logged-in user.</summary>
    [ObservableProperty] private User? currentUser;

    /// <summary>Personalised welcome message shown on the dashboard.</summary>
    [ObservableProperty] private string welcomeMessage = string.Empty;

    /// <summary>Controls visibility of admin-only features.</summary>
    [ObservableProperty] private bool isAdmin;

    /// <summary>
    /// Default constructor for design-time support.
    /// </summary>
    public MainViewModel()
    {
        Title = "Dashboard";
    }

    /// <summary>
    /// Main constructor — receives services via dependency injection.
    /// This means MainViewModel doesn't create its own dependencies,
    /// making it easier to test and maintain.
    /// </summary>
    public MainViewModel(IAuthenticationService authService, INavigationService navigationService)
    {
        _authService = authService;
        _navigationService = navigationService;
        Title = "Dashboard";
        LoadUserData();
    }

    /// <summary>
    /// Loads the current user's data and determines admin status.
    /// </summary>
    private void LoadUserData()
    {
        CurrentUser = _authService.CurrentUser;
        IsAdmin = _authService.HasRole("Admin");
        if (CurrentUser != null)
            WelcomeMessage = $"Welcome, {CurrentUser.FullName}!";
    }

    /// <summary>
    /// Navigates to the Items List page where users can browse available items.
    /// </summary>
    [RelayCommand]
    private async Task NavigateToItemsAsync()
    {
        await Shell.Current.GoToAsync(nameof(ItemsListPage));
    }

    /// <summary>
    /// Navigates to the Rentals page where users can view their rental requests.
    /// </summary>
    [RelayCommand]
    private async Task NavigateToRentalsAsync()
    {
        await Shell.Current.GoToAsync(nameof(RentalsPage));
    }

    /// <summary>
    /// Logs out the current user after confirmation and navigates back to the login page.
    /// </summary>
    [RelayCommand]
    private async Task LogoutAsync()
    {
        var result = await Application.Current.MainPage.DisplayAlert(
            "Logout", "Are you sure you want to logout?", "Yes", "No");
        if (result)
        {
            await _authService.LogoutAsync();
            await _navigationService.NavigateToAsync("LoginPage");
        }
    }

    /// <summary>Navigates to the user profile page.</summary>
    [RelayCommand]
    private async Task NavigateToProfileAsync()
    {
        await _navigationService.NavigateToAsync("TempPage");
    }

    /// <summary>Navigates to the app settings page.</summary>
    [RelayCommand]
    private async Task NavigateToSettingsAsync()
    {
        await _navigationService.NavigateToAsync("TempPage");
    }

    /// <summary>
    /// Navigates to the user management page. Only accessible to admin users.
    /// </summary>
    [RelayCommand]
    private async Task NavigateToUserListAsync()
    {
        if (!IsAdmin)
        {
            await Application.Current.MainPage.DisplayAlert("Access Denied", "You don't have permission.", "OK");
            return;
        }
        await _navigationService.NavigateToAsync("UserListPage");
    }

    /// <summary>
    /// Refreshes the dashboard data.
    /// </summary>
    [RelayCommand]
    private async Task RefreshDataAsync()
    {
        try
        {
            IsBusy = true;
            LoadUserData();
            await Task.Delay(1000);
        }
        catch (Exception ex)
        {
            SetError($"Failed to refresh data: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }
}