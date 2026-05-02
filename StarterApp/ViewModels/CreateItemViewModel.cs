using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StarterApp.Database.Data.Repositories;
using StarterApp.Database.Models;

namespace StarterApp.ViewModels;

public partial class CreateItemViewModel : BaseViewModel
{
    private readonly IItemRepository _itemRepository;

    [ObservableProperty] private string itemTitle = string.Empty;
    [ObservableProperty] private string description = string.Empty;
    [ObservableProperty] private string category = string.Empty;
    [ObservableProperty] private string location = string.Empty;
    [ObservableProperty] private string dailyRate = string.Empty;

    public CreateItemViewModel(IItemRepository itemRepository)
    {
        _itemRepository = itemRepository;
        Title = "List an Item";
    }

    [RelayCommand]
    public async Task CreateItemAsync()
    {
        if (IsBusy) return;
        if (string.IsNullOrWhiteSpace(ItemTitle)) { SetError("Title is required"); return; }
        if (!decimal.TryParse(DailyRate, out var rate)) { SetError("Invalid daily rate"); return; }

        IsBusy = true;
        ClearError();

        try
        {
            var item = new Item
            {
                Title = ItemTitle,
                Description = Description,
                Category = Category,
                Location = Location,
                DailyRate = rate,
                OwnerId = 1 // TODO: replace with logged-in user ID
            };

            await _itemRepository.CreateAsync(item);
            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            SetError($"Failed to create item: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }
}