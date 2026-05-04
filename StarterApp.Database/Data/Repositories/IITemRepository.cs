using StarterApp.Database.Models;

namespace StarterApp.Database.Data.Repositories;

/// <summary>
/// Defines the contract for item data access operations.
/// ViewModels depend on this interface rather than the concrete implementation,
/// which allows the database layer to be swapped out without changing ViewModel code.
/// </summary>
public interface IItemRepository
{
    /// <summary>
    /// Retrieves all available items from the database.
    /// </summary>
    Task<List<Item>> GetAllAsync();

    /// <summary>
    /// Retrieves a single item by its unique ID, including owner information.
    /// Returns null if no item is found.
    /// </summary>
    Task<Item?> GetByIdAsync(int id);

    /// <summary>
    /// Saves a new item to the database and returns the saved item with its assigned ID.
    /// </summary>
    Task<Item> CreateAsync(Item item);

    /// <summary>
    /// Updates an existing item in the database and returns the updated item.
    /// </summary>
    Task<Item> UpdateAsync(Item item);

    /// <summary>
    /// Retrieves all items listed by a specific owner.
    /// </summary>
    Task<List<Item>> GetByOwnerIdAsync(int ownerId);
}