namespace StarterApp.Database.Models;

/// <summary>
/// Represents an item listed for rent on the platform.
/// Each item belongs to an owner (User) and can have multiple rental requests.
/// </summary>
public class Item
{
    /// <summary>Unique identifier for the item.</summary>
    public int Id { get; set; }

    /// <summary>The name of the item, e.g. "Power Drill".</summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>A description of the item's condition and features.</summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>The cost to rent the item per day in GBP.</summary>
    public decimal DailyRate { get; set; }

    /// <summary>The category of the item, e.g. "Tools" or "Games".</summary>
    public string Category { get; set; } = string.Empty;

    /// <summary>The location where the item can be collected.</summary>
    public string Location { get; set; } = string.Empty;

    /// <summary>Foreign key linking the item to its owner in the Users table.</summary>
    public int OwnerId { get; set; }

    /// <summary>Navigation property to load the owner's details.</summary>
    public User? Owner { get; set; }

    /// <summary>The date and time the item was listed.</summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>Whether the item is currently available to rent.</summary>
    public bool IsAvailable { get; set; } = true;
}