using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StarterApp.Database.Data.Repositories;
using StarterApp.Database.Models;
using System.Collections.ObjectModel;
using StarterApp.Views;

namespace StarterApp.ViewModels;

public partial class ItemsListViewModel : BaseViewModel
{
    private readonly IItemRepository _itemRepository;

    [ObservableProperty]
    private ObservableCollection<Item> items = new();

    [ObservableProperty]
    private Item? selectedItem;

    public ItemsListViewModel(IItemRepository itemRepository)
    {
        _itemRepository = itemRepository;
        Title = "Available Items";
    }


    [RelayCommand]
    public async Task NavigateToCreateAsync()
    {
        await Shell.Current.GoToAsync(nameof(CreateItemPage));
    }

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
}