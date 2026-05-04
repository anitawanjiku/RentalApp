using Microsoft.EntityFrameworkCore;
using StarterApp.Database.Data;
using StarterApp.Database.Data.Repositories;
using StarterApp.Database.Models;
using Xunit;

namespace StarterApp.Test.Repositories;

/// <summary>
/// Unit tests for the ItemRepository class.
/// Uses an in-memory database to avoid depending on a real PostgreSQL connection.
/// </summary>
public class ItemRepositoryTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly ItemRepository _repository;

    /// <summary>
    /// Sets up a fresh in-memory database before each test.
    /// Using a unique database name ensures tests don't interfere with each other.
    /// </summary>
    public ItemRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _context = new AppDbContext(options);
        _repository = new ItemRepository(_context);
    }

    /// <summary>
    /// Cleans up the database context after each test.
    /// </summary>
    public void Dispose()
    {
        _context.Dispose();
    }

    /// <summary>
    /// Verifies that GetAllAsync only returns items marked as available.
    /// Items with IsAvailable = false should be excluded from results.
    /// </summary>
    [Fact]
    public async Task GetAllAsync_ReturnsOnlyAvailableItems()
    {
        // Arrange - add one available and one unavailable item
        var user = new User { Id = 1, Email = "test@test.com", FirstName = "Test", LastName = "User", PasswordHash = "", PasswordSalt = "" };
        _context.Users.Add(user);
        _context.Items.Add(new Item { Title = "Drill", DailyRate = 5, OwnerId = 1, IsAvailable = true });
        _context.Items.Add(new Item { Title = "Saw", DailyRate = 3, OwnerId = 1, IsAvailable = false });
        await _context.SaveChangesAsync();

        // Act - fetch all available items
        var result = await _repository.GetAllAsync();

        // Assert - only the available item should be returned
        Assert.Single(result);
        Assert.Equal("Drill", result[0].Title);
    }

    /// <summary>
    /// Verifies that CreateAsync saves a new item to the database
    /// and assigns it a valid ID.
    /// </summary>
    [Fact]
    public async Task CreateAsync_SavesItemToDatabase()
    {
        // Arrange
        var user = new User { Id = 1, Email = "test@test.com", FirstName = "Test", LastName = "User", PasswordHash = "", PasswordSalt = "" };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        var item = new Item { Title = "Hammer", DailyRate = 2, OwnerId = 1 };

        // Act
        var result = await _repository.CreateAsync(item);

        // Assert - item should have been assigned a database ID
        Assert.NotEqual(0, result.Id);
        Assert.Equal("Hammer", result.Title);
    }

    /// <summary>
    /// Verifies that GetByIdAsync returns the correct item when given a valid ID.
    /// </summary>
    [Fact]
    public async Task GetByIdAsync_ReturnsCorrectItem()
    {
        // Arrange
        var user = new User { Id = 1, Email = "test@test.com", FirstName = "Test", LastName = "User", PasswordHash = "", PasswordSalt = "" };
        _context.Users.Add(user);
        var item = new Item { Title = "Ladder", DailyRate = 8, OwnerId = 1 };
        _context.Items.Add(item);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdAsync(item.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Ladder", result.Title);
    }

    /// <summary>
    /// Verifies that GetByIdAsync returns null when no item exists with the given ID.
    /// </summary>
    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenItemNotFound()
    {
        // Act
        var result = await _repository.GetByIdAsync(999);

        // Assert
        Assert.Null(result);
    }

    /// <summary>
    /// Verifies that GetByOwnerIdAsync only returns items belonging to the specified owner.
    /// </summary>
    [Fact]
    public async Task GetByOwnerIdAsync_ReturnsOnlyOwnerItems()
    {
        // Arrange - two users each with one item
        _context.Users.AddRange(
            new User { Id = 1, Email = "a@a.com", FirstName = "A", LastName = "A", PasswordHash = "", PasswordSalt = "" },
            new User { Id = 2, Email = "b@b.com", FirstName = "B", LastName = "B", PasswordHash = "", PasswordSalt = "" }
        );
        _context.Items.AddRange(
            new Item { Title = "Drill", DailyRate = 5, OwnerId = 1 },
            new Item { Title = "Saw", DailyRate = 3, OwnerId = 2 }
        );
        await _context.SaveChangesAsync();

        // Act - fetch items for owner 1 only
        var result = await _repository.GetByOwnerIdAsync(1);

        // Assert - only owner 1's item should be returned
        Assert.Single(result);
        Assert.Equal("Drill", result[0].Title);
    }
}