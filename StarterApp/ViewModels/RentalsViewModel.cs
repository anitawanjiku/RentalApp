using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StarterApp.Database.Data.Repositories;
using StarterApp.Database.Models;
using System.Collections.ObjectModel;

namespace StarterApp.ViewModels;

public partial class RentalsViewModel : BaseViewModel
{
    private readonly IRentalRepository _rentalRepository;
    private readonly IItemRepository _itemRepository;

    [ObservableProperty] private ObservableCollection<Rental> incomingRentals = new();
    [ObservableProperty] private ObservableCollection<Rental> outgoingRentals = new();
    [ObservableProperty] private int currentUserId = 1; // TODO: replace with logged-in user

    public RentalsViewModel(IRentalRepository rentalRepository, IItemRepository itemRepository)
    {
        _rentalRepository = rentalRepository;
        _itemRepository = itemRepository;
        Title = "Rentals";
    }

    [RelayCommand]
    public async Task LoadRentalsAsync()
    {
        if (IsBusy) return;
        IsBusy = true;
        ClearError();

        try
        {
            var incoming = await _rentalRepository.GetIncomingAsync(CurrentUserId);
            var outgoing = await _rentalRepository.GetOutgoingAsync(CurrentUserId);

            IncomingRentals.Clear();
            OutgoingRentals.Clear();

            foreach (var r in incoming) IncomingRentals.Add(r);
            foreach (var r in outgoing) OutgoingRentals.Add(r);
        }
        catch (Exception ex)
        {
            SetError($"Failed to load rentals: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

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
                BorrowerId = CurrentUserId,
                StartDate = DateTime.UtcNow.AddDays(1),
                EndDate = DateTime.UtcNow.AddDays(3),
                TotalPrice = item.DailyRate * 2,
                Status = "Requested"
            };

            await _rentalRepository.CreateAsync(rental);
            await LoadRentalsAsync();
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

    [RelayCommand]
    public async Task UpdateRentalStatusAsync(Rental rental)
    {
        try
        {
            await _rentalRepository.UpdateAsync(rental);
            await LoadRentalsAsync();
        }
        catch (Exception ex)
        {
            SetError($"Failed to update rental: {ex.Message}");
        }
    }
}