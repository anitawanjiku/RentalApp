using StarterApp.Database.Models;

namespace StarterApp.Database.Data.Repositories;

public interface IRentalRepository
{
    Task<List<Rental>> GetIncomingAsync(int ownerId);
    Task<List<Rental>> GetOutgoingAsync(int borrowerId);
    Task<Rental?> GetByIdAsync(int id);
    Task<Rental> CreateAsync(Rental rental);
    Task<Rental> UpdateAsync(Rental rental);
}