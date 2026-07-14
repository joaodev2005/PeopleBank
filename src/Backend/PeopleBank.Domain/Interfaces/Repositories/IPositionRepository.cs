using PeopleBank.Domain.Entities;

namespace PeopleBank.Domain.Interfaces.Repositories;

public interface IPositionRepository
{
    Task<Position?> GetByIdAsync(Guid id);
    Task AddAsync(Position position);
}