using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StarterApp.Database.Data.Repositories;
using StarterApp.Database.Models;
using System.Collections.ObjectModel;
using StarterApp.Views;

namespace StarterApp.ViewModels;

/// <summary>
/// ViewModel for the Items List page.
/// Follows the MVVM pattern — this class handles all logic for the view,
/// exposing data through ObservableProperty and actions through RelayCommands.
/// The view (ItemsListPage.xaml) binds to these properties without containing any logic itself.
/// </summary>
public partial class ItemsListViewModel : BaseViewModel
{
    private readonly IItemRepository _itemRepository;
    private readonly IRentalRepository _rentalRepository;

    /// <summary>
    /// The list of available items displayed in the UI.
    /// ObservableCollection automatically notifies the UI when items are added or removed.
    /// </summary>
    [ObservableProperty]
    private ObservableCollection<Item> items = new();

    /// <summary>
    /// The currently selected item, if any.
    /// </summary>
    [ObservableProperty]
    private Item? selectedItem;

    /// <summary>
    /// Constructor uses dependency injection to receive repository instances.
    /// This means the ViewModel doesn't create its own dependencies — they are provided externally.
    /// </summary>
    public ItemsListViewModel(IItemRepository itemRepository, IRentalRepository rentalRepository)
    {
        _itemRepository = itemRepository;
        _rentalRepository = rentalRepository;
        Title = "Available Items";
    }

    /// <summary>
    /// Navigates to the Create Item page.
    /// RelayCommand exposes this as a bindable command in the XAML view.
    /// </summary>
    [RelayCommand]
    public async Task NavigateToCreateAsync()
    {
        await Shell.Current.GoToAsync(nameof(CreateItemPage));
    }

    /// <summary>
    /// Loads all available items from the database into the Items collection.
    /// Sets IsBusy to true while loading to show a loading indicator in the UI.
    /// </summary>
    [RelayCommand]
    public async Task LoadItemsAsync()
    {
        if (IsBusy) return;
        IsBusy = true;
        ClearError();

        try
        {
            var result = await _itemRepository.GetAllAsync();
            Items.Clear();
            foreach (var item in result)
                Items.Add(item);
        }
        catch (Exception ex)
        {
            SetError($"Failed to load items: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    /// <summary>
    /// Creates a rental request for the specified item.
    /// Fetches item details, creates a Rental object with status "Requested",
    /// and saves it to the database via the RentalRepository.
    /// </summary>
    [RelayCommand]
    public async Task RequestRentalAsync(int itemId)
    {
        if (IsBusy) return;
        IsBusy = true;
        ClearError();

        try
        {
            var item = await _itemRepository.GetByIdAsync(itemId);
            if (item == null) { SetError("Item not found"); return; }

            var rental = new Rental
            {
                ItemId = itemId,
                BorrowerId = 1, // TODO: replace with logged-in user
                StartDate = DateTime.UtcNow.AddDays(1),
                EndDate = DateTime.UtcNow.AddDays(3),
                TotalPrice = item.DailyRate * 2,
                Status = "Requested"
            };

            await _rentalRepository.CreateAsync(rental);
            await Shell.Current.DisplayAlert("Success", "Rental requested!", "OK");
        }
        catch (Exception ex)
        {
            SetError($"Failed to request rental: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }
}