using Microsoft.EntityFrameworkCore;
using PeopleBank.Domain.Entities;
using PeopleBank.Domain.Interfaces.Repositories;
using PeopleBank.Infrastructure.Data;

namespace PeopleBank.Infrastructure.Repositories;

public class PositionRepository : IPositionRepository
{
    private readonly PeopleBankDbContext _context;

    public PositionRepository(PeopleBankDbContext context) => _context = context;

    public async Task<Position?> GetByIdAsync(Guid id) =>
        await _context.Positions.FirstOrDefaultAsync(p => p.Id == id);

    public async Task AddAsync(Position position) =>
        await _context.Positions.AddAsync(position);
}