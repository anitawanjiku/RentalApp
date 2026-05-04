using Moq;
using StarterApp.Database.Data.Repositories;
using StarterApp.Database.Models;
using Xunit;

namespace StarterApp.Test.Repositories;

/// <summary>
/// Demonstrates mock-based testing using Moq.
/// Instead of using a real database, we mock the repository interface
/// to test behaviour in isolation.
/// </summary>
public class ItemRepositoryMockTests
{
    /// <summary>
    /// Verifies that a mock repository returns the expected items.
    /// This pattern is used when testing classes that depend on IItemRepository
    /// without needing a real database connection.
    /// </summary>
    [Fact]
    public async Task MockRepository_ReturnsExpectedItems()
    {
        // Arrange - create a mock of the repository interface
        var mockRepo = new Mock<IItemRepository>();
        var expectedItems = new List<Item>
        {
            new Item { Id = 1, Title = "Drill", DailyRate = 5, OwnerId = 1 },
            new Item { Id = 2, Title = "Ladder", DailyRate = 8, OwnerId = 1 }
        };

        mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(expectedItems);

        // Act - call the mocked method
        var result = await mockRepo.Object.GetAllAsync();

        // Assert - verify the mock returned the expected data
        Assert.Equal(2, result.Count);
        Assert.Equal("Drill", result[0].Title);
    }

    /// <summary>
    /// Verifies that CreateAsync is called exactly once when adding an item.
    /// </summary>
    [Fact]
    public async Task MockRepository_CreateAsync_CalledOnce()
    {
        // Arrange
        var mockRepo = new Mock<IItemRepository>();
        var item = new Item { Title = "Hammer", DailyRate = 2, OwnerId = 1 };

        mockRepo.Setup(r => r.CreateAsync(It.IsAny<Item>())).ReturnsAsync(item);

        // Act
        await mockRepo.Object.CreateAsync(item);

        // Assert - verify CreateAsync was called exactly once
        mockRepo.Verify(r => r.CreateAsync(It.IsAny<Item>()), Times.Once);
    }

    /// <summary>
    /// Verifies that GetByIdAsync returns null when no item exists.
    /// </summary>
    [Fact]
    public async Task MockRepository_GetByIdAsync_ReturnsNull_WhenNotFound()
    {
        // Arrange
        var mockRepo = new Mock<IItemRepository>();
        mockRepo.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Item?)null);

        // Act
        var result = await mockRepo.Object.GetByIdAsync(999);

        // Assert
        Assert.Null(result);
    }
}