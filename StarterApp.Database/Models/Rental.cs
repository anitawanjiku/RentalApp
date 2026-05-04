namespace StarterApp.Database.Models;

/// <summary>
/// Represents a rental request made by a borrower for a listed item.
/// Tracks the full lifecycle of a rental from request to completion.
/// </summary>
public class Rental
{
    /// <summary>Unique identifier for the rental.</summary>
    public int Id { get; set; }

    /// <summary>Foreign key linking the rental to the item being rented.</summary>
    public int ItemId { get; set; }

    /// <summary>Navigation property to load the item's details.</summary>
    public Item? Item { get; set; }

    /// <summary>Foreign key linking the rental to the user borrowing the item.</summary>
    public int BorrowerId { get; set; }

    /// <summary>Navigation property to load the borrower's details.</summary>
    public User? Borrower { get; set; }

    /// <summary>The date the rental period begins.</summary>
    public DateTime StartDate { get; set; }

    /// <summary>The date the rental period ends.</summary>
    public DateTime EndDate { get; set; }

    /// <summary>The total cost of the rental calculated from DailyRate and duration.</summary>
    public decimal TotalPrice { get; set; }

    /// <summary>
    /// The current status of the rental.
    /// Possible values: "Requested", "Approved", "Rejected", "Returned", "Completed"
    /// </summary>
    public string Status { get; set; } = "Requested";

    /// <summary>The date and time the rental request was created.</summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}