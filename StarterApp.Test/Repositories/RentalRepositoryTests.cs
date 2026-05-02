using Microsoft.EntityFrameworkCore;
using StarterApp.Database.Data;
using StarterApp.Database.Data.Repositories;
using StarterApp.Database.Models;
using Xunit;

namespace StarterApp.Test.Repositories;

public class RentalRepositoryTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly RentalRepository _repository;

    public RentalRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _context = new AppDbContext(options);
        _repository = new RentalRepository(_context);
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    private async Task SeedDataAsync()
    {
        _context.Users.AddRange(
            new User { Id = 1, Email = "owner@test.com", FirstName = "Owner", LastName = "User", PasswordHash = "", PasswordSalt = "" },
            new User { Id = 2, Email = "borrower@test.com", FirstName = "Borrower", LastName = "User", PasswordHash = "", PasswordSalt = "" }
        );
        _context.Items.Add(new Item { Id = 1, Title = "Drill", DailyRate = 5, OwnerId = 1 });
        await _context.SaveChangesAsync();
    }

    [Fact]
    public async Task CreateAsync_SavesRentalToDatabase()
    {
        // Arrange
        await SeedDataAsync();
        var rental = new Rental
        {
            ItemId = 1,
            BorrowerId = 2,
            StartDate = DateTime.UtcNow.AddDays(1),
            EndDate = DateTime.UtcNow.AddDays(3),
            TotalPrice = 10,
            Status = "Requested"
        };

        // Act
        var result = await _repository.CreateAsync(rental);

        // Assert
        Assert.NotEqual(0, result.Id);
        Assert.Equal("Requested", result.Status);
    }

    [Fact]
    public async Task GetOutgoingAsync_ReturnsRentalsForBorrower()
    {
        // Arrange
        await SeedDataAsync();
        _context.Rentals.Add(new Rental { ItemId = 1, BorrowerId = 2, StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow.AddDays(2), TotalPrice = 10, Status = "Requested" });
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetOutgoingAsync(2);

        // Assert
        Assert.Single(result);
        Assert.Equal(2, result[0].BorrowerId);
    }

    [Fact]
    public async Task GetIncomingAsync_ReturnsRentalsForOwner()
    {
        // Arrange
        await SeedDataAsync();
        _context.Rentals.Add(new Rental { ItemId = 1, BorrowerId = 2, StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow.AddDays(2), TotalPrice = 10, Status = "Requested" });
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetIncomingAsync(1);

        // Assert
        Assert.Single(result);
    }

    [Fact]
    public async Task UpdateAsync_ChangesRentalStatus()
    {
        // Arrange
        await SeedDataAsync();
        var rental = new Rental { ItemId = 1, BorrowerId = 2, StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow.AddDays(2), TotalPrice = 10, Status = "Requested" };
        _context.Rentals.Add(rental);
        await _context.SaveChangesAsync();

        // Act
        rental.Status = "Approved";
        var result = await _repository.UpdateAsync(rental);

        // Assert
        Assert.Equal("Approved", result.Status);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenNotFound()
    {
        // Act
        var result = await _repository.GetByIdAsync(999);

        // Assert
        Assert.Null(result);
    }
}