using Microsoft.EntityFrameworkCore;
using StarterApp.Database.Data;
using StarterApp.Database.Data.Repositories;
using StarterApp.Database.Models;
using Xunit;

namespace StarterApp.Test.Repositories;

public class ItemRepositoryTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly ItemRepository _repository;

    public ItemRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _context = new AppDbContext(options);
        _repository = new ItemRepository(_context);
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    [Fact]
    public async Task GetAllAsync_ReturnsOnlyAvailableItems()
    {
        // Arrange
        var user = new User { Id = 1, Email = "test@test.com", FirstName = "Test", LastName = "User", PasswordHash = "", PasswordSalt = "" };
        _context.Users.Add(user);
        _context.Items.Add(new Item { Title = "Drill", DailyRate = 5, OwnerId = 1, IsAvailable = true });
        _context.Items.Add(new Item { Title = "Saw", DailyRate = 3, OwnerId = 1, IsAvailable = false });
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        Assert.Single(result);
        Assert.Equal("Drill", result[0].Title);
    }

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

        // Assert
        Assert.NotEqual(0, result.Id);
        Assert.Equal("Hammer", result.Title);
    }

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

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenItemNotFound()
    {
        // Act
        var result = await _repository.GetByIdAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByOwnerIdAsync_ReturnsOnlyOwnerItems()
    {
        // Arrange
        _context.Users.AddRange(
            new User { Id = 1, Email = "a@a.com", FirstName = "A", LastName = "A", PasswordHash = "", PasswordSalt = "" },
            new User { Id = 2, Email = "b@b.com", FirstName = "B", LastName = "B", PasswordHash = "", PasswordSalt = "" }
        );
        _context.Items.AddRange(
            new Item { Title = "Drill", DailyRate = 5, OwnerId = 1 },
            new Item { Title = "Saw", DailyRate = 3, OwnerId = 2 }
        );
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByOwnerIdAsync(1);

        // Assert
        Assert.Single(result);
        Assert.Equal("Drill", result[0].Title);
    }
}